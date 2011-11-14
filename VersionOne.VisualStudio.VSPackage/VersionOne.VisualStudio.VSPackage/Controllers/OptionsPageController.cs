using VersionOne.VisualStudio.DataLayer;
using VersionOne.VisualStudio.DataLayer.Settings;
using VersionOne.VisualStudio.VSPackage.Controls;
using VersionOne.VisualStudio.VSPackage.Events;
using VersionOne.VisualStudio.VSPackage.Settings;

namespace VersionOne.VisualStudio.VSPackage.Controllers {
    public class OptionsPageController : BaseController {
        private IOptionsPageView view;

        protected override EventReceiver ReceiverType { get { return EventReceiver.OptionsView; } }

        public OptionsPageController(IDataLayer dataLayer, ISettings settings, IEventDispatcher eventDispatcher) : base(dataLayer, settings, eventDispatcher) { }

        public void RegisterView(IOptionsPageView view) {
            this.view = view;
            view.Controller = this;
        }

        public void PrepareView() {
            view.Model = Settings;
        }

        protected override void HandleModelChanged(object sender, ModelChangedArgs e) {
            var versionOneSettings = CreateVersionOneSettings(view.Model);
            DataLayer.Connect(versionOneSettings);
            EventDispatcher.Notify(this, new ModelChangedArgs(EventReceiver.ProjectView, EventContext.ProjectsRequested));
        }

        public void UpdateData() {
            view.LoadSettings();
        }

        public void HandleSaveCommand() {
            view.UpdateModel();
            view.Model.StoreSettings();

            try {
                EventDispatcher.Notify(this, new ModelChangedArgs(EventReceiver.OptionsView, EventContext.V1SettingsChanged));
            } catch(DataLayerException ex) {
                view.ShowErrorMessage(string.Format("Settings are invalid or V1 server inaccessible ({0}).", ex.Message), "Verification failed");
            }
        }

        public void HandleVerifyConnectionCommand(ISettings settings) {
            try {
                var versionOneSettings = CreateVersionOneSettings(settings);
                DataLayer.CheckConnection(versionOneSettings);
                view.ShowMessage("Login Successful!", "Test Connection");
            } catch(DataLayerException ex) {
                view.ShowErrorMessage(ex.Message, "Test Connection");
            }
        }

        private static VersionOneSettings CreateVersionOneSettings(ISettings settings) {
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