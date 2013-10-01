using System.IO;
using System.Reflection;
using System.Xml.Serialization;

namespace VersionOne.VisualStudio.VSPackage.Settings {
    [XmlRoot("Configuration")]
    public class Configuration {
        private GridSettings gridSettings = new GridSettings();
        private ProjectTreeSettings projectTreeSettings = new ProjectTreeSettings();
        private AssetDetailSettings assetDetailSettings = new AssetDetailSettings();
        private string apiVersion = "8.3";

        private void Save(Stream stream) {
            var serializer = new XmlSerializer(typeof (Configuration));
            serializer.Serialize(stream, this);
        }

        private static Configuration Load(Stream stream) {
            var serializer = new XmlSerializer(typeof (Configuration));
            return (Configuration) serializer.Deserialize(stream);
        }

        public GridSettings GridSettings {
            get { return gridSettings; }
            set { gridSettings = value; }
        }

        public ProjectTreeSettings ProjectTree {
            get { return projectTreeSettings; }
            set { projectTreeSettings = value; }
        }

        public AssetDetailSettings AssetDetail {
            get { return assetDetailSettings; }
            set { assetDetailSettings = value; }
        }

        public string APIVersion {
            get { return apiVersion; }
            set { apiVersion = value; }

        }

        private static string ConfigurationFile {
            get {
                var info = new DirectoryInfo(new System.Uri(Assembly.GetCallingAssembly().CodeBase).LocalPath);
                return info.Parent.FullName + "\\configuration.xml";
            }
        }

        public static Configuration Load() {
            using (var configurationFile = File.OpenRead(ConfigurationFile)) {
                return Load(configurationFile);
            }
        }
    }
}