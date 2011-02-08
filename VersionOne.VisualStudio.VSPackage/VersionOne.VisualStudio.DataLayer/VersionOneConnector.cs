using System;
using VersionOne.SDK.APIClient;
using VersionOne.VisualStudio.DataLayer.Settings;

namespace VersionOne.VisualStudio.DataLayer {
    internal class VersionOneConnector {
        private const string MetaUrlSuffix = "meta.v1/";
        private const string LocalizerUrlSuffix = "loc.v1/";
        private const string DataUrlSuffix = "rest-1.v1/";
        private const string ConfigUrlSuffix = "config.v1/";

        private VersionOneSettings versionOneSettings = new VersionOneSettings();
        private string apiVersion = "8.3";

        public string ApiVersion {
            get { return apiVersion; }
            set { apiVersion = value; }
        }

        public IMetaModel MetaModel { get; private set; }
        public IServices Services { get; private set; }
        public ILocalizer Localizer { get; private set; }
        public V1Configuration V1Configuration { get; private set; }
        public bool IsConnected { get; set; }


        public void Connect(VersionOneSettings settings) {
            var path = settings.Path;
            var username = settings.Username;
            var password = settings.Password;
            var integrated = settings.Integrated;
            var proxy = GetProxy(settings.ProxySettings);

            var metaConnector = new V1APIConnector(path + MetaUrlSuffix, username, password, integrated, proxy);
            MetaModel = new MetaModel(metaConnector);

            var localizerConnector = new V1APIConnector(path + LocalizerUrlSuffix, username, password, integrated, proxy);
            Localizer = new Localizer(localizerConnector);

            var dataConnector = new V1APIConnector(path + DataUrlSuffix, username, password, integrated, proxy);
            Services = new Services(MetaModel, dataConnector);

            V1Configuration = new V1Configuration(new V1APIConnector(path + ConfigUrlSuffix, null, null, integrated, proxy));

            versionOneSettings = settings;
        }

        private ProxyProvider GetProxy(ProxyConnectionSettings settings) {
            if(settings == null || !settings.UseProxy) {
                return null;
            }

            var uri = new Uri(settings.Url);
            return new ProxyProvider(uri, settings.Username, settings.Password, settings.Domain);
        }

        internal void CheckConnection() {
            if(!IsConnected) {
                Logger.Error("Connection is not established");
            }
        }

        public void CheckConnection(VersionOneSettings settings) {
            var connectionValidator = new V1ConnectionValidator(settings.Path, settings.Username,
                                                                settings.Password, settings.Integrated,
                                                                GetProxy(settings.ProxySettings));
            try {
                connectionValidator.Test(ApiVersion);
            } catch(Exception ex) {
                Logger.Error("Cannot connect to V1 server.", ex);
            }
        }

        public void Reconnect() {
            Connect(versionOneSettings);
        }
    }
}
