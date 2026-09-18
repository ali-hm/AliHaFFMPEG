using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace AliHaFFMPEG
{
    public partial class MainForm : Form
    {
        private List<CRFItem> _crfItems;
        private MyPreset _currentPreset;
        private Dictionary<string, MyPreset> savedPresets = new Dictionary<string, MyPreset>();
        private bool _rebindingPresets;
        private bool _suppressQuickPresetReset;
        private bool _applyingPreset;

        private string _ffmpegPath;
        private string _ffprobePath;

        private static readonly HashSet<string> MediaExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".mkv", ".mp4", ".avi", ".mov", ".wmv", ".flv", ".webm", ".ts", ".m4v", ".mpg", ".mpeg",
            ".vob", ".3gp", ".ogv", ".mp3", ".wav", ".aac", ".flac", ".ogg", ".m4a", ".wma", ".opus"
        };

        // conversion state
        private Process process;
        private bool canceled;
        private bool converting;
        private double? _totalDurationSeconds;
        private double _lastOutputSeconds;
        private double _lastSpeed;
        private DateTime _lastLogUiUpdate = DateTime.MinValue;
        private readonly object _errorTailLock = new object();
        private readonly StringBuilder _errorTail = new StringBuilder();
        private readonly object _progressLock = new object();
        private readonly List<QueueItem> _queue = new List<QueueItem>();
        private string _lastRunArgs;
        private string _lastLogFile;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            _currentPreset = new MyPreset();
            _crfItems = new List<CRFItem>() { new CRFItem() { Title = string.Empty, Number = -1 } };
            for (var i = 51; i >= -1; i--)
            {
                var title = i.ToString();
                switch (i)
                {
                    case 51: title += " (Worst Quality)"; break;
                    case 25: title += " (OK for most cases)"; break;
                    case 23: title += " (Default)"; break;
                    case 0: title += " (Lossless and Large Output)"; break;
                    case -1: title = string.Empty; break;
                }
                _crfItems.Add(new CRFItem() { Number = i, Title = title });
            }

            cmbCrf.DataSource = _crfItems;
            cmbCrf.SelectedIndex = 26;

            cmbVideoCodec.SelectedIndex = 0;
            cmbOutputFormat.SelectedIndex = 0;

            cmbQuickPreset.Items.Add(string.Empty);
            foreach (var name in BuiltInPresets.All.Keys)
            {
                cmbQuickPreset.Items.Add(name);
            }
            cmbQuickPreset.SelectedIndex = 0;

            _ffmpegPath = FfmpegTools.FindTool("ffmpeg.exe");
            _ffprobePath = FfmpegTools.FindTool("ffprobe.exe");
            if (string.IsNullOrEmpty(_ffmpegPath))
            {
                MessageBox.Show(
                    "ffmpeg.exe was not found.\n\nPut ffmpeg.exe (and ffprobe.exe) next to AliHaFFMPEG.exe, or add them to your PATH.\nConversion is disabled until then.",
                    "ffmpeg not found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            var hardware = FfmpegTools.DetectHardwareEncoders(_ffmpegPath);
            foreach (var encoder in hardware)
            {
                cmbVideoCodec.Items.Add(encoder);
            }
            lblHwHint.Text = hardware.Count > 0
                ? "Hardware encoders detected and added to the Video Codec list: " + string.Join(", ", hardware)
                : "No hardware encoders detected - CPU encoding (libx264/libx265) will be used.";

            LoadPresets();
            cmbSavedPresets_SelectedIndexChanged(sender, e);
            UpdateControlStates();
            lblProgress.Text = "Ready.";
        }

        #region Presets

        private static string PresetsDir =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AliHaFFMPEG");

        private static string PresetsPath => Path.Combine(PresetsDir, "presets.json");

        private void LoadPresets()
        {
            try
            {
                if (File.Exists(PresetsPath))
                {
                    savedPresets =
                        JsonConvert.DeserializeObject<Dictionary<string, MyPreset>>(File.ReadAllText(PresetsPath)) ??
                        new Dictionary<string, MyPreset>();
                }
                else if (File.Exists("presets.settings"))
                {
                    // migrate legacy presets stored next to the old working-directory file
                    savedPresets =
                        JsonConvert.DeserializeObject<Dictionary<string, MyPreset>>(File.ReadAllText("presets.settings")) ??
                        new Dictionary<string, MyPreset>();
                    SavePresets();
                }

                RebindPresets();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not load presets: " + ex.Message, "Presets",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                savedPresets = new Dictionary<string, MyPreset>();
                RebindPresets();
            }
        }

        private void SavePresets()
        {
            try
            {
                Directory.CreateDirectory(PresetsDir);
                File.WriteAllText(PresetsPath, JsonConvert.SerializeObject(savedPresets));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not save presets: " + ex.Message, "Presets",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void RebindPresets(string selectName = null)
        {
            _rebindingPresets = true;
            try
            {
                cmbSavedPresets.DataSource = savedPresets.Select(x => x.Value).ToList();
                if (selectName != null)
                {
                    var items = cmbSavedPresets.Items.Cast<MyPreset>().ToList();
                    var idx = items.FindIndex(p => p.PresetName == selectName);
                    if (idx >= 0)
                    {
                        cmbSavedPresets.SelectedIndex = idx;
                    }
                }
            }
            finally
            {
                _rebindingPresets = false;
            }
        }

        private static void SelectCombo(ComboBox cmb, string value)
        {
            cmb.SelectedIndex = string.IsNullOrEmpty(value) ? -1 : cmb.FindStringExact(value);
        }

        private void ApplyPreset(MyPreset preset)
        {
            if (preset == null)
            {
                return;
            }

            _applyingPreset = true;
            try
            {
                if (preset.CRF.HasValue)
                {
                    cmbCrf.SelectedIndex = _crfItems.FindIndex(x => x.Number == preset.CRF.Value);
                }
                else
                {
                    cmbCrf.SelectedIndex = -1;
                }

                SelectCombo(cmbVideoCodec, preset.VideoCodec);
                SelectCombo(cmbAudioCodec, preset.AudioCodec);
                SelectCombo(cmbProfile, preset.Profile);
                SelectCombo(cmbLevel, preset.Level);
                SelectCombo(cmbPreset, preset.Preset);
                SelectCombo(cmbTune, preset.Tune);
                SelectCombo(cmbPixFormat, preset.PixFormat);
                SelectCombo(cmbOutputFormat, preset.Format);
                SelectCombo(cmbScale, preset.Scale);
                SelectCombo(cmbFps, preset.Fps);
                SelectCombo(cmbAudioBitrate, preset.AudioBitrate);
                SelectCombo(cmbSubs, preset.Subs);
                txtExtraArgs.Text = preset.ExtraArgs ?? string.Empty;
            }
            finally
            {
                _applyingPreset = false;
            }

            BuildParams();
        }

        private void CapturePreset()
        {
            _currentPreset = new MyPreset();
            if (cmbCrf.SelectedItem is CRFItem crfItem && crfItem.Number != -1)
            {
                _currentPreset.CRF = crfItem.Number;
            }
            _currentPreset.VideoCodec = ComboValue(cmbVideoCodec);
            _currentPreset.AudioCodec = ComboValue(cmbAudioCodec);
            _currentPreset.Profile = ComboValue(cmbProfile);
            _currentPreset.Level = cmbProfile.SelectedIndex > 0 ? ComboValue(cmbLevel) : null;
            _currentPreset.Preset = ComboValue(cmbPreset);
            _currentPreset.Tune = ComboValue(cmbTune);
            _currentPreset.PixFormat = ComboValue(cmbPixFormat);
            _currentPreset.Format = cmbOutputFormat.SelectedItem as string;
            _currentPreset.Scale = ComboValue(cmbScale);
            _currentPreset.Fps = ComboValue(cmbFps);
            _currentPreset.AudioBitrate = ComboValue(cmbAudioBitrate);
            _currentPreset.Subs = ComboValue(cmbSubs);
            var extra = txtExtraArgs.Text.Trim();
            _currentPreset.ExtraArgs = extra.Length > 0 ? extra : null;
        }

        private void btnSavePreset_Click(object sender, EventArgs e)
        {
            var frm = new PresetNameForm();
            if (cmbSavedPresets.SelectedItem is MyPreset myPreset)
            {
                frm.PresetName = myPreset.PresetName;
            }

            if (frm.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            var newName = frm.PresetName.Trim();
            if (string.IsNullOrEmpty(newName))
            {
                MessageBox.Show("Invalid Preset Name");
                return;
            }

            if (savedPresets.ContainsKey(newName) &&
                MessageBox.Show("Preset Name already exists do you want to overwrite it?", "Confirm",
                    MessageBoxButtons.YesNo) != DialogResult.Yes)
            {
                return;
            }

            CapturePreset();
            _currentPreset.PresetName = newName;
            savedPresets[newName] = _currentPreset;
            SavePresets();
            RebindPresets(newName);
        }

        private void btnDeletePreset_Click(object sender, EventArgs e)
        {
            if (!(cmbSavedPresets.SelectedItem is MyPreset myPreset))
            {
                return;
            }

            if (MessageBox.Show($"Delete preset \"{myPreset.PresetName}\"?", "Confirm Delete",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            savedPresets.Remove(myPreset.PresetName);
            SavePresets();
            RebindPresets();
            BuildParams();
        }

        private void cmbSavedPresets_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_rebindingPresets)
            {
                return;
            }

            if (cmbSavedPresets.SelectedItem is MyPreset myPreset)
            {
                // applying a saved preset invalidates the quick preset selection
                MarkQuickPresetDirty();
                ApplyPreset(myPreset);
            }
        }

        private void cmbQuickPreset_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_rebindingPresets || _suppressQuickPresetReset || _applyingPreset)
            {
                return;
            }

            if (cmbQuickPreset.SelectedIndex > 0 && cmbQuickPreset.SelectedItem is string name &&
                BuiltInPresets.All.TryGetValue(name, out var preset))
            {
                ApplyPreset(preset);
            }
        }

        #endregion

        #region Command building

        private void SettingChanged(object sender, EventArgs e)
        {
            // a manual setting change invalidates the selected quick preset
            MarkQuickPresetDirty();
            BuildParams();
        }

        private void MarkQuickPresetDirty()
        {
            if (_applyingPreset || _rebindingPresets || _suppressQuickPresetReset)
            {
                return;
            }

            if (cmbQuickPreset.SelectedIndex > 0)
            {
                _suppressQuickPresetReset = true;
                try
                {
                    cmbQuickPreset.SelectedIndex = 0;
                }
                finally
                {
                    _suppressQuickPresetReset = false;
                }
            }
        }

        private void txtFilePath_TextChanged(object sender, EventArgs e)
        {
            UpdateMediaInfo(txtFilePath.Text);
            BuildParams();
        }

        private void BuildParams()
        {
            CapturePreset();
            UpdateControlStates();

            if (!string.IsNullOrEmpty(txtFilePath.Text) && File.Exists(txtFilePath.Text))
            {
                txtCommandLine.Text = BuildArgsFor(txtFilePath.Text, ComputeOutputPath(txtFilePath.Text));
            }
            else
            {
                txtCommandLine.Text = BuildArgsFor(null, null);
            }
        }

        private string BuildArgsFor(string inputPath, string outputPath)
        {
            var sb = new StringBuilder();
            var videoCodec = ComboValue(cmbVideoCodec);
            var audioCodec = ComboValue(cmbAudioCodec);
            var format = cmbOutputFormat.SelectedItem as string ?? "mkv";
            var isAudioOnly = format == "mp3" || format == "m4a" || format == "wav";
            var isGif = string.Equals(format, "gif", StringComparison.OrdinalIgnoreCase);
            var reEncodingVideo = !isAudioOnly && !isGif && !string.IsNullOrEmpty(videoCodec) && videoCodec != "copy";

            var trimStart = ParseTimeString(txtTrimStart.Text);
            var trimEnd = ParseTimeString(txtTrimEnd.Text);

            if (!string.IsNullOrEmpty(inputPath))
            {
                sb.AppendFormat("-i \"{0}\" ", inputPath);
            }

            // trim: -ss before -i for fast seek; with a start-trim, end becomes a duration (-t)
            if (trimStart.HasValue)
            {
                sb.AppendFormat("-ss {0} ", FormatSeconds(trimStart.Value));
            }
            if (trimEnd.HasValue)
            {
                if (trimStart.HasValue && _totalDurationSeconds.HasValue)
                {
                    // start + end -> fast seek + duration
                    var dur = Math.Max(0.1, trimEnd.Value - trimStart.Value);
                    sb.AppendFormat("-t {0} ", FormatSeconds(dur));
                }
                else
                {
                    // end only -> absolute stop position
                    sb.AppendFormat("-to {0} ", FormatSeconds(trimEnd.Value));
                }
            }

            var mapTarget = isAudioOnly ? "-map 0:a -vn " : "-map 0 -map -0:s? ";
            sb.Append(mapTarget);

            if (isAudioOnly || isGif)
            {
                // no video-specific options for audio-only or GIF output
            }
            else
            {

            if (!string.IsNullOrEmpty(videoCodec))
            {
                sb.AppendFormat("-c:v {0} ", videoCodec);
            }

            if (reEncodingVideo)
            {
                if (cmbCrf.SelectedItem is CRFItem crfItem && crfItem.Number != -1)
                {
                    sb.AppendFormat("-crf {0} ", crfItem.Number);
                }

                if (cmbPreset.SelectedIndex > 0)
                {
                    sb.AppendFormat("-preset {0} ", cmbPreset.SelectedItem);
                }

                if (cmbTune.SelectedIndex > 0)
                {
                    sb.AppendFormat("-tune {0} ", cmbTune.SelectedItem);
                }

                if (cmbPixFormat.SelectedIndex > 0)
                {
                    sb.AppendFormat("-pix_fmt {0} ", cmbPixFormat.SelectedItem);
                }
            }

                if (videoCodec == "libx264" && cmbProfile.SelectedIndex > 0)
                {
                    sb.AppendFormat("-profile:v {0} ", cmbProfile.SelectedItem);
                    if (cmbLevel.SelectedIndex > 0)
                    {
                        sb.AppendFormat("-level:v {0} ", cmbLevel.SelectedItem);
                    }
                }

                if (cmbScale.SelectedIndex > 0)
                {
                    sb.AppendFormat("-vf scale=-2:{0} ", cmbScale.SelectedItem);
                }

                if (cmbFps.SelectedIndex > 0)
                {
                    sb.AppendFormat("-r {0} ", cmbFps.SelectedItem);
                }
            }

            if (isGif)
            {
                // GIF: 10 fps, max 480px wide, optimized palette, no audio, loop forever
                sb.Append("-vf \"fps=10,scale=480:-1:flags=lanczos,split[a][b];[a]palettegen[p];[b][p]paletteuse\" ");
                sb.Append("-an -loop 0 ");
            }

            if (!string.IsNullOrEmpty(audioCodec))
            {
                sb.AppendFormat("-c:a {0} ", audioCodec);
            }

            if (cmbAudioBitrate.SelectedIndex > 0 && !string.IsNullOrEmpty(audioCodec) && audioCodec != "copy")
            {
                sb.AppendFormat("-b:a {0} ", cmbAudioBitrate.SelectedItem);
            }

            var extra = txtExtraArgs.Text.Trim();
            if (extra.Length > 0)
            {
                sb.Append(extra).Append(' ');
            }

            if (!string.IsNullOrEmpty(outputPath))
            {
                sb.AppendFormat("\"{0}\" ", outputPath);
            }

            return sb.ToString().TrimEnd();
        }

        private static double? ParseTimeString(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return null;
            }

            text = text.Trim();

            // accepts "90", "1:30", "01:02:03", "01:02:03.5"
            if (text.Contains(":"))
            {
                double seconds = 0;
                foreach (var part in text.Split(':'))
                {
                    if (!double.TryParse(part, NumberStyles.Float, CultureInfo.InvariantCulture, out var value))
                    {
                        return null;
                    }
                    seconds = seconds * 60 + value;
                }
                return seconds;
            }

            if (double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var plain))
            {
                return plain;
            }

            return null;
        }

        private static string FormatSeconds(double seconds)
        {
            return seconds.ToString("0.###", CultureInfo.InvariantCulture);
        }

        private static string ComboValue(ComboBox cmb)
        {
            return cmb.SelectedIndex > 0 ? cmb.SelectedItem as string : null;
        }

        private string ComputeOutputPath(string inputPath)
        {
            var directory = Path.GetDirectoryName(inputPath) ?? string.Empty;
            var dir = string.IsNullOrEmpty(txtDestFolder.Text)
                ? directory.TrimEnd('\\')
                : txtDestFolder.Text.TrimEnd(new char[] { ' ', '\\' });
            if (string.IsNullOrEmpty(dir))
            {
                dir = ".";
            }

            var fileName = Path.GetFileNameWithoutExtension(inputPath);
            var format = cmbOutputFormat.SelectedItem as string ?? "mkv";

            string outputPath;
            if (cmbOutputName.SelectedIndex == 1)
            {
                // "Keep name only" mode: fixed name, no auto-rename (overwrite mode)
                outputPath = $"{dir}\\{fileName}.{format}";
            }
            else
            {
                outputPath = $"{dir}\\{fileName}_conv.{format}";
                var i = 1;
                while (File.Exists(outputPath))
                {
                    outputPath = $"{dir}\\{fileName}_conv{i}.{format}";
                    i++;
                }
            }

            return outputPath;
        }

        private void UpdateControlStates()
        {
            var videoCodec = ComboValue(cmbVideoCodec);
            var audioCodec = ComboValue(cmbAudioCodec);
            var format = cmbOutputFormat.SelectedItem as string ?? "mkv";
            var isAudioOnly = format == "mp3" || format == "m4a" || format == "wav";
            var isGif = string.Equals(format, "gif", StringComparison.OrdinalIgnoreCase);
            var reEncodingVideo = !isAudioOnly && !isGif && !string.IsNullOrEmpty(videoCodec) && videoCodec != "copy";

            cmbCrf.Enabled = reEncodingVideo;
            cmbPreset.Enabled = reEncodingVideo;
            cmbTune.Enabled = reEncodingVideo;
            cmbPixFormat.Enabled = reEncodingVideo;
            cmbScale.Enabled = !isAudioOnly && !isGif;
            cmbFps.Enabled = !isAudioOnly && !isGif;
            cmbProfile.Enabled = videoCodec == "libx264" && !isAudioOnly && !isGif;
            cmbLevel.Enabled = cmbProfile.Enabled && cmbProfile.SelectedIndex > 0;
            cmbAudioCodec.Enabled = !isGif;
            cmbAudioBitrate.Enabled = !isGif && !string.IsNullOrEmpty(audioCodec) && audioCodec != "copy";
            cmbSubs.Enabled = format == "mkv" || format == "mp4";
            cmbOutputName.Enabled = !isGif;
        }

        #endregion

        #region Queue and file selection

        private void btnSelectFile_Click(object sender, EventArgs e)
        {
            if (ofd.ShowDialog() == DialogResult.OK && ofd.FileNames.Length > 0)
            {
                txtFilePath.Text = ofd.FileNames[0];
                UpdateMediaInfo(txtFilePath.Text);
                BuildParams();
            }
        }

        private void btnAddFiles_Click(object sender, EventArgs e)
        {
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                AddFilesToQueue(ofd.FileNames);
            }
        }

        private void btnSelectDestFolder_Click(object sender, EventArgs e)
        {
            if (oFolderDialog.ShowDialog() == DialogResult.OK)
            {
                txtDestFolder.Text = oFolderDialog.SelectedPath;
                BuildParams();
            }
        }

        private void MainForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void MainForm_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetData(DataFormats.FileDrop) is string[] files)
            {
                AddFilesToQueue(files);
            }
        }

        private void AddFilesToQueue(IEnumerable<string> files)
        {
            var added = 0;
            var skipped = 0;
            foreach (var file in files)
            {
                if (!File.Exists(file) || !MediaExtensions.Contains(Path.GetExtension(file)))
                {
                    skipped++;
                    continue;
                }

                AddToQueue(file);
                added++;
            }

            if (added > 0 && (string.IsNullOrEmpty(txtFilePath.Text) || !File.Exists(txtFilePath.Text)))
            {
                txtFilePath.Text = _queue[0].InputPath;
                UpdateMediaInfo(txtFilePath.Text);
            }

            lblProgress.Text = added > 0
                ? $"Added {added} file(s) to the queue" + (skipped > 0 ? $" ({skipped} skipped)" : "") + "."
                : "No supported media files found.";
        }

        private void AddToQueue(string path)
        {
            var item = new QueueItem { InputPath = path, Status = QueueStatus.Pending };
            _queue.Add(item);
            var lvi = new ListViewItem(Path.GetFileName(path)) { Tag = item };
            lvi.SubItems.Add("Pending");
            lstQueue.Items.Add(lvi);
        }

        private void RefreshQueueRow(QueueItem item)
        {
            foreach (ListViewItem lvi in lstQueue.Items)
            {
                if (!ReferenceEquals(lvi.Tag, item))
                {
                    continue;
                }

                string status;
                Color color;
                switch (item.Status)
                {
                    case QueueStatus.Converting: status = "Converting..."; color = Color.DarkOrange; break;
                    case QueueStatus.Done: status = "Done"; color = Color.ForestGreen; break;
                    case QueueStatus.Failed: status = "Failed"; color = Color.Firebrick; break;
                    case QueueStatus.Canceled: status = "Canceled"; color = Color.DimGray; break;
                    default: status = "Pending"; color = SystemColors.ControlText; break;
                }

                lvi.SubItems[1].Text = status;
                lvi.ForeColor = color;
            }
        }

        private void btnClearQueue_Click(object sender, EventArgs e)
        {
            if (converting)
            {
                return;
            }

            _queue.Clear();
            lstQueue.Items.Clear();
        }

        private void btnRemoveSelected_Click(object sender, EventArgs e)
        {
            if (converting)
            {
                return;
            }

            foreach (ListViewItem lvi in lstQueue.SelectedItems)
            {
                if (lvi.Tag is QueueItem item)
                {
                    _queue.Remove(item);
                }
                lstQueue.Items.Remove(lvi);
            }
        }

        private void UpdateMediaInfo(string path)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path) || string.IsNullOrEmpty(_ffprobePath))
            {
                lblMediaInfo.Text = "Media info appears here when a file is selected.";
                return;
            }

            var info = FfmpegTools.GetMediaInfo(_ffprobePath, path);
            if (info == null)
            {
                lblMediaInfo.Text = "Media info unavailable for this file.";
                return;
            }

            var parts = new List<string>();
            if (info.Width.HasValue && info.Height.HasValue)
            {
                parts.Add($"{info.Width}x{info.Height}");
            }
            if (!string.IsNullOrEmpty(info.VideoCodec))
            {
                parts.Add("video: " + info.VideoCodec);
            }
            if (!string.IsNullOrEmpty(info.AudioCodec))
            {
                parts.Add("audio: " + info.AudioCodec);
            }
            if (info.DurationSeconds.HasValue)
            {
                parts.Add(TimeSpan.FromSeconds(info.DurationSeconds.Value).ToString(@"hh\:mm\:ss"));
            }
            if (!string.IsNullOrEmpty(info.Bitrate))
            {
                parts.Add(info.Bitrate);
            }

            lblMediaInfo.Text = string.Join("   |   ", parts);
        }

        #endregion

        #region Conversion

        private async void btnConvert_Click(object sender, EventArgs e)
        {
            if (converting)
            {
                RequestStop();
                return;
            }

            if (string.IsNullOrEmpty(_ffmpegPath))
            {
                MessageBox.Show(
                    "ffmpeg.exe was not found.\n\nPut ffmpeg.exe (and ffprobe.exe) next to AliHaFFMPEG.exe, or add them to your PATH.",
                    "ffmpeg not found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var items = CollectItemsToConvert();
            if (items == null || items.Count == 0)
            {
                return;
            }

            if (chkShutdown.Checked &&
                MessageBox.Show(
                    $"The PC will SHUT DOWN after converting {items.Count} file(s).\n\nContinue?",
                    "Confirm shutdown", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
            {
                return;
            }

            btnConvert.Text = "Stop";
            converting = true;
            progressBar1.Value = 0;
            lblProgress.Visible = true;

            try
            {
                var failed = 0;
                var done = 0;

                for (var i = 0; i < items.Count; i++)
                {
                    var item = items[i];
                    item.Status = QueueStatus.Converting;
                    RefreshQueueRow(item);
                    lblProgress.Text = items.Count > 1
                        ? $"File {i + 1}/{items.Count}: {Path.GetFileName(item.InputPath)}"
                        : "Converting...";

                    if (!item.TotalSeconds.HasValue)
                    {
                        item.TotalSeconds = FfmpegTools.GetDurationSeconds(_ffprobePath, item.InputPath);
                    }
                    _totalDurationSeconds = item.TotalSeconds;

                    item.OutputPath = ComputeOutputPath(item.InputPath);
                    var args = BuildArgsFor(item.InputPath, item.OutputPath);
                    txtCommandLine.Text = args;

                    var exitCode = await RunFfmpegAsync(args);

                    if (canceled)
                    {
                        item.Status = QueueStatus.Canceled;
                    }
                    else if (exitCode != 0)
                    {
                        item.Status = QueueStatus.Failed;
                        failed++;
                        string tail;
                        lock (_errorTailLock) tail = _errorTail.ToString();
                        MessageBox.Show($"ffmpeg exited with code {exitCode}.\n\n{tail.Trim()}",
                            "Conversion failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        item.Status = QueueStatus.Done;
                        done++;
                    }

                    RefreshQueueRow(item);
                    WriteLogFile(item, exitCode);

                    if (canceled && i < items.Count - 1)
                    {
                        var remaining = items.Count - i - 1;
                        if (MessageBox.Show(
                                $"Conversion stopped. Convert the remaining {remaining} file(s)?",
                                "Stopped", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                        {
                            for (var j = i + 1; j < items.Count; j++)
                            {
                                items[j].Status = QueueStatus.Canceled;
                                RefreshQueueRow(items[j]);
                            }
                            break;
                        }

                        canceled = false;
                    }
                }

                lblProgress.Text = $"{done}/{items.Count} converted" + (failed > 0 ? $", {failed} failed" : "") + ".";
                if (failed == 0 && done == items.Count && _totalDurationSeconds.HasValue)
                {
                    progressBar1.Value = 100;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unexpected error during conversion: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                converting = false;
                btnConvert.Text = "Convert";
            }

            if (chkNotify.Checked)
            {
                MessageBox.Show("All conversions finished.", "AliHaFFMPEG",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            if (chkShutdown.Checked && !canceled)
            {
                try
                {
                    Process.Start(new ProcessStartInfo("shutdown", "/s /t 60") { CreateNoWindow = true });
                    MessageBox.Show("The PC will shut down in 60 seconds.\n\nRun 'shutdown /a' to abort.",
                        "Shutting down", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                catch
                {
                }
            }
        }

        private List<QueueItem> CollectItemsToConvert()
        {
            var pending = _queue.Where(x => x.Status == QueueStatus.Pending).ToList();
            if (pending.Count == 0 && !string.IsNullOrEmpty(txtFilePath.Text) && File.Exists(txtFilePath.Text))
            {
                // single-file flow: put the selected file into the queue so progress is visible
                AddToQueue(txtFilePath.Text);
                pending = _queue.Where(x => x.Status == QueueStatus.Pending).ToList();
            }

            if (pending.Count == 0)
            {
                MessageBox.Show("Add media files to the queue (or select one) before converting.",
                    "Nothing to convert", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }

            return pending;
        }

        private async Task<int> RunFfmpegAsync(string args)
        {
            lock (_errorTailLock) _errorTail.Clear();
            lock (_progressLock) { _lastOutputSeconds = 0; _lastSpeed = 0; }
            canceled = false;
            progressBar1.Value = 0;
            _lastRunArgs = args;

            var completion = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = _ffmpegPath,
                    // machine-readable progress on stdout; kept out of the user-visible command
                    Arguments = args + " -progress pipe:1 -nostats",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    RedirectStandardInput = true
                },
                EnableRaisingEvents = true
            };

            proc.OutputDataReceived += Process_ProgressDataReceived;
            proc.ErrorDataReceived += Process_LogDataReceived;
            proc.Exited += (s, ea) =>
            {
                int exitCode;
                try { exitCode = proc.ExitCode; }
                catch { exitCode = -1; }
                completion.TrySetResult(exitCode);
            };

            process = proc;
            try
            {
                proc.Start();
            }
            catch (Exception ex)
            {
                process = null;
                MessageBox.Show("Failed to start ffmpeg: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }

            try { proc.PriorityClass = ProcessPriorityClass.BelowNormal; } catch { }

            proc.BeginOutputReadLine();
            proc.BeginErrorReadLine();

            var exit = await completion.Task;
            process = null;
            return exit;
        }

        private void RequestStop()
        {
            var proc = process;
            canceled = true;
            if (proc == null || proc.HasExited)
            {
                return;
            }

            // graceful stop: ffmpeg finalizes the output file on "q" so partial files stay playable
            try { proc.StandardInput.WriteLine("q"); } catch { }

            Task.Run(() =>
            {
                try
                {
                    if (!proc.WaitForExit(5000))
                    {
                        try { proc.Kill(true); } catch { }
                    }
                }
                catch
                {
                }
            });
        }

        private void Process_ProgressDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Data))
            {
                return;
            }

            var eq = e.Data.IndexOf('=');
            if (eq <= 0)
            {
                return;
            }

            var key = e.Data.Substring(0, eq).Trim();
            var value = e.Data.Substring(eq + 1).Trim();

            double outputSeconds = -1, speed = 0;
            var updated = false;

            if (key == "out_time_us" || key == "out_time_ms")
            {
                // ffmpeg reports microseconds here (out_time_ms is a legacy alias with the same unit)
                if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var micro) && micro > 0)
                {
                    outputSeconds = micro / 1000000.0;
                    updated = true;
                }
            }
            else if (key == "out_time")
            {
                if (TimeSpan.TryParse(value, CultureInfo.InvariantCulture, out var t))
                {
                    outputSeconds = t.TotalSeconds;
                    updated = true;
                }
            }
            else if (key == "speed")
            {
                var numeric = value.EndsWith("x") ? value.Substring(0, value.Length - 1) : value;
                if (double.TryParse(numeric, NumberStyles.Float, CultureInfo.InvariantCulture, out var sp) && sp > 0)
                {
                    speed = sp;
                    updated = true;
                }
            }

            if (!updated)
            {
                return;
            }

            lock (_progressLock)
            {
                if (outputSeconds >= 0)
                {
                    _lastOutputSeconds = outputSeconds;
                }

                if (speed > 0)
                {
                    _lastSpeed = speed;
                }
            }

            InvokeAction(UpdateProgressUi);
        }

        private void Process_LogDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Data))
            {
                return;
            }

            lock (_errorTailLock)
            {
                _errorTail.AppendLine(e.Data);
                if (_errorTail.Length > 64000)
                {
                    _errorTail.Remove(0, _errorTail.Length - 32000);
                }
            }

            // throttle live log UI updates
            if ((DateTime.UtcNow - _lastLogUiUpdate).TotalMilliseconds < 200)
            {
                return;
            }
            _lastLogUiUpdate = DateTime.UtcNow;
            InvokeAction(UpdateLogUi);
        }

        private void UpdateLogUi()
        {
            if (txtLog.IsDisposed)
            {
                return;
            }

            string tail;
            lock (_errorTailLock) tail = _errorTail.ToString();
            txtLog.Text = tail;
        }

        private void UpdateProgressUi()
        {
            double outSec, speed;
            double? total;
            lock (_progressLock)
            {
                outSec = _lastOutputSeconds;
                speed = _lastSpeed;
                total = _totalDurationSeconds;
            }

            var timeText = TimeSpan.FromSeconds(Math.Max(0, outSec)).ToString(@"hh\:mm\:ss");
            if (total.HasValue && total.Value > 0)
            {
                var totalText = TimeSpan.FromSeconds(total.Value).ToString(@"hh\:mm\:ss");
                var percent = Math.Min(100.0, 100.0 * outSec / total.Value);
                progressBar1.Value = (int)Math.Min(100, Math.Max(0, Math.Round(percent)));
                var etaText = speed > 0.01
                    ? TimeSpan.FromSeconds(Math.Max(0, (total.Value - outSec) / speed)).ToString(@"hh\:mm\:ss")
                    : "--:--:--";
                lblProgress.Text =
                    $"{percent:0}%  ({timeText} / {totalText})   speed: {speed:0.0#}x   ETA: {etaText}";
            }
            else
            {
                lblProgress.Text = $"time= {timeText}" + (speed > 0 ? $"   speed: {speed:0.0#}x" : "");
            }
        }

        private void WriteLogFile(QueueItem item, int exitCode)
        {
            try
            {
                var logDir = Path.Combine(PresetsDir, "logs");
                Directory.CreateDirectory(logDir);
                var stamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss", CultureInfo.InvariantCulture);
                var safeName = string.Join("_",
                    Path.GetFileNameWithoutExtension(item.InputPath).Split(Path.GetInvalidFileNameChars()));
                var logPath = Path.Combine(logDir, $"{stamp}_{safeName}.log");

                string tail;
                lock (_errorTailLock) tail = _errorTail.ToString();

                var sb = new StringBuilder();
                sb.AppendLine("Input:    " + item.InputPath);
                sb.AppendLine("Output:   " + item.OutputPath);
                sb.AppendLine("Command:  ffmpeg " + (_lastRunArgs ?? "<see Command tab>"));
                sb.AppendLine("ExitCode: " + exitCode);
                sb.AppendLine("Finished: " + DateTime.Now);
                sb.AppendLine();
                sb.AppendLine(tail);

                File.WriteAllText(logPath, sb.ToString());
                _lastLogFile = logPath;
            }
            catch
            {
                // logging must never break conversions
            }
        }

        private void btnCopyCommand_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtCommandLine.Text))
            {
                Clipboard.SetText(txtCommandLine.Text);
                lblProgress.Text = "Command copied to clipboard.";
            }
        }

        private void btnOpenLastLog_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_lastLogFile) && File.Exists(_lastLogFile))
            {
                try
                {
                    Process.Start(new ProcessStartInfo(_lastLogFile) { UseShellExecute = true });
                }
                catch
                {
                }
            }
            else
            {
                MessageBox.Show("No log file written yet. Logs are saved after each conversion job.",
                    "Log", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        #endregion

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (converting)
            {
                if (MessageBox.Show("We are converting. Do you want to stop the conversion and exit?",
                        "Confirm Exit", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    RequestStop();
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }

        private void InvokeAction(Action act)
        {
            if (IsDisposed || Disposing)
            {
                return;
            }

            if (this.InvokeRequired)
            {
                try
                {
                    this.Invoke((MethodInvoker)delegate { act(); });
                }
                catch (ObjectDisposedException)
                {
                    // form closed while the conversion was still reporting progress
                }
            }
            else
            {
                act();
            }
        }
    }

    public class CRFItem
    {
        public int Number { get; set; }
        public string Title { get; set; }
    }

    public class MyPreset
    {
        public string PresetName { get; set; }
        public override string ToString()
        {
            return PresetName;
        }

        public int? CRF { get; set; }
        public string VideoCodec { get; set; }
        public string AudioCodec { get; set; }
        public string Profile { get; set; }
        public string Level { get; set; }
        public string Preset { get; set; }
        public string Tune { get; set; }
        public string Format { get; set; }
        public string PixFormat { get; set; }
        public string Scale { get; set; }
        public string Fps { get; set; }
        public string AudioBitrate { get; set; }
        public string Subs { get; set; }
        public string ExtraArgs { get; set; }
    }
}
