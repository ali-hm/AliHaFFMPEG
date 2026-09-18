using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
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
                    case 51:
                        title += " (Worst Quality)";
                        break;
                    case 25:
                        title += " (OK for most cases)";
                        break;
                    case 23:
                        title += " (Default)";
                        break;
                    case 0:
                        title += " (Lossless and Large Output)";
                        break;
                    case -1:
                        title = string.Empty;
                        break;
                }
                _crfItems.Add(new CRFItem() { Number = i, Title = title });
            }

            cmbCrf.DataSource = _crfItems;
            cmbCrf.SelectedIndex = 26;

            cmbVideoCodec.SelectedIndex = 0;
            cmbOutputFormat.SelectedIndex = 0;
            if (File.Exists("presets.settings"))
            {
                savedPresets =
                    JsonConvert.DeserializeObject<Dictionary<string, MyPreset>>(File.ReadAllText("presets.settings"));
                cmbSavedPresets.DataSource = savedPresets.Select(x => x.Value).ToList();
                cmbSavedPresets_SelectedIndexChanged(sender, e);
            }

        }

        private void cmbCrf_SelectedIndexChanged(object sender, EventArgs e)
        {
            BuildParams();
        }

        private void BuildParams()
        {
            var sb = new StringBuilder();
            _currentPreset = new MyPreset();
            if (!string.IsNullOrEmpty(txtFilePath.Text))
            {
                sb.AppendFormat("-i \"{0}\" ", txtFilePath.Text);
            }
            if (cmbCrf.SelectedItem != null && cmbCrf.SelectedItem is CRFItem crfItem && crfItem.Number != -1)
            {
                sb.AppendFormat("-crf {0} ", crfItem.Number);
                _currentPreset.CRF = crfItem.Number;
            }

            if (cmbVideoCodec.SelectedIndex > 0)
            {
                sb.AppendFormat("-c:v {0} ", cmbVideoCodec.Items[cmbVideoCodec.SelectedIndex]);
                _currentPreset.VideoCodec = (string)cmbVideoCodec.Items[cmbVideoCodec.SelectedIndex];
            }
            if (cmbAudioCodec.SelectedIndex > 0)
            {
                sb.AppendFormat("-c:a {0} ", cmbAudioCodec.Items[cmbAudioCodec.SelectedIndex]);
                _currentPreset.AudioCodec = (string)cmbAudioCodec.Items[cmbAudioCodec.SelectedIndex];
            }

            if (cmbProfile.SelectedIndex > 0)
            {
                sb.AppendFormat("-profile:v {0} ", cmbProfile.Items[cmbProfile.SelectedIndex]);
                _currentPreset.Profile = (string)cmbProfile.Items[cmbProfile.SelectedIndex];
                if (cmbLevel.SelectedIndex > 0)
                {
                    sb.AppendFormat("-level:v {0} ", cmbLevel.Items[cmbLevel.SelectedIndex]);
                    _currentPreset.Level = (string)cmbLevel.Items[cmbLevel.SelectedIndex];
                }
            }
            if (cmbPreset.SelectedIndex > 0)
            {
                sb.AppendFormat("-preset {0} ", cmbPreset.Items[cmbPreset.SelectedIndex]);
                _currentPreset.Preset = (string)cmbPreset.Items[cmbPreset.SelectedIndex];
            }

            if (cmbTune.SelectedIndex > 0)
            {
                sb.AppendFormat("-tune {0} ", cmbTune.Items[cmbTune.SelectedIndex]);
                _currentPreset.Tune = (string)cmbTune.Items[cmbTune.SelectedIndex];
            }

            if (cmbPixFormat.SelectedIndex > 0)
            {
                sb.AppendFormat("-pix_fmt {0} ", cmbPixFormat.Items[cmbPixFormat.SelectedIndex]);
                _currentPreset.PixFormat = (string)cmbPixFormat.Items[cmbPixFormat.SelectedIndex];
            }
            var outPutformat = cmbOutputFormat.SelectedIndex >= 0
                ? (string)cmbOutputFormat.Items[cmbOutputFormat.SelectedIndex]
                : "mkv";
            _currentPreset.Format = outPutformat;
            sb.Append("-map 0 ");
            if (!string.IsNullOrEmpty(txtFilePath.Text))
            {
                var dir = string.IsNullOrEmpty(txtDestFolder.Text)
                    ? Path.GetDirectoryName(txtFilePath.Text).TrimEnd('\\')
                    : txtDestFolder.Text.TrimEnd(new char[] { ' ', '\\' });
                var fileName = Path.GetFileNameWithoutExtension(txtFilePath.Text);
                var outputPath = $"{dir}\\{fileName}_conv.{outPutformat}";
                var i = 1;
                while (File.Exists(outputPath))
                {
                    outputPath = $"{dir}\\{fileName}_conv{i}.{outPutformat}";
                    i++;
                }

                sb.AppendFormat("\"{0}\" ", outputPath);
            }

            txtCommandLine.Text = sb.ToString();
        }

        private void btnSelectFile_Click(object sender, EventArgs e)
        {
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                txtFilePath.Text = ofd.FileName;
                BuildParams();
            }
        }

        private static Process process = null;
        private static bool canceled = false;
        private static string currentOutput;
        private bool converting = false;
        private Dictionary<string, string> progressData = new Dictionary<string, string>();
        private void btnConvert_Click(object sender, EventArgs e)
        {
            if (converting)
            {
                StopConversion();
                return;
            }
            process = new Process
            {
                StartInfo = { RedirectStandardOutput = true, RedirectStandardError = true, FileName = @"ffmpeg.exe" }
            };
            process.StartInfo.Arguments = txtCommandLine.Text;


            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            process.EnableRaisingEvents = true;
            process.OutputDataReceived += Process_OutputDataReceived;
            process.ErrorDataReceived += Process_ErrorDataReceived;
            process.Exited += Process_Exited;
            process.Start();
            try
            {
                process.PriorityClass = ProcessPriorityClass.BelowNormal;
            }
            catch (Exception ex)
            {

            }
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            btnConvert.Text = "Stop";
            converting = true;
            lblProgress.Visible = true;
        }

        private void StopConversion()
        {
            if (process != null && !process.HasExited)
            {
                process.Kill();
            }

            process = null;
            btnConvert.Text = "Convert";
            converting = false;
            lblProgress.Visible = false;
        }

        private void Process_Exited(object? sender, EventArgs e)
        {

            InvokeAction(() =>
            {
                btnConvert.Text = "Convert";
                lblProgress.Visible = false;

            });
            //
            converting = false;
        }

        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null && e.Data.StartsWith("frame=", StringComparison.InvariantCultureIgnoreCase))
            {
                var data = new List<string>(e.Data.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
                var index = data.FindIndex(x => x.StartsWith("frame=", StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    if (data[index].Length > 6)
                    {
                        var subData = data[index].Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                        if (subData.Length > 1)
                        {
                            progressData["frame"] = subData[1].Trim();
                        }
                    }
                    else if (index + 1 < data.Count)
                    {
                        progressData["frame"] = data[index + 1];
                    }
                }

                index = data.FindIndex(x => x.StartsWith("time=", StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    if (data[index].Length > 5)
                    {
                        var subData = data[index].Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                        if (subData.Length > 1)
                        {
                            progressData["time"] = subData[1].Trim();
                        }
                    }
                    else if (index + 1 < data.Count)
                    {
                        progressData["time"] = data[index + 1];
                    }
                }

                if (progressData.ContainsKey("time"))
                {
                    InvokeAction(() =>
                    {
                        lblProgress.Text = $"time= {progressData["time"]}";
                    });

                }
                /*Console.SetCursorPosition(0,Console.CursorTop!=0? Console.CursorTop-1:0);
                Console.WriteLine(e.Data);*/

            }

        }
        private void InvokeAction(Action act)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate { act(); });
            }
            else
            {
                act();
            }
        }
        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            /*if(!string.IsNullOrEmpty(e.Data))
                Console.WriteLine(e.Data);*/
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (converting)
            {
                if (MessageBox.Show("We Are Converting do you want to stop conversion and exit?", "Confirm Exit",
                    MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    StopConversion();

                }
                else
                {
                    e.Cancel = true;
                }
            }
        }

        private void btnSavePreset_Click(object sender, EventArgs e)
        {
            var frm = new PresetNameForm();
            if (cmbSavedPresets.SelectedItem is MyPreset myPreset)
            {
                frm.PresetName = myPreset.PresetName;
            }

            if (frm.ShowDialog() == DialogResult.OK)
            {
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

                _currentPreset.PresetName = newName;
                savedPresets[newName] = _currentPreset;
                File.WriteAllText("presets.settings", JsonConvert.SerializeObject(savedPresets));
                cmbSavedPresets.DataSource = savedPresets.Select(x => x.Value).ToList();
            }
        }

        private void cmbSavedPresets_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbSavedPresets.SelectedItem is MyPreset myPreset)
            {
                if (myPreset.CRF.HasValue)
                {
                    var index = _crfItems.FindIndex(x => x.Number == myPreset.CRF);
                    cmbCrf.SelectedIndex = index;
                }
                else
                {
                    cmbCrf.SelectedIndex = -1;
                }

                if (!string.IsNullOrEmpty(myPreset.VideoCodec))
                {
                    cmbVideoCodec.SelectedIndex = cmbVideoCodec.FindString(myPreset.VideoCodec);
                }
                else
                {
                    cmbVideoCodec.SelectedIndex = -1;
                }

                if (!string.IsNullOrEmpty(myPreset.AudioCodec))
                {
                    cmbAudioCodec.SelectedIndex = cmbAudioCodec.FindString(myPreset.AudioCodec);
                }
                else
                {
                    cmbAudioCodec.SelectedIndex = -1;
                }

                if (!string.IsNullOrEmpty(myPreset.Profile))
                {
                    cmbProfile.SelectedIndex = cmbProfile.FindString(myPreset.Profile);
                }
                else
                {
                    cmbProfile.SelectedIndex = -1;
                }

                if (!string.IsNullOrEmpty(myPreset.Level))
                {
                    cmbLevel.SelectedIndex = cmbLevel.FindString(myPreset.Level);
                }
                else
                {
                    cmbLevel.SelectedIndex = -1;
                }

                if (!string.IsNullOrEmpty(myPreset.Preset))
                {
                    cmbPreset.SelectedIndex = cmbPreset.FindString(myPreset.Preset);
                }
                else
                {
                    cmbPreset.SelectedIndex = -1;
                }

                if (!string.IsNullOrEmpty(myPreset.Tune))
                {
                    cmbTune.SelectedIndex = cmbTune.FindString(myPreset.Tune);
                }
                else
                {
                    cmbTune.SelectedIndex = -1;
                }

                if (!string.IsNullOrEmpty(myPreset.PixFormat))
                {
                    cmbPixFormat.SelectedIndex = cmbPixFormat.FindString(myPreset.PixFormat);
                }
                else
                {
                    cmbPixFormat.SelectedIndex = -1;
                }

                if (!string.IsNullOrEmpty(myPreset.Format))
                {
                    cmbOutputFormat.SelectedIndex = cmbOutputFormat.FindString(myPreset.Format);
                }
                else
                {
                    cmbOutputFormat.SelectedIndex = -1;
                }
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
    }

    public class CRFItem
    {
        public int Number { get; set; }
        public string Title { get; set; }
    }

    /*public class ProfileAndLevel
    {
        public string Profile { get; set; }
        public string Level { get; set; }
        public override string ToString()
        {
            return $"{Profile} {Level}";
        }
    }*/
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
    }
}