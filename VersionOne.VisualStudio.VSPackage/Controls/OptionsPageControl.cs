using System;
using System.Windows.Forms;
using VersionOne.VisualStudio.VSPackage.Controllers;
using VersionOne.VisualStudio.VSPackage.Settings;

using LogLevel = VersionOne.VisualStudio.DataLayer.Logging.LogLevel;

namespace VersionOne.VisualStudio.VSPackage.Controls {
    public partial class OptionsPageControl : UserControl, IOptionsPageView {
        public ISettings Model { get; set; }
        public OptionsPageController Controller { get; set; }

        public OptionsPageControl() {
            InitializeComponent();

            btnTestConnection.Click += btnTestConnection_Click;
            chkIntegrated.CheckedChanged += chkIntegrated_CheckedChanged;
            chkUseProxy.CheckedChanged += chkUseProxy_CheckedChanged;

            foreach (Control control in pnlContent.Controls) {
                control.GotFocus += (sender, e) => pnlContent.ScrollControlIntoView(sender as Control ?? ActiveControl);
            }
        }

        public void LoadSettings() {
            txtUserName.Text = Model.Username;
            txtPassword.Text = Model.Password;
            txtUrl.Text = Model.ApplicationUrl;
            chkIntegrated.Checked = Model.IntegratedAuth;
            chkUseProxy.Checked = Model.UseProxy;
            txtProxyUrl.Text = Model.ProxyUrl;
            txtProxyUsername.Text = Model.ProxyUsername;
            txtProxyPassword.Text = Model.ProxyPassword;
            txtProxyDomain.Text = Model.ProxyDomain;
            cboMinLogLevel.DataSource = Enum.GetValues(typeof (LogLevel));
            cboMinLogLevel.SelectedItem = Model.MinLogLevel;

            SetProxyRelatedFieldsEnabled(Model.UseProxy);
        }

        public void UpdateModel() {
            UpdateModel(Model);
        }

        private void UpdateModel(ISettings model) {
            model.Username = txtUserName.Text.Trim();
            model.Password = txtPassword.Text.Trim();
            model.ApplicationUrl = GetUrl(txtUrl.Text);
            model.IntegratedAuth = chkIntegrated.Checked;
            model.UseProxy = chkUseProxy.Checked;
            model.ProxyUrl = GetUrl(txtProxyUrl.Text);
            model.ProxyUsername = txtProxyUsername.Text;
            model.ProxyPassword = txtProxyPassword.Text;
            model.ProxyDomain = txtProxyDomain.Text;

            LogLevel selectedLogLevel;

            model.MinLogLevel = Enum.TryParse(cboMinLogLevel.SelectedItem.ToString(), out selectedLogLevel)
                                    ? selectedLogLevel
                                    : LogLevel.Debug;
        }

        private static string GetUrl(string text) {
            var url = text.Trim();

            if(!string.IsNullOrEmpty(url) && !url.EndsWith("/")) {
                url += "/";
            }

            return url;
        }

        private void VerifyConnectionSettings() {
            if (!UrlIsValid(txtUrl.Text)) {
                MessageBox.Show("Application URL is not valid.", "Test Connection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (chkUseProxy.Checked && !UrlIsValid(txtProxyUrl.Text)) {
                MessageBox.Show("Proxy server URL is not valid.", "Test Connection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var model = new SettingsImpl();
            UpdateModel(model);
            Controller.HandleVerifyConnectionCommand(model);
        }


        private static bool UrlIsValid(string url) {
            try {
                new Uri(url);
                return true;
            } catch(Exception) {
                return false;
            }
        }

        private void SetProxyRelatedFieldsEnabled(bool enabled) {
            txtProxyUrl.Enabled = txtProxyUsername.Enabled = txtProxyPassword.Enabled = txtProxyDomain.Enabled = enabled;
        }

        private void btnTestConnection_Click(object sender, EventArgs e) {
            VerifyConnectionSettings();
        }

        private void chkIntegrated_CheckedChanged(object sender, EventArgs e) {
            var credentialInputEnabled = !chkIntegrated.Checked;
            txtUserName.Enabled = txtPassword.Enabled = credentialInputEnabled;
        }

        private void chkUseProxy_CheckedChanged(object sender, EventArgs e) {
            SetProxyRelatedFieldsEnabled(chkUseProxy.Checked);
        }

        public void ShowErrorMessage(string message, string caption) {
            MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public void ShowMessage(string message, string caption) {
            MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}