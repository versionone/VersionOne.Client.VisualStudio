using System;
using System.Windows.Forms;
using VersionOne.VisualStudio.DataLayer;
using VersionOne.VisualStudio.VSPackage.Events;
using VersionOne.VisualStudio.VSPackage.Settings;

namespace VersionOne.VisualStudio.VSPackage.Controls {    
    public partial class OptionsPageControl : UserControl {
        private readonly ISettings settings;
        private readonly IEventDispatcher eventDispatcher;

        public OptionsPageControl() {
            InitializeComponent();

            if(!DesignMode) {
                settings = SettingsImpl.Instance;
                eventDispatcher = EventDispatcher.Instance;
            }

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

        /// <summary>
        /// Save settings.
        /// </summary>
        /// <returns>bool, indicating whether saved settings were different.</returns>
        public bool SaveSettings() {
            string url = txtUrl.Text.Trim();

            if (!url.EndsWith("/")) {
                url += "/";
            }
            //if (settings.IsDifferent(url, txtUserName.Text.Trim(), txtPassword.Text.Trim(), chkIntegrated.Checked)) {
                settings.Username = txtUserName.Text.Trim();
                settings.Password = txtPassword.Text.Trim();

                settings.ApplicationUrl = url;
                settings.IntegratedAuth = chkIntegrated.Checked;

                settings.StoreSettings();
                IDataLayer dataLayer = ApiDataLayer.Instance;
                
                try {
                    dataLayer.Connect(url, settings.Username, settings.Password, settings.IntegratedAuth);
                } catch (DataLayerException ex) {
                    MessageBox.Show(string.Format("Settings are invalid or V1 server inaccessible ({0}).", ex.Message),
                                    "Verification failed",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                eventDispatcher.InvokeModelChanged(this, ModelChangedArgs.SettingsChanged);

                return true;
            //}
        }

        private void VerifyConnectionSettings() {
            try {
                ApiDataLayer.Instance.CheckConnection(txtUrl.Text.Trim(), txtUserName.Text.Trim(), txtPassword.Text.Trim(), chkIntegrated.Checked);
                MessageBox.Show("Login Successful!", "Test Connection", MessageBoxButtons.OK, MessageBoxIcon.Information);
            } catch (DataLayerException ex) {
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
            bool credentialInputEnabled = !chkIntegrated.Checked;
            txtUserName.Enabled = txtPassword.Enabled = credentialInputEnabled;
        }

        private void chkUseProxy_CheckedChanged(object sender, EventArgs e) {
            SetProxyRelatedFieldsEnabled(chkUseProxy.Checked);
        }

        #endregion
    }
}