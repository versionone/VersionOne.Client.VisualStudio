using System;
using System.IO;
using System.Xml.Serialization;
using VersionOne.VisualStudio.DataLayer;
using VersionOne.VisualStudio.DataLayer.Logging;

namespace VersionOne.VisualStudio.VSPackage.Settings {
    [XmlRoot("Settings")]
    public class SettingsImpl : ISettings {
        private readonly IDataLayer dataLayer = ServiceLocator.Instance.Get<IDataLayer>();

        private string applicationUrl;
        private string selectedScopeToken;
        private static SettingsImpl settings;

        public string Username { get; set; }
        public string Password { get; set; }
        public bool IntegratedAuth { get; set; }

        public bool UseProxy { get; set; }
        public string ProxyUrl { get; set; }
        public string ProxyUsername { get; set; }
        public string ProxyPassword { get; set; }
        public string ProxyDomain { get; set; }

        public bool ShowMyTasks { get; set; }

        public LogLevel MinLogLevel { get; set; }

        public string ApplicationUrl {
            get {
                if(applicationUrl == null) {
                    return null;
                }

                if(!applicationUrl.EndsWith("/")) {
                    return applicationUrl + "//";
                }

                return applicationUrl;
            }
            set { applicationUrl = value; }
        }

        public string SelectedProjectId {
            get { return selectedScopeToken; }
            set {
                // TODO is any additional action forgotten here?
                if (selectedScopeToken != value) {
                    selectedScopeToken = value;
                }
            }
        }

        public void StoreSettings() {
            Save(SettingsFile);
        }
        
        public SettingsImpl() {
            selectedScopeToken = dataLayer.NullProjectToken;
        }

        private void Save(string file) {
            var dir = Path.GetDirectoryName(file);

            if(!Directory.Exists(dir)) {
                Directory.CreateDirectory(dir);
            }

            using(var fileStream = File.Create(file)) {
                Save(fileStream);
            }
        }

        private void Save(Stream stream) {
            var xmlSerializer = new XmlSerializer(typeof(SettingsImpl));
            xmlSerializer.Serialize(stream, this);
        }

        private static SettingsImpl Load(string file) {
            try {
                using(var fileStream = File.OpenRead(file)) {
                    return Load(fileStream);
                }
            } catch(Exception) {
                return new SettingsImpl();
            }
        }

        private static SettingsImpl Load(Stream stream) {
            var xmlSerializer = new XmlSerializer(typeof(SettingsImpl));
            return (SettingsImpl) xmlSerializer.Deserialize(stream);
        }

        public static ISettings Load() {
            return Load(SettingsFile);
        }

        public static string SettingsFile {
            get {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "VersionOne.VSPackage", "settings.xml");
            }
        }
    }
}