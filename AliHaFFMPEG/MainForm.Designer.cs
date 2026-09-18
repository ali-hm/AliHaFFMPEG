namespace AliHaFFMPEG
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            label1 = new System.Windows.Forms.Label();
            cmbCrf = new System.Windows.Forms.ComboBox();
            cmbVideoCodec = new System.Windows.Forms.ComboBox();
            label2 = new System.Windows.Forms.Label();
            cmbAudioCodec = new System.Windows.Forms.ComboBox();
            label3 = new System.Windows.Forms.Label();
            cmbProfile = new System.Windows.Forms.ComboBox();
            label4 = new System.Windows.Forms.Label();
            cmbPreset = new System.Windows.Forms.ComboBox();
            label5 = new System.Windows.Forms.Label();
            cmbTune = new System.Windows.Forms.ComboBox();
            label6 = new System.Windows.Forms.Label();
            cmbPixFormat = new System.Windows.Forms.ComboBox();
            label7 = new System.Windows.Forms.Label();
            txtCommandLine = new System.Windows.Forms.RichTextBox();
            cmbOutputFormat = new System.Windows.Forms.ComboBox();
            label8 = new System.Windows.Forms.Label();
            label9 = new System.Windows.Forms.Label();
            txtFilePath = new System.Windows.Forms.TextBox();
            btnSelectFile = new System.Windows.Forms.Button();
            ofd = new System.Windows.Forms.OpenFileDialog();
            cmbLevel = new System.Windows.Forms.ComboBox();
            label10 = new System.Windows.Forms.Label();
            btnConvert = new System.Windows.Forms.Button();
            lblProgress = new System.Windows.Forms.Label();
            cmbSavedPresets = new System.Windows.Forms.ComboBox();
            label11 = new System.Windows.Forms.Label();
            btnSavePreset = new System.Windows.Forms.Button();
            txtDestFolder = new System.Windows.Forms.TextBox();
            label12 = new System.Windows.Forms.Label();
            btnSelectDestFolder = new System.Windows.Forms.Button();
            oFolderDialog = new System.Windows.Forms.FolderBrowserDialog();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(119, 137);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(28, 15);
            label1.TabIndex = 0;
            label1.Text = "CRF";
            // 
            // cmbCrf
            // 
            cmbCrf.DisplayMember = "Title";
            cmbCrf.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmbCrf.FormattingEnabled = true;
            cmbCrf.Location = new System.Drawing.Point(159, 133);
            cmbCrf.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cmbCrf.Name = "cmbCrf";
            cmbCrf.Size = new System.Drawing.Size(162, 23);
            cmbCrf.TabIndex = 1;
            cmbCrf.SelectedIndexChanged += cmbCrf_SelectedIndexChanged;
            // 
            // cmbVideoCodec
            // 
            cmbVideoCodec.DisplayMember = "Title";
            cmbVideoCodec.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmbVideoCodec.FormattingEnabled = true;
            cmbVideoCodec.Items.AddRange(new object[] { "", "libx264", "libx265", "copy" });
            cmbVideoCodec.Location = new System.Drawing.Point(159, 164);
            cmbVideoCodec.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cmbVideoCodec.Name = "cmbVideoCodec";
            cmbVideoCodec.Size = new System.Drawing.Size(162, 23);
            cmbVideoCodec.TabIndex = 3;
            cmbVideoCodec.SelectedIndexChanged += cmbCrf_SelectedIndexChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(72, 168);
            label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(74, 15);
            label2.TabIndex = 2;
            label2.Text = "Video Codec";
            // 
            // cmbAudioCodec
            // 
            cmbAudioCodec.DisplayMember = "Title";
            cmbAudioCodec.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmbAudioCodec.FormattingEnabled = true;
            cmbAudioCodec.Items.AddRange(new object[] { "", "aac", "ac3", "mp3", "copy" });
            cmbAudioCodec.Location = new System.Drawing.Point(159, 196);
            cmbAudioCodec.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cmbAudioCodec.Name = "cmbAudioCodec";
            cmbAudioCodec.Size = new System.Drawing.Size(162, 23);
            cmbAudioCodec.TabIndex = 5;
            cmbAudioCodec.SelectedIndexChanged += cmbCrf_SelectedIndexChanged;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(72, 199);
            label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(76, 15);
            label3.TabIndex = 4;
            label3.Text = "Audio Codec";
            // 
            // cmbProfile
            // 
            cmbProfile.DisplayMember = "Title";
            cmbProfile.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmbProfile.FormattingEnabled = true;
            cmbProfile.Items.AddRange(new object[] { "", "baseline", "main", "high" });
            cmbProfile.Location = new System.Drawing.Point(159, 227);
            cmbProfile.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cmbProfile.Name = "cmbProfile";
            cmbProfile.Size = new System.Drawing.Size(75, 23);
            cmbProfile.TabIndex = 7;
            cmbProfile.SelectedIndexChanged += cmbCrf_SelectedIndexChanged;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(106, 230);
            label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(41, 15);
            label4.TabIndex = 6;
            label4.Text = "Profile";
            // 
            // cmbPreset
            // 
            cmbPreset.DisplayMember = "Title";
            cmbPreset.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmbPreset.FormattingEnabled = true;
            cmbPreset.Items.AddRange(new object[] { "", "ultrafast", "superfast", "veryfast", "faster", "fast", "medium", "slow", "slower", "veryslow", "placebo" });
            cmbPreset.Location = new System.Drawing.Point(159, 258);
            cmbPreset.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cmbPreset.Name = "cmbPreset";
            cmbPreset.Size = new System.Drawing.Size(162, 23);
            cmbPreset.TabIndex = 9;
            cmbPreset.SelectedIndexChanged += cmbCrf_SelectedIndexChanged;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(108, 261);
            label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(39, 15);
            label5.TabIndex = 8;
            label5.Text = "Preset";
            // 
            // cmbTune
            // 
            cmbTune.DisplayMember = "Title";
            cmbTune.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmbTune.FormattingEnabled = true;
            cmbTune.Items.AddRange(new object[] { "", "film", "animation", "grain", "stillimage", "fastdecode", "zerolatency" });
            cmbTune.Location = new System.Drawing.Point(159, 289);
            cmbTune.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cmbTune.Name = "cmbTune";
            cmbTune.Size = new System.Drawing.Size(162, 23);
            cmbTune.TabIndex = 11;
            cmbTune.SelectedIndexChanged += cmbCrf_SelectedIndexChanged;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new System.Drawing.Point(114, 293);
            label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(33, 15);
            label6.TabIndex = 10;
            label6.Text = "Tune";
            // 
            // cmbPixFormat
            // 
            cmbPixFormat.DisplayMember = "Title";
            cmbPixFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmbPixFormat.FormattingEnabled = true;
            cmbPixFormat.Items.AddRange(new object[] { "", "yuv420p" });
            cmbPixFormat.Location = new System.Drawing.Point(159, 320);
            cmbPixFormat.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cmbPixFormat.Name = "cmbPixFormat";
            cmbPixFormat.Size = new System.Drawing.Size(162, 23);
            cmbPixFormat.TabIndex = 13;
            cmbPixFormat.SelectedIndexChanged += cmbCrf_SelectedIndexChanged;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new System.Drawing.Point(83, 324);
            label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(66, 15);
            label7.TabIndex = 12;
            label7.Text = "Pix_Format";
            // 
            // txtCommandLine
            // 
            txtCommandLine.Location = new System.Drawing.Point(13, 395);
            txtCommandLine.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            txtCommandLine.Name = "txtCommandLine";
            txtCommandLine.Size = new System.Drawing.Size(698, 121);
            txtCommandLine.TabIndex = 14;
            txtCommandLine.Text = "";
            // 
            // cmbOutputFormat
            // 
            cmbOutputFormat.DisplayMember = "Title";
            cmbOutputFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmbOutputFormat.FormattingEnabled = true;
            cmbOutputFormat.Items.AddRange(new object[] { "mkv", "mp4", "mp3" });
            cmbOutputFormat.Location = new System.Drawing.Point(468, 133);
            cmbOutputFormat.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cmbOutputFormat.Name = "cmbOutputFormat";
            cmbOutputFormat.Size = new System.Drawing.Size(162, 23);
            cmbOutputFormat.TabIndex = 16;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new System.Drawing.Point(374, 137);
            label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(86, 15);
            label8.TabIndex = 15;
            label8.Text = "Output Format";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new System.Drawing.Point(76, 22);
            label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label9.Name = "label9";
            label9.Size = new System.Drawing.Size(68, 15);
            label9.TabIndex = 17;
            label9.Text = "Select a File";
            // 
            // txtFilePath
            // 
            txtFilePath.Location = new System.Drawing.Point(159, 22);
            txtFilePath.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            txtFilePath.Name = "txtFilePath";
            txtFilePath.Size = new System.Drawing.Size(474, 23);
            txtFilePath.TabIndex = 18;
            // 
            // btnSelectFile
            // 
            btnSelectFile.Location = new System.Drawing.Point(643, 22);
            btnSelectFile.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnSelectFile.Name = "btnSelectFile";
            btnSelectFile.Size = new System.Drawing.Size(43, 23);
            btnSelectFile.TabIndex = 19;
            btnSelectFile.Text = "...";
            btnSelectFile.UseVisualStyleBackColor = true;
            btnSelectFile.Click += btnSelectFile_Click;
            // 
            // ofd
            // 
            ofd.Filter = "Video Files|*.mkv;*.mp4;*.avi;*.3gp;*.flv;*.vob;*.mpg";
            ofd.RestoreDirectory = true;
            ofd.Title = "Select a Video File";
            // 
            // cmbLevel
            // 
            cmbLevel.DisplayMember = "Title";
            cmbLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmbLevel.FormattingEnabled = true;
            cmbLevel.Items.AddRange(new object[] { "", "3", "4.1", "5" });
            cmbLevel.Location = new System.Drawing.Point(271, 227);
            cmbLevel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cmbLevel.Name = "cmbLevel";
            cmbLevel.Size = new System.Drawing.Size(50, 23);
            cmbLevel.TabIndex = 21;
            cmbLevel.SelectedIndexChanged += cmbCrf_SelectedIndexChanged;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new System.Drawing.Point(236, 230);
            label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label10.Name = "label10";
            label10.Size = new System.Drawing.Size(34, 15);
            label10.TabIndex = 20;
            label10.Text = "Level";
            // 
            // btnConvert
            // 
            btnConvert.Location = new System.Drawing.Point(462, 351);
            btnConvert.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnConvert.Name = "btnConvert";
            btnConvert.Size = new System.Drawing.Size(88, 27);
            btnConvert.TabIndex = 22;
            btnConvert.Text = "Convert";
            btnConvert.UseVisualStyleBackColor = true;
            btnConvert.Click += btnConvert_Click;
            // 
            // lblProgress
            // 
            lblProgress.AutoSize = true;
            lblProgress.Location = new System.Drawing.Point(27, 359);
            lblProgress.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblProgress.Name = "lblProgress";
            lblProgress.Size = new System.Drawing.Size(0, 15);
            lblProgress.TabIndex = 23;
            // 
            // cmbSavedPresets
            // 
            cmbSavedPresets.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmbSavedPresets.FormattingEnabled = true;
            cmbSavedPresets.Location = new System.Drawing.Point(159, 78);
            cmbSavedPresets.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cmbSavedPresets.Name = "cmbSavedPresets";
            cmbSavedPresets.Size = new System.Drawing.Size(474, 23);
            cmbSavedPresets.TabIndex = 25;
            cmbSavedPresets.SelectedIndexChanged += cmbSavedPresets_SelectedIndexChanged;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new System.Drawing.Point(63, 81);
            label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label11.Name = "label11";
            label11.Size = new System.Drawing.Size(78, 15);
            label11.TabIndex = 24;
            label11.Text = "Saved Presets";
            // 
            // btnSavePreset
            // 
            btnSavePreset.Location = new System.Drawing.Point(570, 351);
            btnSavePreset.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnSavePreset.Name = "btnSavePreset";
            btnSavePreset.Size = new System.Drawing.Size(141, 27);
            btnSavePreset.TabIndex = 26;
            btnSavePreset.Text = "Save Preset";
            btnSavePreset.UseVisualStyleBackColor = true;
            btnSavePreset.Click += btnSavePreset_Click;
            // 
            // txtDestFolder
            // 
            txtDestFolder.Location = new System.Drawing.Point(159, 51);
            txtDestFolder.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            txtDestFolder.Name = "txtDestFolder";
            txtDestFolder.Size = new System.Drawing.Size(474, 23);
            txtDestFolder.TabIndex = 28;
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Location = new System.Drawing.Point(76, 51);
            label12.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label12.Name = "label12";
            label12.Size = new System.Drawing.Size(66, 15);
            label12.TabIndex = 27;
            label12.Text = "Dest Folder";
            // 
            // btnSelectDestFolder
            // 
            btnSelectDestFolder.Location = new System.Drawing.Point(643, 51);
            btnSelectDestFolder.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnSelectDestFolder.Name = "btnSelectDestFolder";
            btnSelectDestFolder.Size = new System.Drawing.Size(43, 23);
            btnSelectDestFolder.TabIndex = 29;
            btnSelectDestFolder.Text = "...";
            btnSelectDestFolder.UseVisualStyleBackColor = true;
            btnSelectDestFolder.Click += btnSelectDestFolder_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(726, 528);
            Controls.Add(btnSelectDestFolder);
            Controls.Add(txtDestFolder);
            Controls.Add(label12);
            Controls.Add(btnSavePreset);
            Controls.Add(cmbSavedPresets);
            Controls.Add(label11);
            Controls.Add(lblProgress);
            Controls.Add(btnConvert);
            Controls.Add(cmbLevel);
            Controls.Add(label10);
            Controls.Add(btnSelectFile);
            Controls.Add(txtFilePath);
            Controls.Add(label9);
            Controls.Add(cmbOutputFormat);
            Controls.Add(label8);
            Controls.Add(txtCommandLine);
            Controls.Add(cmbPixFormat);
            Controls.Add(label7);
            Controls.Add(cmbTune);
            Controls.Add(label6);
            Controls.Add(cmbPreset);
            Controls.Add(label5);
            Controls.Add(cmbProfile);
            Controls.Add(label4);
            Controls.Add(cmbAudioCodec);
            Controls.Add(label3);
            Controls.Add(cmbVideoCodec);
            Controls.Add(label2);
            Controls.Add(cmbCrf);
            Controls.Add(label1);
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "MainForm";
            Text = "AliHa FFMPEG v1.1";
            FormClosing += MainForm_FormClosing;
            Load += MainForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        private System.Windows.Forms.Button btnSavePreset;
        private System.Windows.Forms.ComboBox cmbSavedPresets;
        private System.Windows.Forms.Label label11;

        private System.Windows.Forms.Label lblProgress;

        private System.Windows.Forms.Button btnConvert;

        private System.Windows.Forms.ComboBox cmbLevel;
        private System.Windows.Forms.Label label10;

        private System.Windows.Forms.Button btnSelectFile;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.OpenFileDialog ofd;
        private System.Windows.Forms.TextBox txtFilePath;

        private System.Windows.Forms.ComboBox cmbOutputFormat;
        private System.Windows.Forms.Label label8;

        private System.Windows.Forms.RichTextBox txtCommandLine;

        private System.Windows.Forms.ComboBox cmbPixFormat;
        private System.Windows.Forms.Label label7;

        private System.Windows.Forms.ComboBox cmbTune;
        private System.Windows.Forms.Label label6;

        private System.Windows.Forms.ComboBox cmbPreset;
        private System.Windows.Forms.Label label5;

        private System.Windows.Forms.ComboBox cmbProfile;
        private System.Windows.Forms.Label label4;

        private System.Windows.Forms.ComboBox cmbAudioCodec;
        private System.Windows.Forms.Label label3;

        private System.Windows.Forms.ComboBox cmbVideoCodec;
        private System.Windows.Forms.Label label2;

        private System.Windows.Forms.ComboBox cmbCrf;
        private System.Windows.Forms.Label label1;

        #endregion

        private System.Windows.Forms.TextBox txtDestFolder;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button btnSelectDestFolder;
        private System.Windows.Forms.FolderBrowserDialog oFolderDialog;
    }
}