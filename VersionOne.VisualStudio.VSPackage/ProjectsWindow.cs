using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;
using VersionOne.VisualStudio.DataLayer;
using VersionOne.VisualStudio.VSPackage.Dependencies;

namespace VersionOne.VisualStudio.VSPackage {
    [Guid("DE3F3B5C-86CD-4f59-A3F1-9E8BBD3F0D2C")]
    public class ProjectsWindow : ToolWindowPane, IParentWindow {
        public ProjectsWindow() : base(null) {
            Caption = Resources.ProjectsWindowTitle;
            BitmapResourceID = 300;
            BitmapIndex = 1;

            ServiceLocator.Instance.Container.Bind<IParentWindow>().ToConstant(this).Named("Projects");
        }

        public override IWin32Window Window {
            get { return ServiceLocator.Instance.Get<IUIComponentFactory>().GetProjectView(); }
        }

        public object GetVsService(Type serviceType) {
            return GetService(serviceType);
        }
    }
}