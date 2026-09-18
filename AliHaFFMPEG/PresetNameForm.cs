using System;
using System.Windows.Forms;

namespace AliHaFFMPEG
{
    public partial class PresetNameForm : Form
    {
        [System.ComponentModel.DesignerSerializationVisibility(
            System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public string PresetName { get; set; }
        public PresetNameForm()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            PresetName = txtPresetName.Text;
        }

        private void PresetNameForm_Load(object sender, EventArgs e)
        {
            txtPresetName.Text = PresetName;
        }
    }
}