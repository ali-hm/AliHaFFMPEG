namespace AliHaFFMPEG
{
    partial class PresetBrowserForm
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
            lvPresets = new System.Windows.Forms.ListView();
            colName = new System.Windows.Forms.ColumnHeader();
            colDetails = new System.Windows.Forms.ColumnHeader();
            lblHint = new System.Windows.Forms.Label();
            btnApply = new System.Windows.Forms.Button();
            btnCancel = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // lvPresets
            // 
            lvPresets.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { colName, colDetails });
            lvPresets.FullRowSelect = true;
            lvPresets.Location = new System.Drawing.Point(12, 12);
            lvPresets.MultiSelect = false;
            lvPresets.Name = "lvPresets";
            lvPresets.Size = new System.Drawing.Size(860, 400);
            lvPresets.TabIndex = 0;
            lvPresets.UseCompatibleStateImageBehavior = false;
            lvPresets.View = System.Windows.Forms.View.Details;
            lvPresets.DoubleClick += lvPresets_DoubleClick;
            // 
            // colName
            // 
            colName.Text = "Preset";
            colName.Width = 300;
            // 
            // colDetails
            // 
            colDetails.Text = "What it does";
            colDetails.Width = 540;
            // 
            // lblHint
            // 
            lblHint.AutoSize = true;
            lblHint.ForeColor = System.Drawing.Color.DimGray;
            lblHint.Location = new System.Drawing.Point(12, 420);
            lblHint.Name = "lblHint";
            lblHint.Text = "Double-click a preset (or select it and press Apply) to load it into the Settings tab.";
            // 
            // btnApply
            // 
            btnApply.Location = new System.Drawing.Point(700, 440);
            btnApply.Name = "btnApply";
            btnApply.Size = new System.Drawing.Size(172, 32);
            btnApply.TabIndex = 1;
            btnApply.Text = "Apply";
            btnApply.UseVisualStyleBackColor = true;
            btnApply.Click += btnApply_Click;
            // 
            // btnCancel
            // 
            btnCancel.Location = new System.Drawing.Point(560, 440);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(130, 32);
            btnCancel.TabIndex = 2;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // PresetBrowserForm
            // 
            AcceptButton = btnApply;
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = btnCancel;
            ClientSize = new System.Drawing.Size(884, 484);
            Controls.Add(lvPresets);
            Controls.Add(lblHint);
            Controls.Add(btnApply);
            Controls.Add(btnCancel);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "PresetBrowserForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Presets";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.ListView lvPresets;
        private System.Windows.Forms.ColumnHeader colName;
        private System.Windows.Forms.ColumnHeader colDetails;
        private System.Windows.Forms.Label lblHint;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button btnCancel;
    }
}