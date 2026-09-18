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

            _ffmpegPath = ffmpegPath;
            _installDirectory = installDirectory;
            _repository = string.IsNullOrWhiteSpace(repository)
                ? UpdateChecker.DefaultRepository
                : repository.Trim();
            _onToolsChanged = onToolsChanged;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            lblVersion.Text = "Version " + AppVersion.GetDisplay();
            lblAuthor.Text = "Author: Ali Hamidi";
            RefreshFfmpegInfo();
            lnkReleases.Text = "github.com/" + _repository + "/releases";
            lblStatus.Text = "Ready.";

            // silent check when the dialog opens
            var unused = CheckForUpdatesAsync(true);
        }

        private void RefreshFfmpegInfo()
        {
            var path = string.IsNullOrEmpty(_ffmpegPath) ? FfmpegLocator.FindTool("ffmpeg.exe") : _ffmpegPath;
            if (string.IsNullOrEmpty(path))
            {
                lblFfmpeg.Text = "ffmpeg: NOT FOUND - use 'Download ffmpeg' below.";
                return;
            }

            var version = FfmpegManager.GetVersion(path);
            lblFfmpeg.Text = "ffmpeg: " + (version ?? "unknown version") + Environment.NewLine + "   " + path;
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
            }
            catch (Exception ex)
            {
                lblStatus.Text = silent
                    ? "Update check failed (offline, or the repository is not published yet)."
                    : "Update check failed: " + ex.Message;
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

        private void btnOpenDataFolder_Click(object sender, EventArgs e)
        {
            AppUpdater.OpenFolder(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AliHaFFMPEG"));
        }

        private void lnkReleases_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo(UpdateChecker.BuildReleasesPageUrl(_repository))
                {
                    UseShellExecute = true
                });
            }
            catch
            {
                // ignore
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
