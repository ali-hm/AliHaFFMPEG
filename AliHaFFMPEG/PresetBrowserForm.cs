using System;
using System.Collections.Generic;
using System.Windows.Forms;
using AliHaFFMPEG.Core;

namespace AliHaFFMPEG
{
    /// <summary>Browsable list of all built-in presets, so none of them are hidden in a dropdown.</summary>
    public partial class PresetBrowserForm : Form
    {
        public MyPreset SelectedPreset { get; private set; }

        public PresetBrowserForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var row = 0;
            foreach (var pair in BuiltInPresets.All)
            {
                var item = new ListViewItem(pair.Key) { Tag = pair.Value };
                item.SubItems.Add(BuiltInPresets.Describe(pair.Value));
                lvPresets.Items.Add(item);
                row++;
            }

            Text = $"Presets ({row} built-in)";
            if (lvPresets.Items.Count > 0)
            {
                lvPresets.Items[0].Selected = true;
            }
        }

        private void lvPresets_DoubleClick(object sender, EventArgs e)
        {
            Accept();
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            Accept();
        }

        private void Accept()
        {
            if (lvPresets.SelectedItems.Count == 0)
            {
                MessageBox.Show("Select a preset first.", "Presets",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            SelectedPreset = lvPresets.SelectedItems[0].Tag as MyPreset;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}