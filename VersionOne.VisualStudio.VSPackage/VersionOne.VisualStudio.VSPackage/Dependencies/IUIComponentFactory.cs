using System.Windows.Forms;

namespace VersionOne.VisualStudio.VSPackage.Dependencies {
    internal interface IUIComponentFactory {
        Control GetWorkitemView();
        Control GetProjectView();
    }
}