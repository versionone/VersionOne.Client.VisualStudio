using System;
using VersionOne.SDK.APIClient;

namespace VersionOne.VisualStudio.DataLayer {
    internal class VersionOneConnector {
        private const string MetaUrlSuffix = "meta.v1/";
        private const string LocalizerUrlSuffix = "loc.v1/";
        private const string DataUrlSuffix = "rest-1.v1/";
        private const string ConfigUrlSuffix = "config.v1/";
        
        private string path;
        private string username;
        private string password;
        private bool integrated;

        private IMetaModel metaModel;
        private IServices services;
        private ILocalizer localizer;
        private V1Configuration config;

        private bool isConnected;

        private string apiVersion = "8.3";

        public bool Integrated {
            get { return integrated; }
        }

        public string Password {
            get { return password; }
        }

        public string Username {
            get { return username; }
        }

        public string Path {
            get { return path; }
        }

        public bool IsConnected {
            get { return isConnected; }
            set { isConnected = value; }
        }

        public string ApiVersion {
            get { return apiVersion; }
            set { apiVersion = value; }
        }

        public IMetaModel MetaModel {
            get { return metaModel; }
        }

        public IServices Services {
            get { return services; }
        }

        public ILocalizer Localizer {
            get { return localizer; }
        }

        public V1Configuration V1Configuration {
            get { return config; }
        }

        public void Connect(string path, string username, string password, bool integrated) {
            this.path = path;
            this.username = username;
            this.password = password;
            this.integrated = integrated;

            V1APIConnector metaConnector = new V1APIConnector(path + MetaUrlSuffix, username, password, integrated);
            metaModel = new MetaModel(metaConnector);

            V1APIConnector localizerConnector = new V1APIConnector(path + LocalizerUrlSuffix, username, password, integrated);
            localizer = new Localizer(localizerConnector);

            V1APIConnector dataConnector = new V1APIConnector(path + DataUrlSuffix, username, password, integrated);
            services = new Services(metaModel, dataConnector);

            config = new V1Configuration(new V1APIConnector(path + ConfigUrlSuffix));
        }

        internal void CheckConnection() {
            if(!IsConnected) {
                throw ApiDataLayer.Warning("Connection is not established");
            }
        }

        public void CheckConnection(string path, string userName, string password, bool integrated) {
            V1ConnectionValidator connectionValidator = new V1ConnectionValidator(path, userName, password, integrated);
            try {
                connectionValidator.Test(ApiVersion);
            }
            catch (Exception ex) {
                throw ApiDataLayer.Warning("Cannot connect to V1 server.", ex);
            }
        }
    }
}
