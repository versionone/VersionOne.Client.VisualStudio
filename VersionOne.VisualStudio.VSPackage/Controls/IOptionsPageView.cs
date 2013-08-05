using VersionOne.VisualStudio.VSPackage.Controllers;
using VersionOne.VisualStudio.VSPackage.Settings;

namespace VersionOne.VisualStudio.VSPackage.Controls {
    public interface IOptionsPageView {
        OptionsPageController Controller { get; set; }
        ISettings Model { get; set; }
        void LoadSettings();
        void ShowErrorMessage(string message, string caption);
        void ShowMessage(string message, string caption);
        void UpdateModel();
    }
}