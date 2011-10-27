using System;
using System.Windows.Forms;

namespace VersionOne.VisualStudio.VSPackage.Forms {
    public partial class OptionsDialog : Form {
        public OptionsDialog() {
            InitializeComponent();
            optionsPage.LoadSettings();

            btnOk.Click += btnOk_Click;
        }

        private void btnOk_Click(object sender, EventArgs e) {
            optionsPage.SaveSettings();
            Close();
        }
    }
}