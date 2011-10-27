using System;
using System.Windows.Forms;
using VersionOne.VisualStudio.DataLayer;
using VersionOne.VisualStudio.DataLayer.Settings;
using VersionOne.VisualStudio.VSPackage.Events;
using VersionOne.VisualStudio.VSPackage.Settings;

namespace VersionOne.VisualStudio.VSPackage.Controls {
    public partial class OptionsPageControl : UserControl {
        private readonly ISettings settings;
        private readonly IEventDispatcher eventDispatcher;

        public OptionsPageControl() {
            InitializeComponent();

            settings = SettingsImpl.Instance;
            eventDispatcher = EventDispatcher.Instance;

            btnTestConnection.Click += btnTestConnection_Click;
            chkIntegrated.CheckedChanged += chkIntegrated_CheckedChanged;
            chkUseProxy.CheckedChanged += chkUseProxy_CheckedChanged;
        }

        public void LoadSettings() {
            txtUserName.Text = settings.Username;
            txtPassword.Text = settings.Password;
            txtUrl.Text = settings.ApplicationUrl;
            chkIntegrated.Checked = settings.IntegratedAuth;
            chkUseProxy.Checked = settings.UseProxy;
            txtProxyUrl.Text = settings.ProxyUrl;
            txtProxyUsername.Text = settings.ProxyUsername;
            txtProxyPassword.Text = settings.ProxyPassword;
            txtProxyDomain.Text = settings.ProxyDomain;
        }

        public void SaveSettings() {
            settings.Username = txtUserName.Text.Trim();
            settings.Password = txtPassword.Text.Trim();
            settings.ApplicationUrl = GetUrl(txtUrl.Text);
            settings.IntegratedAuth = chkIntegrated.Checked;
            settings.UseProxy = chkUseProxy.Checked;
            settings.ProxyUrl = GetUrl(txtProxyUrl.Text);
            settings.ProxyUsername = txtProxyUsername.Text;
            settings.ProxyPassword = txtProxyPassword.Text;
            settings.ProxyDomain = txtProxyDomain.Text;

            settings.StoreSettings();

            try {
                var versionOneSettings = new VersionOneSettings {
                            Path = settings.ApplicationUrl,
                            Username = settings.Username,
                            Password = settings.Password,
                            Integrated = settings.IntegratedAuth,
                            ProxySettings = {
                                                UseProxy = settings.UseProxy,
                                                Url = settings.ProxyUrl,
                                                Domain = settings.ProxyDomain,
                                                Username = settings.ProxyUsername,
                                                Password = settings.ProxyPassword
                                            }
                        };

                ApiDataLayer.Instance.Connect(versionOneSettings);
            } catch(DataLayerException ex) {
                MessageBox.Show(string.Format("Settings are invalid or V1 server inaccessible ({0}).", ex.Message), "Verification failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            eventDispatcher.InvokeModelChanged(this, ModelChangedArgs.SettingsChanged);

            return;
        }

        private static string GetUrl(string text) {
            var url = text.Trim();

            if(!url.EndsWith("/")) {
                url += "/";
            }

            return url;
        }

        private void VerifyConnectionSettings() {
            try {
                var versionOneSettings = new VersionOneSettings {
                            Path = GetUrl(txtUrl.Text),
                            Username = txtUserName.Text.Trim(),
                            Password = txtPassword.Text.Trim(),
                            Integrated = chkIntegrated.Checked,
                            ProxySettings = {
                                                UseProxy = chkUseProxy.Checked,
                                                Url = GetUrl(txtProxyUrl.Text),
                                                Domain = txtProxyDomain.Text.Trim(),
                                                Username = txtProxyUsername.Text.Trim(),
                                                Password = txtProxyPassword.Text.Trim()
                                            }
                        };

                ApiDataLayer.Instance.CheckConnection(versionOneSettings);

                MessageBox.Show("Login Successful!", "Test Connection", MessageBoxButtons.OK, MessageBoxIcon.Information);
            } catch(DataLayerException ex) {
                MessageBox.Show(ex.Message, "Test Connection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void SetProxyRelatedFieldsEnabled(bool enabled) {
            txtProxyUrl.Enabled = txtProxyUsername.Enabled = txtProxyPassword.Enabled = txtProxyDomain.Enabled = enabled;
        }

        #region Event Handlers

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

        #endregion
    }
}