using VersionOne.SDK.APIClient;
using VersionOne.VisualStudio.DataLayer.Settings;

namespace VersionOne.VisualStudio.DataLayer {
    public interface IVersionOneConnector {
        string ApiVersion { get; set; }
        IMetaModel MetaModel { get; }
        IServices Services { get; }
        ILocalizer Localizer { get; }
        IV1Configuration V1Configuration { get; }
        bool IsConnected { get; set; }
        VersionOneSettings VersionOneSettings { get; }
        void Connect(VersionOneSettings settings);
        IV1Configuration LoadV1Configuration();
        void CheckConnection(VersionOneSettings settings);
    }
}