using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using AliHaFFMPEG.Core;

namespace AliHaFFMPEG
{
    /// <summary>About box: version, author, ffmpeg info, update check and ffmpeg download.</summary>
    public partial class AboutForm : Form
    {
        private readonly string _ffmpegPath;
        private readonly string _installDirectory;
        private readonly string _repository;
        private readonly Action _onToolsChanged;

        private UpdateInfo _update;

        public AboutForm(string ffmpegPath, string installDirectory, string repository, Action onToolsChanged = null)
        {
            InitializeComponent();
            AppIcon.Apply(this);

            _ffmpegPath = ffmpegPath;
            _installDirectory = installDirectory;
            _repository = string.IsNullOrWhiteSpace(repository)
                ? UpdateChecker.DefaultRepository
                : repository.Trim();
            _onToolsChanged = onToolsChanged;

            // Any text change can change a wrapped label's height, so re-flow the
            // stack whenever it happens. Without this the buttons/links below stay
            // put and a longer path or status ends up drawn over them.
            lblFfmpeg.TextChanged += OnStackedTextChanged;
            lblStatus.TextChanged += OnStackedTextChanged;

            StackAboutContent();
        }

        private bool _stacking;

        private void OnStackedTextChanged(object sender, EventArgs e)
        {
            // TextChanged fires *before* the label's AutoSize has re-measured its
            // wrapped height, so stacking right now would use the old height.
            // Force the layout, stack, then stack once more after the pending
            // layout pass so a height that settles later still pushes the
            // controls below it instead of overlapping them.
            var label = sender as Label;
            label?.PerformLayout();

            StackAboutContent();

            if (!IsHandleCreated || IsDisposed)
            {
                return;
            }

            BeginInvoke(new Action(StackAboutContent));
        }

        /// <summary>
        /// Stacks the variable-height text blocks top-down so a long wrapped
        /// ffmpeg path or multi-line status can never cover the links/buttons
        /// below them (fixed Designer coordinates cannot know the wrapped height).
        /// </summary>
        private void StackAboutContent()
        {
            if (_stacking) { return; }
            _stacking = true;
            try
            {
                var y = lblDescription.Top;
                lblFfmpeg.Top = y + lblDescription.Height + 6;
                y = lblFfmpeg.Top + lblFfmpeg.Height + 6;
                lblStatus.Top = y;
                y += lblStatus.Height + 8;
                lnkReleases.Top = y;
                lnkGithub.Top = y;
                y += Math.Max(lnkReleases.Height, lnkGithub.Height) + 10;
                btnCheckUpdate.Top = y;
                btnInstallUpdate.Top = y;
                y += btnCheckUpdate.Height + 8;
                btnDownloadFfmpeg.Top = y;
                btnOpenDataFolder.Top = y;
                y += btnDownloadFfmpeg.Height + 8;
                progressBar1.Top = y;
                y += progressBar1.Height + 8;
                btnClose.Top = y;
                ClientSize = new System.Drawing.Size(ClientSize.Width, y + btnClose.Height + 16);
            }
            finally
            {
                _stacking = false;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            lblVersion.Text = AppVersion.GetTitle();
            lblAuthor.Text = "Author: Ali Hamidi";
            Text = AppVersion.GetTitle();
            RefreshFfmpegInfo();
            StackAboutContent();
            lnkReleases.Text = "github.com/" + _repository + "/releases";
            lnkGithub.Text = "github.com/" + _repository;
            lblStatus.Text = "Ready.";

            // silent check when the dialog opens
            var unused = CheckForUpdatesAsync(true);
        }

        private void RefreshFfmpegInfo()
        {
            var path = string.IsNullOrEmpty(_ffmpegPath) ? FfmpegLocator.FindTool("ffmpeg.exe") : _ffmpegPath;
            if (string.IsNullOrEmpty(path))
            {
                lblFfmpeg.Text = "ffmpeg: NOT FOUND" + Environment.NewLine +
                                 "   use 'Download ffmpeg' below to install it";
                return;
            }

            var version = FfmpegManager.GetVersion(path);

            // The path goes on its own line and the label now auto-sizes with a
            // wrapping width, so a long path wraps instead of being clipped away.
            lblFfmpeg.Text = "ffmpeg: " + (version ?? "unknown version") + Environment.NewLine +
                             "   " + path;

            StackAboutContent();
        }

        private async Task CheckForUpdatesAsync(bool silent)
        {
            try
            {
                if (!silent)
                {
                    lblStatus.Text = "Checking GitHub for updates...";
                }

                var info = await UpdateChecker.CheckForUpdateAsync(_repository, AppVersion.Get()).ConfigureAwait(true);
                if (info == null)
                {
                    lblStatus.Text = silent
                        ? "Up to date (checked silently)."
                        : "You are running the newest version.";
                    btnInstallUpdate.Enabled = false;
                    return;
                }

                _update = info;
                btnInstallUpdate.Enabled = true;
                btnInstallUpdate.Text = "Install " + (info.TagName ?? info.Version.ToString());
                lblStatus.Text = "Update available: " + (info.ReleaseName ?? info.TagName) +
                                 (string.IsNullOrEmpty(info.AssetName)
                                     ? "  (no downloadable asset in the release)"
                                     : "  (" + info.AssetName + ")");
                StackAboutContent();
            }
            catch (Exception ex)
            {
                lblStatus.Text = silent
                    ? "Update check failed (offline, or the repository is not published yet)."
                    : "Update check failed: " + ex.Message;
                StackAboutContent();
                btnInstallUpdate.Enabled = false;
            }
        }

        private async void btnCheckUpdate_Click(object sender, EventArgs e)
        {
            await CheckForUpdatesAsync(false).ConfigureAwait(true);
        }

        private async void btnInstallUpdate_Click(object sender, EventArgs e)
        {
            if (_update == null || string.IsNullOrEmpty(_update.AssetUrl))
            {
                lblStatus.Text = "This release has no downloadable asset.";
                return;
            }

            if (!AppUpdater.CanWriteToInstallDirectory(_installDirectory))
            {
                lblStatus.Text = "Cannot write to " + _installDirectory +
                                 " - download the update manually from the releases page.";
                return;
            }

            try
            {
                btnInstallUpdate.Enabled = false;
                var progress = new Progress<int>(p => progressBar1.Value = Math.Min(100, Math.Max(0, p)));
                var target = Path.Combine(_installDirectory, _update.AssetName);

                lblStatus.Text = "Downloading " + _update.AssetName + "...";
                await Downloader.DownloadFileAsync(_update.AssetUrl, target, progress).ConfigureAwait(true);

                var script = AppUpdater.CreateApplyScript(target, _installDirectory, "AliHaFFMPEG.exe");
                lblStatus.Text = "Installing - the app will close and restart automatically.";

                if (AppUpdater.TryStartApplier(script, out var error))
                {
                    Application.Exit();
                }
                else
                {
                    lblStatus.Text = "Could not start the updater: " + error;
                    AppUpdater.OpenFolder(_installDirectory);
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Update failed: " + ex.Message;
                btnInstallUpdate.Enabled = true;
            }
        }

        private async void btnDownloadFfmpeg_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(
                    "Download the latest BtbN ffmpeg shared build for Windows (~85 MB: ffmpeg.exe, ffprobe.exe and the shared DLLs) and install it into:\n\n" +
                    _installDirectory + "\n\nContinue?",
                    "Download ffmpeg", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            try
            {
                btnDownloadFfmpeg.Enabled = false;
                var progress = new Progress<int>(p => progressBar1.Value = Math.Min(100, Math.Max(0, p)));
                lblStatus.Text = "Resolving the newest win64 shared build on GitHub...";

                var report = await FfmpegManager
                    .DownloadAndInstallAsync(_installDirectory, progress)
                    .ConfigureAwait(true);

                lblStatus.Text = report;
                _onToolsChanged?.Invoke();
                RefreshFfmpegInfo();
            }
            catch (Exception ex)
            {
                lblStatus.Text = "ffmpeg download failed: " + ex.Message;
            }
            finally
            {
                btnDownloadFfmpeg.Enabled = true;
                progressBar1.Value = 0;
            }
        }

        private void OpenUrl(string url)
        {
            try
            {
                Process.Start(new ProcessStartInfo(url)
                {
                    UseShellExecute = true
                });
            }
            catch
            {
                // ignore
            }
        }

        private void btnOpenDataFolder_Click(object sender, EventArgs e)
        {
            AppUpdater.OpenFolder(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AliHaFFMPEG"));
        }

        private void lnkGithub_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenUrl("https://github.com/" + _repository);
        }

        private void lnkReleases_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenUrl(UpdateChecker.BuildReleasesPageUrl(_repository));
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
