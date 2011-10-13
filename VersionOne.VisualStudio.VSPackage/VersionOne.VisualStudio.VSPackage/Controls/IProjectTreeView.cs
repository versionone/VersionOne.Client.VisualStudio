using VersionOne.VisualStudio.VSPackage.Controllers;

namespace VersionOne.VisualStudio.VSPackage.Controls {
    public interface IProjectTreeView : IWaitCursorProvider {
        void RefreshProperties();
        void UpdateData();

        ProjectTreeController Controller { get; set; }
    }
}