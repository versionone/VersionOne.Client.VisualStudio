using System;
using System.IO;
using System.Xml.Serialization;

// TODO resolve by getting non-APIClient types from DL
// using VersionOne.SDK.APIClient;

namespace VersionOne.VisualStudio.VSPackage.Settings {
    [XmlRoot("Settings")]
    public class SettingsImpl : ISettings {
        private string username;
        private string password;
        private string applicationUrl;
        
        // TODO APIClient types usage in DL only. The following uncommented code would not work but would compile.
        //private string selectedScopeToken = Oid.Null.Token;
        private string selectedScopeToken = "Oid.Null.Token";
        
        private bool integratedAuth;
        private static SettingsImpl settings;
        private bool showMyTasks;

        public string Username {
            get { return username; }
            set { username = value; }
        }

        public string Password {
            get { return password; }
            set { password = value; }
        }

        public bool IntegratedAuth {
            get { return integratedAuth; }
            set { integratedAuth = value; }
        }

        public string ApplicationUrl {
            get {
                if (applicationUrl == null)
                    return null;

                if (!applicationUrl.EndsWith("/"))
                    return applicationUrl + "//";

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

        private void Save(string file) {
            string dir = Path.GetDirectoryName(file);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            using (FileStream fileStream = File.Create(file))
                Save(fileStream);
        }

        private void Save(Stream stream) {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(SettingsImpl));
            xmlSerializer.Serialize(stream, this);
        }

        public static SettingsImpl Load(string file) {
            try {
                using (FileStream fileStream = File.OpenRead(file))
                    return Load(fileStream);
            }
            catch (Exception) {
                return new SettingsImpl();
            }
        }

        private static SettingsImpl Load(Stream stream) {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(SettingsImpl));
            return (SettingsImpl)xmlSerializer.Deserialize(stream);
        }

        public static string SettingsFile {
            get {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "VersionOne.VSPackage\\settings.xml");
            }
        }

        public static ISettings Instance {
            get {
                return settings ?? (settings = Load(SettingsFile));
            }
        }

        public bool ShowMyTasks {
            get { return showMyTasks; }
            set { showMyTasks = value; }
        }

        public void StoreSettings() {
            Save(SettingsFile);
        }

        public bool IsDifferent(string url, string userName, string password, bool integrated) {
            return url != ApplicationUrl || userName != Username || password != Password || IntegratedAuth != integrated;
        }
    }
}