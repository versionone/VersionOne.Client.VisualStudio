using System.ComponentModel;
using System.Windows.Forms;
using VersionOne.VisualStudio.VSPackage.Forms;

namespace VersionOne.VisualStudio.VSPackage.Controls {
    public partial class ErrorMessageControl : UserControl {
        private LinkLabel lnkOptions;
        private TextBox txtError;
        private Label lblErrorMessage;

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private Container components = null;

        public ErrorMessageControl() {
            InitializeComponent();

            lnkOptions.LinkClicked += linkLabelOptions_LinkClicked;
        }

        private void linkLabelOptions_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            var optionsDialog = new OptionsDialog();
            optionsDialog.ShowDialog(this);
        }
    }
}