using System;
using System.Windows.Forms;

namespace VersionOne.VisualStudio.VSPackage.Forms {
    public partial class OptionsDialog : Form {
        public event EventHandler SettingsChanged;
        
        public OptionsDialog() {
            InitializeComponent();
            optionsPage.LoadSettings();

            btnOk.Click += btnOk_Click;
        }

        private void btnOk_Click(object sender, EventArgs e) {
            bool settingsModified = optionsPage.SaveSettings();
            
            if(settingsModified && SettingsChanged != null) {
                SettingsChanged(this, EventArgs.Empty);
            } 
            
            Close();
        }
    }
}