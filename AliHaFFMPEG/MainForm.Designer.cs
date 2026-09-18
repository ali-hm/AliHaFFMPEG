using AliHaFFMPEG.Core;

namespace AliHaFFMPEG
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            label5 = new System.Windows.Forms.Label();
            label6 = new System.Windows.Forms.Label();
            label7 = new System.Windows.Forms.Label();
            label8 = new System.Windows.Forms.Label();
            label9 = new System.Windows.Forms.Label();
            label10 = new System.Windows.Forms.Label();
            label11 = new System.Windows.Forms.Label();
            label12 = new System.Windows.Forms.Label();
            label13 = new System.Windows.Forms.Label();
            label14 = new System.Windows.Forms.Label();
            label15 = new System.Windows.Forms.Label();
            label16 = new System.Windows.Forms.Label();
            label17 = new System.Windows.Forms.Label();
            label18 = new System.Windows.Forms.Label();
            label19 = new System.Windows.Forms.Label();
            label20 = new System.Windows.Forms.Label();
            label21 = new System.Windows.Forms.Label();
            label22 = new System.Windows.Forms.Label();
            label23 = new System.Windows.Forms.Label();
            labelQueue = new System.Windows.Forms.Label();
            lblProgress = new System.Windows.Forms.Label();
            lblMediaInfo = new System.Windows.Forms.Label();
            lblHwHint = new System.Windows.Forms.Label();
            cmbCrf = new System.Windows.Forms.ComboBox();
            cmbVideoCodec = new System.Windows.Forms.ComboBox();
            cmbAudioCodec = new System.Windows.Forms.ComboBox();
            cmbProfile = new System.Windows.Forms.ComboBox();
            cmbPreset = new System.Windows.Forms.ComboBox();
            cmbTune = new System.Windows.Forms.ComboBox();
            cmbPixFormat = new System.Windows.Forms.ComboBox();
            cmbOutputFormat = new System.Windows.Forms.ComboBox();
            cmbLevel = new System.Windows.Forms.ComboBox();
            cmbSavedPresets = new System.Windows.Forms.ComboBox();
            cmbQuickPreset = new System.Windows.Forms.ComboBox();
            cmbScale = new System.Windows.Forms.ComboBox();
            cmbFps = new System.Windows.Forms.ComboBox();
            cmbAudioBitrate = new System.Windows.Forms.ComboBox();
            cmbSubs = new System.Windows.Forms.ComboBox();
            cmbOutputName = new System.Windows.Forms.ComboBox();
            txtFilePath = new System.Windows.Forms.TextBox();
            txtDestFolder = new System.Windows.Forms.TextBox();
            txtTrimStart = new System.Windows.Forms.TextBox();
            txtTrimEnd = new System.Windows.Forms.TextBox();
            txtExtraArgs = new System.Windows.Forms.TextBox();
            txtCommandLine = new System.Windows.Forms.RichTextBox();
            txtLog = new System.Windows.Forms.RichTextBox();
            btnSelectFile = new System.Windows.Forms.Button();
            btnAddFiles = new System.Windows.Forms.Button();
            btnSelectDestFolder = new System.Windows.Forms.Button();
            btnConvert = new System.Windows.Forms.Button();
            btnSavePreset = new System.Windows.Forms.Button();
            btnDeletePreset = new System.Windows.Forms.Button();
            btnClearQueue = new System.Windows.Forms.Button();
            btnRemoveSelected = new System.Windows.Forms.Button();
            btnCopyCommand = new System.Windows.Forms.Button();
            btnOpenLastLog = new System.Windows.Forms.Button();
            lstQueue = new System.Windows.Forms.ListView();
            colFile = new System.Windows.Forms.ColumnHeader();
            colStatus = new System.Windows.Forms.ColumnHeader();
            chkShutdown = new System.Windows.Forms.CheckBox();
            chkNotify = new System.Windows.Forms.CheckBox();
            progressBar1 = new System.Windows.Forms.ProgressBar();
            tabs = new System.Windows.Forms.TabControl();
            tabFiles = new System.Windows.Forms.TabPage();
            tabSettings = new System.Windows.Forms.TabPage();
            tabCommand = new System.Windows.Forms.TabPage();
            ofd = new System.Windows.Forms.OpenFileDialog();
            oFolderDialog = new System.Windows.Forms.FolderBrowserDialog();
            label24 = new System.Windows.Forms.Label();
            label25 = new System.Windows.Forms.Label();
            label26 = new System.Windows.Forms.Label();
            cmbQualityMode = new System.Windows.Forms.ComboBox();
            txtVideoBitrate = new System.Windows.Forms.TextBox();
            txtTargetSizeMB = new System.Windows.Forms.TextBox();
            btnRetryFailed = new System.Windows.Forms.Button();
            btnOpenOutput = new System.Windows.Forms.Button();
            btnBrowsePresets = new System.Windows.Forms.Button();
            lblPresetInfo = new System.Windows.Forms.Label();
            btnAbout = new System.Windows.Forms.Button();
            lblCodecInfo = new System.Windows.Forms.Label();
            lblWarnings = new System.Windows.Forms.Label();
            tabs.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(16, 96);
            label1.Name = "label1";
            label1.Text = "CRF / Quality";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(16, 64);
            label2.Name = "label2";
            label2.Text = "Video Codec";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(330, 160);
            label3.Name = "label3";
            label3.Text = "Audio Codec";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(16, 224);
            label4.Name = "label4";
            label4.Text = "Profile";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(16, 128);
            label5.Name = "label5";
            label5.Text = "Encoder Preset";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new System.Drawing.Point(16, 160);
            label6.Name = "label6";
            label6.Text = "Tune";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new System.Drawing.Point(16, 192);
            label7.Name = "label7";
            label7.Text = "Pixel Format";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new System.Drawing.Point(330, 64);
            label8.Name = "label8";
            label8.Text = "Output Format";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new System.Drawing.Point(16, 26);
            label9.Name = "label9";
            label9.Text = "Select a File";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new System.Drawing.Point(206, 228);
            label10.Name = "label10";
            label10.Text = "Level";
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new System.Drawing.Point(546, 26);
            label11.Name = "label11";
            label11.Text = "Saved";
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Location = new System.Drawing.Point(30, 56);
            label12.Name = "label12";
            label12.Text = "Dest Folder";
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Location = new System.Drawing.Point(16, 26);
            label13.Name = "label13";
            label13.Text = "Quick Preset";
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Location = new System.Drawing.Point(330, 96);
            label14.Name = "label14";
            label14.Text = "Scale";
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.Location = new System.Drawing.Point(330, 128);
            label15.Name = "label15";
            label15.Text = "Frame Rate";
            // 
            // label16
            // 
            label16.AutoSize = true;
            label16.Location = new System.Drawing.Point(330, 192);
            label16.Name = "label16";
            label16.Text = "Audio Bitrate";
            // 
            // label17
            // 
            label17.AutoSize = true;
            label17.Location = new System.Drawing.Point(330, 224);
            label17.Name = "label17";
            label17.Text = "Subtitles";
            // 
            // label18
            // 
            label18.AutoSize = true;
            label18.Location = new System.Drawing.Point(630, 64);
            label18.Name = "label18";
            label18.Text = "Trim Start";
            // 
            // label19
            // 
            label19.AutoSize = true;
            label19.Location = new System.Drawing.Point(630, 96);
            label19.Name = "label19";
            label19.Text = "Trim End";
            // 
            // label20
            // 
            label20.AutoSize = true;
            label20.Location = new System.Drawing.Point(630, 128);
            label20.Name = "label20";
            label20.Text = "Output Name";
            // 
            // label21
            // 
            label21.AutoSize = true;
            label21.Location = new System.Drawing.Point(630, 160);
            label21.Name = "label21";
            label21.Text = "Extra Args";
            // 
            // label22
            // 
            label22.AutoSize = true;
            label22.Location = new System.Drawing.Point(16, 194);
            label22.Name = "label22";
            label22.Text = "Live Log (each job is also saved to %APPDATA%\\AliHaFFMPEG\\logs)";
            // 
            // label23
            // 
            label23.AutoSize = true;
            label23.Location = new System.Drawing.Point(16, 58);
            label23.Name = "label23";
            label23.Text = "Command (what will actually run)";
            // 
            // labelQueue
            // 
            labelQueue.AutoSize = true;
            labelQueue.Location = new System.Drawing.Point(16, 86);
            labelQueue.Name = "labelQueue";
            labelQueue.Text = "Queue (drop media files here, or use Add Files)";
            // 
            // lblProgress
            // 
            lblProgress.AutoSize = true;
            lblProgress.Location = new System.Drawing.Point(16, 402);
            lblProgress.Name = "lblProgress";
            lblProgress.Text = "";
            // 
            // lblMediaInfo
            // 
            lblMediaInfo.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            lblMediaInfo.Location = new System.Drawing.Point(16, 310);
            lblMediaInfo.Name = "lblMediaInfo";
            lblMediaInfo.Size = new System.Drawing.Size(932, 32);
            lblMediaInfo.Text = "Media info appears here when a file is selected.";
            // 
            // lblHwHint
            // 
            lblHwHint.AutoSize = true;
            lblHwHint.ForeColor = System.Drawing.Color.DimGray;
            lblHwHint.Location = new System.Drawing.Point(16, 318);
            lblHwHint.Name = "lblHwHint";
            lblHwHint.Text = "";
            // 
            // lblCodecInfo
            // 
            lblCodecInfo.AutoSize = true;
            lblCodecInfo.ForeColor = System.Drawing.Color.DimGray;
            lblCodecInfo.Location = new System.Drawing.Point(16, 344);
            lblCodecInfo.Name = "lblCodecInfo";
            lblCodecInfo.Text = "";
            // 
            // lblWarnings
            // 
            lblWarnings.AutoSize = true;
            lblWarnings.ForeColor = System.Drawing.Color.Firebrick;
            lblWarnings.Location = new System.Drawing.Point(16, 370);
            lblWarnings.Name = "lblWarnings";
            lblWarnings.Text = "";
            lblWarnings.Visible = false;
            // 
            // btnAbout
            // 
            btnAbout.Location = new System.Drawing.Point(800, 430);
            btnAbout.Name = "btnAbout";
            btnAbout.Size = new System.Drawing.Size(118, 30);
            btnAbout.Text = "About";
            btnAbout.UseVisualStyleBackColor = true;
            // 
            // label24
            // 
            label24.AutoSize = true;
            label24.Location = new System.Drawing.Point(16, 256);
            label24.Name = "label24";
            label24.Text = "Quality Mode";
            // 
            // label25
            // 
            label25.AutoSize = true;
            label25.Location = new System.Drawing.Point(330, 256);
            label25.Name = "label25";
            label25.Text = "Video Bitrate";
            // 
            // label26
            // 
            label26.AutoSize = true;
            label26.Location = new System.Drawing.Point(630, 256);
            label26.Name = "label26";
            label26.Text = "Target Size (MB)";
            // 
            // cmbQualityMode
            // 
            cmbQualityMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmbQualityMode.FormattingEnabled = true;
            cmbQualityMode.Items.AddRange(new object[] { "CRF (quality)", "Video Bitrate", "Target Size (MB)" });
            cmbQualityMode.Location = new System.Drawing.Point(120, 252);
            cmbQualityMode.Name = "cmbQualityMode";
            cmbQualityMode.Size = new System.Drawing.Size(200, 23);
            // 
            // txtVideoBitrate
            // 
            txtVideoBitrate.Location = new System.Drawing.Point(434, 252);
            txtVideoBitrate.Name = "txtVideoBitrate";
            txtVideoBitrate.PlaceholderText = "e.g. 3000k";
            txtVideoBitrate.Size = new System.Drawing.Size(90, 23);
            // 
            // txtTargetSizeMB
            // 
            txtTargetSizeMB.Location = new System.Drawing.Point(740, 252);
            txtTargetSizeMB.Name = "txtTargetSizeMB";
            txtTargetSizeMB.PlaceholderText = "e.g. 25";
            txtTargetSizeMB.Size = new System.Drawing.Size(100, 23);
            // 
            // btnRetryFailed
            // 
            btnRetryFailed.Location = new System.Drawing.Point(258, 350);
            btnRetryFailed.Name = "btnRetryFailed";
            btnRetryFailed.Size = new System.Drawing.Size(110, 27);
            btnRetryFailed.Text = "Retry Failed";
            btnRetryFailed.UseVisualStyleBackColor = true;
            // 
            // btnOpenOutput
            // 
            btnOpenOutput.Location = new System.Drawing.Point(16, 425);
            btnOpenOutput.Name = "btnOpenOutput";
            btnOpenOutput.Size = new System.Drawing.Size(170, 27);
            btnOpenOutput.Text = "Open Output Folder";
            btnOpenOutput.UseVisualStyleBackColor = true;
            // 
            // cmbCrf
            // 
            cmbCrf.DisplayMember = "Title";
            cmbCrf.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmbCrf.FormattingEnabled = true;
            cmbCrf.Location = new System.Drawing.Point(120, 92);
            cmbCrf.Name = "cmbCrf";
            cmbCrf.Size = new System.Drawing.Size(170, 23);
            cmbCrf.TabIndex = 1;
            // 
            // cmbVideoCodec
            // 
            cmbVideoCodec.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmbVideoCodec.FormattingEnabled = true;
            cmbVideoCodec.Items.AddRange(new object[] { "", "libx264", "libx265", "copy" });
            cmbVideoCodec.Location = new System.Drawing.Point(120, 60);
            cmbVideoCodec.Name = "cmbVideoCodec";
            cmbVideoCodec.Size = new System.Drawing.Size(170, 23);
            cmbVideoCodec.TabIndex = 3;
            // 
            // cmbAudioCodec
            // 
            cmbAudioCodec.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmbAudioCodec.FormattingEnabled = true;
            cmbAudioCodec.Items.AddRange(new object[] { "", "aac", "ac3", "mp3", "copy" });
            cmbAudioCodec.Location = new System.Drawing.Point(434, 156);
            cmbAudioCodec.Name = "cmbAudioCodec";
            cmbAudioCodec.Size = new System.Drawing.Size(170, 23);
            cmbAudioCodec.TabIndex = 5;
            // 
            // cmbProfile
            // 
            cmbProfile.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmbProfile.FormattingEnabled = true;
            cmbProfile.Items.AddRange(new object[] { "", "baseline", "main", "high" });
            cmbProfile.Location = new System.Drawing.Point(120, 220);
            cmbProfile.Name = "cmbProfile";
            cmbProfile.Size = new System.Drawing.Size(100, 23);
            cmbProfile.TabIndex = 7;
            // 
            // cmbPreset
            // 
            cmbPreset.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmbPreset.FormattingEnabled = true;
            cmbPreset.Items.AddRange(new object[] { "", "ultrafast", "superfast", "veryfast", "faster", "fast", "medium", "slow", "slower", "veryslow", "placebo" });
            cmbPreset.Location = new System.Drawing.Point(120, 124);
            cmbPreset.Name = "cmbPreset";
            cmbPreset.Size = new System.Drawing.Size(170, 23);
            cmbPreset.TabIndex = 9;
            // 
            // cmbTune
            // 
            cmbTune.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmbTune.FormattingEnabled = true;
            cmbTune.Items.AddRange(new object[] { "", "film", "animation", "grain", "stillimage", "fastdecode", "zerolatency" });
            cmbTune.Location = new System.Drawing.Point(120, 156);
            cmbTune.Name = "cmbTune";
            cmbTune.Size = new System.Drawing.Size(170, 23);
            cmbTune.TabIndex = 11;
            // 
            // cmbPixFormat
            // 
            cmbPixFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmbPixFormat.FormattingEnabled = true;
            cmbPixFormat.Items.AddRange(new object[] { "", "yuv420p", "yuv420p10le" });
            cmbPixFormat.Location = new System.Drawing.Point(120, 188);
            cmbPixFormat.Name = "cmbPixFormat";
            cmbPixFormat.Size = new System.Drawing.Size(170, 23);
            cmbPixFormat.TabIndex = 13;
            // 
            // cmbOutputFormat
            // 
            cmbOutputFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmbOutputFormat.FormattingEnabled = true;
            cmbOutputFormat.Items.AddRange(new object[] { "mkv", "mp4", "mp3", "m4a", "wav", "gif" });
            cmbOutputFormat.Location = new System.Drawing.Point(434, 60);
            cmbOutputFormat.Name = "cmbOutputFormat";
            cmbOutputFormat.Size = new System.Drawing.Size(170, 23);
            cmbOutputFormat.TabIndex = 16;
            // 
            // cmbLevel
            // 
            cmbLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmbLevel.FormattingEnabled = true;
            cmbLevel.Items.AddRange(new object[] { "", "3", "4.1", "5" });
            cmbLevel.Location = new System.Drawing.Point(240, 220);
            cmbLevel.Name = "cmbLevel";
            cmbLevel.Size = new System.Drawing.Size(50, 23);
            cmbLevel.TabIndex = 21;
            // 
            // cmbSavedPresets
            // 
            cmbSavedPresets.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmbSavedPresets.FormattingEnabled = true;
            cmbSavedPresets.Location = new System.Drawing.Point(592, 22);
            cmbSavedPresets.Name = "cmbSavedPresets";
            cmbSavedPresets.Size = new System.Drawing.Size(190, 23);
            cmbSavedPresets.TabIndex = 25;
            // 
            // cmbQuickPreset
            // 
            cmbQuickPreset.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmbQuickPreset.FormattingEnabled = true;
            cmbQuickPreset.Location = new System.Drawing.Point(120, 22);
            cmbQuickPreset.Name = "cmbQuickPreset";
            cmbQuickPreset.Size = new System.Drawing.Size(310, 23);
            cmbQuickPreset.TabIndex = 40;
            // 
            // cmbScale
            // 
            cmbScale.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmbScale.FormattingEnabled = true;
            cmbScale.Items.AddRange(new object[] { "", "480", "720", "1080", "1440", "2160" });
            cmbScale.Location = new System.Drawing.Point(434, 92);
            cmbScale.Name = "cmbScale";
            cmbScale.Size = new System.Drawing.Size(170, 23);
            cmbScale.TabIndex = 41;
            // 
            // cmbFps
            // 
            cmbFps.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmbFps.FormattingEnabled = true;
            cmbFps.Items.AddRange(new object[] { "", "23.976", "24", "25", "30", "50", "60" });
            cmbFps.Location = new System.Drawing.Point(434, 124);
            cmbFps.Name = "cmbFps";
            cmbFps.Size = new System.Drawing.Size(170, 23);
            cmbFps.TabIndex = 42;
            // 
            // cmbAudioBitrate
            // 
            cmbAudioBitrate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmbAudioBitrate.FormattingEnabled = true;
            cmbAudioBitrate.Items.AddRange(new object[] { "", "64k", "96k", "128k", "160k", "192k", "256k", "320k" });
            cmbAudioBitrate.Location = new System.Drawing.Point(434, 188);
            cmbAudioBitrate.Name = "cmbAudioBitrate";
            cmbAudioBitrate.Size = new System.Drawing.Size(170, 23);
            cmbAudioBitrate.TabIndex = 43;
            // 
            // cmbSubs
            // 
            cmbSubs.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmbSubs.FormattingEnabled = true;
            cmbSubs.Items.AddRange(new object[] { "", "copy", "mov_text", "drop" });
            cmbSubs.Location = new System.Drawing.Point(434, 220);
            cmbSubs.Name = "cmbSubs";
            cmbSubs.Size = new System.Drawing.Size(170, 23);
            cmbSubs.TabIndex = 44;
            // 
            // cmbOutputName
            // 
            cmbOutputName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmbOutputName.FormattingEnabled = true;
            cmbOutputName.Items.AddRange(new object[] { "Append _conv suffix", "Keep name only (overwrite)" });
            cmbOutputName.Location = new System.Drawing.Point(740, 124);
            cmbOutputName.Name = "cmbOutputName";
            cmbOutputName.Size = new System.Drawing.Size(180, 23);
            cmbOutputName.TabIndex = 45;
            // 
            // txtFilePath
            // 
            txtFilePath.Location = new System.Drawing.Point(120, 22);
            txtFilePath.Name = "txtFilePath";
            txtFilePath.Size = new System.Drawing.Size(270, 23);
            txtFilePath.TabIndex = 18;
            // 
            // txtDestFolder
            // 
            txtDestFolder.Location = new System.Drawing.Point(120, 52);
            txtDestFolder.Name = "txtDestFolder";
            txtDestFolder.Size = new System.Drawing.Size(270, 23);
            txtDestFolder.TabIndex = 28;
            // 
            // txtTrimStart
            // 
            txtTrimStart.Location = new System.Drawing.Point(820, 60);
            txtTrimStart.Name = "txtTrimStart";
            txtTrimStart.Size = new System.Drawing.Size(100, 23);
            txtTrimStart.TabIndex = 46;
            // 
            // txtTrimEnd
            // 
            txtTrimEnd.Location = new System.Drawing.Point(820, 92);
            txtTrimEnd.Name = "txtTrimEnd";
            txtTrimEnd.Size = new System.Drawing.Size(100, 23);
            txtTrimEnd.TabIndex = 47;
            // 
            // txtExtraArgs
            // 
            txtExtraArgs.Location = new System.Drawing.Point(740, 156);
            txtExtraArgs.Name = "txtExtraArgs";
            txtExtraArgs.Size = new System.Drawing.Size(180, 23);
            txtExtraArgs.TabIndex = 48;
            // 
            // txtCommandLine
            // 
            txtCommandLine.DetectUrls = false;
            txtCommandLine.Location = new System.Drawing.Point(16, 78);
            txtCommandLine.Name = "txtCommandLine";
            txtCommandLine.ReadOnly = true;
            txtCommandLine.Size = new System.Drawing.Size(932, 110);
            txtCommandLine.TabIndex = 14;
            txtCommandLine.Text = "";
            // 
            // txtLog
            // 
            txtLog.DetectUrls = false;
            txtLog.Font = new System.Drawing.Font("Consolas", 8.5F);
            txtLog.Location = new System.Drawing.Point(16, 214);
            txtLog.Name = "txtLog";
            txtLog.ReadOnly = true;
            txtLog.Size = new System.Drawing.Size(932, 250);
            txtLog.TabIndex = 50;
            txtLog.Text = "";
            // 
            // btnSelectFile
            // 
            btnSelectFile.Location = new System.Drawing.Point(396, 22);
            btnSelectFile.Name = "btnSelectFile";
            btnSelectFile.Size = new System.Drawing.Size(43, 23);
            btnSelectFile.TabIndex = 19;
            btnSelectFile.Text = "...";
            btnSelectFile.UseVisualStyleBackColor = true;
            btnSelectFile.Click += btnSelectFile_Click;
            // 
            // btnAddFiles
            // 
            btnAddFiles.Location = new System.Drawing.Point(700, 58);
            btnAddFiles.Name = "btnAddFiles";
            btnAddFiles.Size = new System.Drawing.Size(100, 25);
            btnAddFiles.TabIndex = 51;
            btnAddFiles.Text = "Add Files";
            btnAddFiles.UseVisualStyleBackColor = true;
            btnAddFiles.Click += btnAddFiles_Click;
            // 
            // btnSelectDestFolder
            // 
            btnSelectDestFolder.Location = new System.Drawing.Point(396, 52);
            btnSelectDestFolder.Name = "btnSelectDestFolder";
            btnSelectDestFolder.Size = new System.Drawing.Size(43, 23);
            btnSelectDestFolder.TabIndex = 29;
            btnSelectDestFolder.Text = "...";
            btnSelectDestFolder.UseVisualStyleBackColor = true;
            btnSelectDestFolder.Click += btnSelectDestFolder_Click;
            // 
            // btnConvert
            // 
            btnConvert.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            btnConvert.Location = new System.Drawing.Point(824, 346);
            btnConvert.Name = "btnConvert";
            btnConvert.Size = new System.Drawing.Size(124, 34);
            btnConvert.TabIndex = 22;
            btnConvert.Text = "Convert";
            btnConvert.UseVisualStyleBackColor = true;
            btnConvert.Click += btnConvert_Click;
            // 
            // btnSavePreset
            // 
            btnSavePreset.Location = new System.Drawing.Point(838, 21);
            btnSavePreset.Name = "btnSavePreset";
            btnSavePreset.Size = new System.Drawing.Size(80, 25);
            btnSavePreset.TabIndex = 26;
            btnSavePreset.Text = "Save";
            btnSavePreset.UseVisualStyleBackColor = true;
            btnSavePreset.Click += btnSavePreset_Click;
            // 
            // btnBrowsePresets
            // 
            btnBrowsePresets.Location = new System.Drawing.Point(436, 22);
            btnBrowsePresets.Name = "btnBrowsePresets";
            btnBrowsePresets.Size = new System.Drawing.Size(100, 23);
            btnBrowsePresets.Text = "Presets...";
            btnBrowsePresets.UseVisualStyleBackColor = true;
            // 
            // lblPresetInfo
            // 
            lblPresetInfo.AutoSize = true;
            lblPresetInfo.ForeColor = System.Drawing.Color.DimGray;
            lblPresetInfo.Location = new System.Drawing.Point(16, 292);
            lblPresetInfo.Name = "lblPresetInfo";
            lblPresetInfo.Text = "";
            // 
            // btnDeletePreset
            // 
            btnDeletePreset.Location = new System.Drawing.Point(788, 21);
            btnDeletePreset.Name = "btnDeletePreset";
            btnDeletePreset.Size = new System.Drawing.Size(44, 25);
            btnDeletePreset.TabIndex = 31;
            btnDeletePreset.Text = "Del";
            btnDeletePreset.UseVisualStyleBackColor = true;
            btnDeletePreset.Click += btnDeletePreset_Click;
            // 
            // btnClearQueue
            // 
            btnClearQueue.Location = new System.Drawing.Point(16, 350);
            btnClearQueue.Name = "btnClearQueue";
            btnClearQueue.Size = new System.Drawing.Size(100, 27);
            btnClearQueue.TabIndex = 52;
            btnClearQueue.Text = "Clear Queue";
            btnClearQueue.UseVisualStyleBackColor = true;
            btnClearQueue.Click += btnClearQueue_Click;
            // 
            // btnRemoveSelected
            // 
            btnRemoveSelected.Location = new System.Drawing.Point(122, 350);
            btnRemoveSelected.Name = "btnRemoveSelected";
            btnRemoveSelected.Size = new System.Drawing.Size(130, 27);
            btnRemoveSelected.TabIndex = 53;
            btnRemoveSelected.Text = "Remove Selected";
            btnRemoveSelected.UseVisualStyleBackColor = true;
            btnRemoveSelected.Click += btnRemoveSelected_Click;
            // 
            // btnCopyCommand
            // 
            btnCopyCommand.Location = new System.Drawing.Point(16, 22);
            btnCopyCommand.Name = "btnCopyCommand";
            btnCopyCommand.Size = new System.Drawing.Size(140, 27);
            btnCopyCommand.TabIndex = 54;
            btnCopyCommand.Text = "Copy Command";
            btnCopyCommand.UseVisualStyleBackColor = true;
            btnCopyCommand.Click += btnCopyCommand_Click;
            // 
            // btnOpenLastLog
            // 
            btnOpenLastLog.Location = new System.Drawing.Point(166, 22);
            btnOpenLastLog.Name = "btnOpenLastLog";
            btnOpenLastLog.Size = new System.Drawing.Size(140, 27);
            btnOpenLastLog.TabIndex = 55;
            btnOpenLastLog.Text = "Open Last Log";
            btnOpenLastLog.UseVisualStyleBackColor = true;
            btnOpenLastLog.Click += btnOpenLastLog_Click;
            // 
            // lstQueue
            // 
            lstQueue.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { colFile, colStatus });
            lstQueue.FullRowSelect = true;
            lstQueue.Location = new System.Drawing.Point(16, 102);
            lstQueue.MultiSelect = true;
            lstQueue.Name = "lstQueue";
            lstQueue.Size = new System.Drawing.Size(932, 200);
            lstQueue.TabIndex = 56;
            lstQueue.View = System.Windows.Forms.View.Details;
            // 
            // colFile
            // 
            colFile.Text = "File";
            colFile.Width = 740;
            // 
            // colStatus
            // 
            colStatus.Text = "Status";
            colStatus.Width = 160;
            // 
            // chkShutdown
            // 
            chkShutdown.AutoSize = true;
            chkShutdown.Location = new System.Drawing.Point(380, 355);
            chkShutdown.Name = "chkShutdown";
            chkShutdown.Text = "Shut down PC when done";
            chkShutdown.UseVisualStyleBackColor = true;
            // 
            // chkNotify
            // 
            chkNotify.AutoSize = true;
            chkNotify.Location = new System.Drawing.Point(440, 355);
            chkNotify.Name = "chkNotify";
            chkNotify.Text = "Show message when done";
            chkNotify.UseVisualStyleBackColor = true;
            // 
            // progressBar1
            // 
            progressBar1.Location = new System.Drawing.Point(16, 386);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new System.Drawing.Size(932, 12);
            progressBar1.TabIndex = 30;
            // 
            // event wiring
            // 
            cmbCrf.SelectedIndexChanged += SettingChanged;
            cmbVideoCodec.SelectedIndexChanged += SettingChanged;
            cmbAudioCodec.SelectedIndexChanged += SettingChanged;
            cmbProfile.SelectedIndexChanged += SettingChanged;
            cmbPreset.SelectedIndexChanged += SettingChanged;
            cmbTune.SelectedIndexChanged += SettingChanged;
            cmbPixFormat.SelectedIndexChanged += SettingChanged;
            cmbOutputFormat.SelectedIndexChanged += SettingChanged;
            cmbLevel.SelectedIndexChanged += SettingChanged;
            cmbSavedPresets.SelectedIndexChanged += cmbSavedPresets_SelectedIndexChanged;
            cmbQuickPreset.SelectedIndexChanged += cmbQuickPreset_SelectedIndexChanged;
            cmbScale.SelectedIndexChanged += SettingChanged;
            cmbFps.SelectedIndexChanged += SettingChanged;
            cmbAudioBitrate.SelectedIndexChanged += SettingChanged;
            cmbSubs.SelectedIndexChanged += SettingChanged;
            cmbOutputName.SelectedIndexChanged += SettingChanged;
            txtTrimStart.TextChanged += SettingChanged;
            txtTrimEnd.TextChanged += SettingChanged;
            txtExtraArgs.TextChanged += SettingChanged;
            txtDestFolder.TextChanged += SettingChanged;
            txtFilePath.TextChanged += txtFilePath_TextChanged;
            lstQueue.DragEnter += MainForm_DragEnter;
            lstQueue.DragDrop += MainForm_DragDrop;
            cmbQualityMode.SelectedIndexChanged += SettingChanged;
            txtVideoBitrate.TextChanged += SettingChanged;
            txtTargetSizeMB.TextChanged += SettingChanged;
            btnRetryFailed.Click += btnRetryFailed_Click;
            btnOpenOutput.Click += btnOpenOutput_Click;
            btnBrowsePresets.Click += btnBrowsePresets_Click;
            btnAbout.Click += btnAbout_Click;
            // 
            // tabs
            // 
            tabs.Controls.Add(tabFiles);
            tabs.Controls.Add(tabSettings);
            tabs.Controls.Add(tabCommand);
            tabs.Location = new System.Drawing.Point(8, 8);
            tabs.Name = "tabs";
            tabs.SelectedIndex = 0;
            tabs.Size = new System.Drawing.Size(964, 510);
            tabs.TabIndex = 60;
            // 
            // tabFiles
            // 
            tabFiles.Controls.Add(label9);
            tabFiles.Controls.Add(txtFilePath);
            tabFiles.Controls.Add(btnSelectFile);
            tabFiles.Controls.Add(label12);
            tabFiles.Controls.Add(txtDestFolder);
            tabFiles.Controls.Add(btnSelectDestFolder);
            tabFiles.Controls.Add(labelQueue);
            tabFiles.Controls.Add(lstQueue);
            tabFiles.Controls.Add(lblMediaInfo);
            tabFiles.Controls.Add(btnClearQueue);
            tabFiles.Controls.Add(btnRemoveSelected);
            tabFiles.Controls.Add(btnRetryFailed);
            tabFiles.Controls.Add(btnOpenOutput);
            tabFiles.Controls.Add(btnAddFiles);
            tabFiles.Controls.Add(chkShutdown);
            tabFiles.Controls.Add(chkNotify);
            tabFiles.Controls.Add(btnConvert);
            tabFiles.Controls.Add(progressBar1);
            tabFiles.Controls.Add(lblProgress);
            tabFiles.Location = new System.Drawing.Point(4, 24);
            tabFiles.Name = "tabFiles";
            tabFiles.Padding = new System.Windows.Forms.Padding(3);
            tabFiles.Size = new System.Drawing.Size(956, 482);
            tabFiles.TabIndex = 0;
            tabFiles.Text = "Files && Queue";
            // 
            // tabSettings
            // 
            tabSettings.Controls.Add(label13);
            tabSettings.Controls.Add(cmbQuickPreset);
            tabSettings.Controls.Add(btnBrowsePresets);
            tabSettings.Controls.Add(lblPresetInfo);
            tabSettings.Controls.Add(label11);
            tabSettings.Controls.Add(cmbSavedPresets);
            tabSettings.Controls.Add(btnSavePreset);
            tabSettings.Controls.Add(btnDeletePreset);
            tabSettings.Controls.Add(label2);
            tabSettings.Controls.Add(cmbVideoCodec);
            tabSettings.Controls.Add(label1);
            tabSettings.Controls.Add(cmbCrf);
            tabSettings.Controls.Add(label5);
            tabSettings.Controls.Add(cmbPreset);
            tabSettings.Controls.Add(label6);
            tabSettings.Controls.Add(cmbTune);
            tabSettings.Controls.Add(label7);
            tabSettings.Controls.Add(cmbPixFormat);
            tabSettings.Controls.Add(label4);
            tabSettings.Controls.Add(cmbProfile);
            tabSettings.Controls.Add(label10);
            tabSettings.Controls.Add(cmbLevel);
            tabSettings.Controls.Add(label8);
            tabSettings.Controls.Add(cmbOutputFormat);
            tabSettings.Controls.Add(label14);
            tabSettings.Controls.Add(cmbScale);
            tabSettings.Controls.Add(label15);
            tabSettings.Controls.Add(cmbFps);
            tabSettings.Controls.Add(label3);
            tabSettings.Controls.Add(cmbAudioCodec);
            tabSettings.Controls.Add(label16);
            tabSettings.Controls.Add(cmbAudioBitrate);
            tabSettings.Controls.Add(label17);
            tabSettings.Controls.Add(cmbSubs);
            tabSettings.Controls.Add(label18);
            tabSettings.Controls.Add(txtTrimStart);
            tabSettings.Controls.Add(label19);
            tabSettings.Controls.Add(txtTrimEnd);
            tabSettings.Controls.Add(label20);
            tabSettings.Controls.Add(cmbOutputName);
            tabSettings.Controls.Add(label21);
            tabSettings.Controls.Add(txtExtraArgs);
            tabSettings.Controls.Add(label24);
            tabSettings.Controls.Add(cmbQualityMode);
            tabSettings.Controls.Add(label25);
            tabSettings.Controls.Add(txtVideoBitrate);
            tabSettings.Controls.Add(label26);
            tabSettings.Controls.Add(txtTargetSizeMB);
            tabSettings.Controls.Add(lblHwHint);
            tabSettings.Controls.Add(lblCodecInfo);
            tabSettings.Controls.Add(lblWarnings);
            tabSettings.Controls.Add(btnAbout);
            tabSettings.Location = new System.Drawing.Point(4, 24);
            tabSettings.Name = "tabSettings";
            tabSettings.Size = new System.Drawing.Size(956, 482);
            tabSettings.TabIndex = 1;
            tabSettings.Text = "Settings";
            // 
            // tabCommand
            // 
            tabCommand.Controls.Add(btnCopyCommand);
            tabCommand.Controls.Add(btnOpenLastLog);
            tabCommand.Controls.Add(label23);
            tabCommand.Controls.Add(txtCommandLine);
            tabCommand.Controls.Add(label22);
            tabCommand.Controls.Add(txtLog);
            tabCommand.Location = new System.Drawing.Point(4, 24);
            tabCommand.Name = "tabCommand";
            tabCommand.Size = new System.Drawing.Size(956, 482);
            tabCommand.TabIndex = 2;
            tabCommand.Text = "Command && Log";
            // 
            // ofd
            // 
            ofd.Filter = "Media Files|*.mkv;*.mp4;*.avi;*.mov;*.wmv;*.flv;*.webm;*.ts;*.m4v;*.mpg;*.mpeg;*.vob;*.3gp;*.mp3;*.wav;*.aac;*.flac;*.ogg;*.m4a;*.wma|Video Files|*.mkv;*.mp4;*.avi;*.mov;*.wmv;*.flv;*.webm;*.ts;*.m4v;*.mpg|All Files|*.*";
            ofd.Multiselect = true;
            ofd.RestoreDirectory = true;
            ofd.Title = "Select Media File(s)";
            // 
            // MainForm
            // 
            AllowDrop = true;
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(980, 560);
            Controls.Add(tabs);
            Name = "MainForm";
            Text = Core.AppVersion.GetTitle();
            DragDrop += MainForm_DragDrop;
            DragEnter += MainForm_DragEnter;
            FormClosing += MainForm_FormClosing;
            Load += MainForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Label labelQueue;
        private System.Windows.Forms.Label lblProgress;
        private System.Windows.Forms.Label lblMediaInfo;
        private System.Windows.Forms.Label lblHwHint;
        private System.Windows.Forms.ComboBox cmbCrf;
        private System.Windows.Forms.ComboBox cmbVideoCodec;
        private System.Windows.Forms.ComboBox cmbAudioCodec;
        private System.Windows.Forms.ComboBox cmbProfile;
        private System.Windows.Forms.ComboBox cmbPreset;
        private System.Windows.Forms.ComboBox cmbTune;
        private System.Windows.Forms.ComboBox cmbPixFormat;
        private System.Windows.Forms.ComboBox cmbOutputFormat;
        private System.Windows.Forms.ComboBox cmbLevel;
        private System.Windows.Forms.ComboBox cmbSavedPresets;
        private System.Windows.Forms.ComboBox cmbQuickPreset;
        private System.Windows.Forms.ComboBox cmbScale;
        private System.Windows.Forms.ComboBox cmbFps;
        private System.Windows.Forms.ComboBox cmbAudioBitrate;
        private System.Windows.Forms.ComboBox cmbSubs;
        private System.Windows.Forms.ComboBox cmbOutputName;
        private System.Windows.Forms.TextBox txtFilePath;
        private System.Windows.Forms.TextBox txtDestFolder;
        private System.Windows.Forms.TextBox txtTrimStart;
        private System.Windows.Forms.TextBox txtTrimEnd;
        private System.Windows.Forms.TextBox txtExtraArgs;
        private System.Windows.Forms.RichTextBox txtCommandLine;
        private System.Windows.Forms.RichTextBox txtLog;
        private System.Windows.Forms.Button btnSelectFile;
        private System.Windows.Forms.Button btnAddFiles;
        private System.Windows.Forms.Button btnSelectDestFolder;
        private System.Windows.Forms.Button btnConvert;
        private System.Windows.Forms.Button btnSavePreset;
        private System.Windows.Forms.Button btnDeletePreset;
        private System.Windows.Forms.Button btnClearQueue;
        private System.Windows.Forms.Button btnRemoveSelected;
        private System.Windows.Forms.Button btnCopyCommand;
        private System.Windows.Forms.Button btnOpenLastLog;
        private System.Windows.Forms.ListView lstQueue;
        private System.Windows.Forms.ColumnHeader colFile;
        private System.Windows.Forms.ColumnHeader colStatus;
        private System.Windows.Forms.CheckBox chkShutdown;
        private System.Windows.Forms.CheckBox chkNotify;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.TabControl tabs;
        private System.Windows.Forms.TabPage tabFiles;
        private System.Windows.Forms.TabPage tabSettings;
        private System.Windows.Forms.TabPage tabCommand;
        private System.Windows.Forms.OpenFileDialog ofd;
        private System.Windows.Forms.FolderBrowserDialog oFolderDialog;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.ComboBox cmbQualityMode;
        private System.Windows.Forms.TextBox txtVideoBitrate;
        private System.Windows.Forms.TextBox txtTargetSizeMB;
        private System.Windows.Forms.Button btnRetryFailed;
        private System.Windows.Forms.Button btnOpenOutput;
        private System.Windows.Forms.Button btnBrowsePresets;
        private System.Windows.Forms.Label lblPresetInfo;
        private System.Windows.Forms.Button btnAbout;
        private System.Windows.Forms.Label lblCodecInfo;
        private System.Windows.Forms.Label lblWarnings;
    }
}
