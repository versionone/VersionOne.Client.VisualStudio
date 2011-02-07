namespace VersionOne.VisualStudio.VSPackage.Settings {
    public class VersionOneSettings {
        public bool Integrated { get; set; }
        public string Path { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public ProxyConnectionSettings ProxySettings { get; set; }

        VersionOneSettings() {
            ProxySettings = new ProxyConnectionSettings();
        }
    }
}
