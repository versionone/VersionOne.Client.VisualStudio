namespace VersionOne.VisualStudio.DataLayer.Settings {
	public class VersionOneSettings
	{
		public string Path { get; set; }

		public string Username { get; set; }
		public string Password { get; set; }

        public bool Integrated { get; set; }

		public bool OAuth2 { get; set; }
		public string SecretsFile { get; set; }
		public string CredsFile { get; set; }

        public ProxyConnectionSettings ProxySettings { get; set; }

        public VersionOneSettings()
        {
	        OAuth2 = true;
            ProxySettings = new ProxyConnectionSettings();
        }
    }
}