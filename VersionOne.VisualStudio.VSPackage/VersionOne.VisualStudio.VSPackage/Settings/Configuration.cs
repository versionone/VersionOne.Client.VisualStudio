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
        private static Configuration configuration;

        private void Save(Stream stream) {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Configuration));

            xmlSerializer.Serialize(stream, this);
        }

        private static Configuration Load(Stream stream) {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Configuration));

            return (Configuration)xmlSerializer.Deserialize(stream);
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
                DirectoryInfo info = new DirectoryInfo(Assembly.GetCallingAssembly().Location);
                return info.Parent.FullName + "\\configuration.xml";
            }
        }

        public static Configuration Instance {
            get {
                if (configuration == null) {
                    using (FileStream configurationFile = File.OpenRead(ConfigurationFile)) {
                        configuration = Load(configurationFile);
                    }
                }
                return configuration;
            }
        }
    }
}