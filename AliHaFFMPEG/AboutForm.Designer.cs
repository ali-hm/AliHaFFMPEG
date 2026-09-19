namespace AliHaFFMPEG
{
    partial class AboutForm
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
            lblTitle = new System.Windows.Forms.Label();
            lblVersion = new System.Windows.Forms.Label();
            lblAuthor = new System.Windows.Forms.Label();
            lblDescription = new System.Windows.Forms.Label();
            lblFfmpeg = new System.Windows.Forms.Label();
            lblStatus = new System.Windows.Forms.Label();
            lnkReleases = new System.Windows.Forms.LinkLabel();
            lnkGithub = new System.Windows.Forms.LinkLabel();
            btnCheckUpdate = new System.Windows.Forms.Button();
            btnInstallUpdate = new System.Windows.Forms.Button();
            btnDownloadFfmpeg = new System.Windows.Forms.Button();
            btnOpenDataFolder = new System.Windows.Forms.Button();
            progressBar1 = new System.Windows.Forms.ProgressBar();
            btnClose = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            lblTitle.Location = new System.Drawing.Point(20, 18);
            lblTitle.Name = "lblTitle";
            lblTitle.Text = "AliHa FFMPEG";
            // 
            // lblVersion
            // 
            lblVersion.AutoSize = true;
            lblVersion.Location = new System.Drawing.Point(22, 54);
            lblVersion.Name = "lblVersion";
            lblVersion.Text = "Version";
            // 
            // lblAuthor
            // 
            lblAuthor.AutoSize = true;
            lblAuthor.Location = new System.Drawing.Point(22, 76);
            lblAuthor.Name = "lblAuthor";
            lblAuthor.Text = "Author: Ali Hamidi";
            // 
            // lblDescription
            // 
            lblDescription.AutoSize = true;
            lblDescription.Location = new System.Drawing.Point(22, 98);
            lblDescription.MaximumSize = new System.Drawing.Size(520, 0);
            lblDescription.Name = "lblDescription";
            lblDescription.Text = "A friendly Windows GUI for ffmpeg: batch-convert audio and video with presets, real progress and ETA. No command line required.";
            // 
            // lblFfmpeg
            // 
            lblFfmpeg.AutoSize = true;
            lblFfmpeg.ForeColor = System.Drawing.Color.DimGray;
            lblFfmpeg.Location = new System.Drawing.Point(22, 138);
            lblFfmpeg.MaximumSize = new System.Drawing.Size(520, 0);
            lblFfmpeg.Name = "lblFfmpeg";
            lblFfmpeg.Text = "ffmpeg";
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Location = new System.Drawing.Point(22, 172);
            lblStatus.MaximumSize = new System.Drawing.Size(520, 0);
            lblStatus.Name = "lblStatus";
            lblStatus.Text = "";
            // 
            // lnkReleases
            // 
            lnkReleases.AutoSize = true;
            lnkReleases.LinkColor = System.Drawing.Color.RoyalBlue;
            lnkReleases.Location = new System.Drawing.Point(22, 214);
            lnkReleases.Name = "lnkReleases";
            lnkReleases.Text = "Releases on GitHub";
            lnkReleases.VisitedLinkColor = System.Drawing.Color.MediumPurple;
            lnkReleases.LinkClicked += lnkReleases_LinkClicked;
            lnkGithub.AutoSize = true;
            lnkGithub.LinkColor = System.Drawing.Color.RoyalBlue;
            lnkGithub.Location = new System.Drawing.Point(322, 214);
            lnkGithub.Name = "lnkGithub";
            lnkGithub.Text = "Source code on GitHub";
            lnkGithub.VisitedLinkColor = System.Drawing.Color.MediumPurple;
            lnkGithub.LinkClicked += lnkGithub_LinkClicked;
            // 
            // btnCheckUpdate
            // 
            btnCheckUpdate.Location = new System.Drawing.Point(22, 242);
            btnCheckUpdate.Name = "btnCheckUpdate";
            btnCheckUpdate.Size = new System.Drawing.Size(170, 30);
            btnCheckUpdate.Text = "Check for updates";
            btnCheckUpdate.UseVisualStyleBackColor = true;
            btnCheckUpdate.Click += btnCheckUpdate_Click;
            // 
            // btnInstallUpdate
            // 
            btnInstallUpdate.Enabled = false;
            btnInstallUpdate.Location = new System.Drawing.Point(202, 242);
            btnInstallUpdate.Name = "btnInstallUpdate";
            btnInstallUpdate.Size = new System.Drawing.Size(190, 30);
            btnInstallUpdate.Text = "Download and install";
            btnInstallUpdate.UseVisualStyleBackColor = true;
            btnInstallUpdate.Click += btnInstallUpdate_Click;
            // 
            // btnDownloadFfmpeg
            // 
            btnDownloadFfmpeg.Location = new System.Drawing.Point(22, 280);
            btnDownloadFfmpeg.Name = "btnDownloadFfmpeg";
            btnDownloadFfmpeg.Size = new System.Drawing.Size(170, 30);
            btnDownloadFfmpeg.Text = "Download ffmpeg";
            btnDownloadFfmpeg.UseVisualStyleBackColor = true;
            btnDownloadFfmpeg.Click += btnDownloadFfmpeg_Click;
            // 
            // btnOpenDataFolder
            // 
            btnOpenDataFolder.Location = new System.Drawing.Point(202, 280);
            btnOpenDataFolder.Name = "btnOpenDataFolder";
            btnOpenDataFolder.Size = new System.Drawing.Size(190, 30);
            btnOpenDataFolder.Text = "Open data folder";
            btnOpenDataFolder.UseVisualStyleBackColor = true;
            btnOpenDataFolder.Click += btnOpenDataFolder_Click;
            // 
            // progressBar1
            // 
            progressBar1.Location = new System.Drawing.Point(22, 324);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new System.Drawing.Size(520, 12);
            // 
            // btnClose
            // 
            btnClose.Location = new System.Drawing.Point(432, 348);
            btnClose.Name = "btnClose";
            btnClose.Size = new System.Drawing.Size(110, 30);
            btnClose.Text = "Close";
            btnClose.UseVisualStyleBackColor = true;
            btnClose.Click += btnClose_Click;
            // 
            // AboutForm
            // 
            AcceptButton = btnClose;
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = btnClose;
            ClientSize = new System.Drawing.Size(564, 392);
            Controls.Add(lblTitle);
            Controls.Add(lblVersion);
            Controls.Add(lblAuthor);
            Controls.Add(lblDescription);
            Controls.Add(lblFfmpeg);
            Controls.Add(lblStatus);
            Controls.Add(lnkReleases);
            Controls.Add(lnkGithub);
            Controls.Add(btnCheckUpdate);
            Controls.Add(btnInstallUpdate);
            Controls.Add(btnDownloadFfmpeg);
            Controls.Add(btnOpenDataFolder);
            Controls.Add(progressBar1);
            Controls.Add(btnClose);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "AboutForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "About AliHa FFMPEG";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.Label lblAuthor;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.Label lblFfmpeg;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.LinkLabel lnkReleases;
        private System.Windows.Forms.LinkLabel lnkGithub;
        private System.Windows.Forms.Button btnCheckUpdate;
        private System.Windows.Forms.Button btnInstallUpdate;
        private System.Windows.Forms.Button btnDownloadFfmpeg;
        private System.Windows.Forms.Button btnOpenDataFolder;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button btnClose;
    }
}
