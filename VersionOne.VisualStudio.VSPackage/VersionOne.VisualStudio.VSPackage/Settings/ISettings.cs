namespace VersionOne.VisualStudio.VSPackage.Settings {
    public interface ISettings {
        string Username { get; set; }
        string Password { get; set; }
        bool IntegratedAuth { get; set; }
        string ApplicationUrl { get; set; }
        string SelectedProjectId { get; set; }
        bool ShowMyTasks { get; set; }
        void StoreSettings();
        bool IsDifferent(string url, string userName, string password, bool integrated);
    }
}