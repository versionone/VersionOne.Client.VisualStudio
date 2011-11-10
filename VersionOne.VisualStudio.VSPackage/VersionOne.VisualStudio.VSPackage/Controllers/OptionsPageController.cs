using System.Windows.Forms;
using VersionOne.VisualStudio.DataLayer;
using VersionOne.VisualStudio.DataLayer.Settings;
using VersionOne.VisualStudio.VSPackage.Controls;
using VersionOne.VisualStudio.VSPackage.Events;
using VersionOne.VisualStudio.VSPackage.Settings;

namespace VersionOne.VisualStudio.VSPackage.Controllers {
    public class OptionsPageController : BaseController {
        private IOptionsPageView view;

        public OptionsPageController(IDataLayer dataLayer, ISettings settings, IEventDispatcher eventDispatcher) : base(dataLayer, settings, eventDispatcher) { }

        public void RegisterView(IOptionsPageView view) {
            this.view = view;
            view.Controller = this;
        }

        public void PrepareView() {
            var model = SettingsImpl.Instance;
            view.Model = model;
        }

        public void UpdateData() {
            view.LoadSettings();
        }

        public void HandleSaveCommand() {
            view.UpdateModel();
            view.Model.StoreSettings();

            try {
                var versionOneSettings = CreateVersionOneSettings(view.Model);
                ApiDataLayer.Instance.Connect(versionOneSettings);
            } catch(DataLayerException ex) {
                view.ShowErrorMessage(string.Format("Settings are invalid or V1 server inaccessible ({0}).", ex.Message), "Verification failed");
            }

            EventDispatcher.InvokeModelChanged(this, ModelChangedArgs.SettingsChanged);
        }

        public void HandleVerifyConnectionCommand(ISettings settings) {
            try {
                var versionOneSettings = CreateVersionOneSettings(settings);
                ApiDataLayer.Instance.CheckConnection(versionOneSettings);
                view.ShowMessage("Login Successful!", "Test Connection");
            } catch(DataLayerException ex) {
                view.ShowErrorMessage(ex.Message, "Test Connection");
            }
        }

        private VersionOneSettings CreateVersionOneSettings(ISettings settings) {
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

            return versionOneSettings;
        }
    }
}