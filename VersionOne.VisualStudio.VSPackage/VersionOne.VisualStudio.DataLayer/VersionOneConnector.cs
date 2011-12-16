using System;
using VersionOne.SDK.APIClient;
using VersionOne.VisualStudio.DataLayer.Settings;

namespace VersionOne.VisualStudio.DataLayer {
    internal class VersionOneConnector {
        private const string MetaUrlSuffix = "meta.v1/";
        private const string LocalizerUrlSuffix = "loc.v1/";
        private const string DataUrlSuffix = "rest-1.v1/";
        private const string ConfigUrlSuffix = "config.v1/";

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

        public VersionOneSettings VersionOneSettings { get; private set; }

        public void Connect(VersionOneSettings settings) {
            var path = settings.Path;
            var username = settings.Username;
            var password = settings.Password;
            var integrated = settings.Integrated;
            var proxy = GetProxy(settings.ProxySettings);
            VersionOneSettings = settings;

            var metaConnector = new V1APIConnector(path + MetaUrlSuffix, username, password, integrated, proxy);
            MetaModel = new MetaModel(metaConnector);

            var localizerConnector = new V1APIConnector(path + LocalizerUrlSuffix, username, password, integrated, proxy);
            Localizer = new Localizer(localizerConnector);

            var dataConnector = new V1APIConnector(path + DataUrlSuffix, username, password, integrated, proxy);
            Services = new Services(MetaModel, dataConnector);                        

            LoadV1Configuration();
        }

        public void LoadV1Configuration() {
            if (VersionOneSettings == null) {
                throw new InvalidOperationException("Connection is needed for configuration loading.");
            }

            var path = VersionOneSettings.Path;
            var integrated = VersionOneSettings.Integrated;
            var proxy = GetProxy(VersionOneSettings.ProxySettings);

            V1Configuration = new V1Configuration(new V1APIConnector(path + ConfigUrlSuffix, null, null, integrated, proxy));
        }

        private static ProxyProvider GetProxy(ProxyConnectionSettings settings) {
            if(settings == null || !settings.UseProxy) {
                return null;
            }

            var uri = new Uri(settings.Url);
            return new ProxyProvider(uri, settings.Username, settings.Password, settings.Domain);
        }

        public void CheckConnection(VersionOneSettings settings) {
            var connectionValidator = new V1ConnectionValidator(settings.Path, settings.Username, settings.Password, settings.Integrated, GetProxy(settings.ProxySettings));
            connectionValidator.Test(ApiVersion);
        }
    }
}