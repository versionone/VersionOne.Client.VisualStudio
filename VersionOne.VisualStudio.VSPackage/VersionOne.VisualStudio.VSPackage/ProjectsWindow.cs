using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;

using VersionOne.VisualStudio.VSPackage.Controllers;
using VersionOne.VisualStudio.VSPackage.Controls;
using VersionOne.VisualStudio.DataLayer;
using VersionOne.VisualStudio.VSPackage.Events;
using VersionOne.VisualStudio.VSPackage.Logging;
using VersionOne.VisualStudio.VSPackage.Settings;

namespace VersionOne.VisualStudio.VSPackage {
    [Guid("DE3F3B5C-86CD-4f59-A3F1-9E8BBD3F0D2C")]
    public class ProjectsWindow : ToolWindowPane, IParentWindow {
        private readonly ProjectTreeControl control;
        private readonly ProjectTreeController controller;

        public ProjectsWindow() : base(null) {
            Caption = Resources.ProjectsWindowTitle;
            BitmapResourceID = 300;
            BitmapIndex = 1;

            controller = new ProjectTreeController(LoggerFactory.Instance, ApiDataLayer.Instance, SettingsImpl.Instance, EventDispatcher.Instance);
            control = new ProjectTreeControl(this);
            controller.RegisterView(control);
            controller.PrepareView();
            controller.Prepare();
        }

        override public IWin32Window Window {
            get { return control; }
        }

        public object GetVsService(Type serviceType) {
            return GetService(serviceType);
        }
    }
}