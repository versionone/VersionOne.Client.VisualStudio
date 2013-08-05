using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;

using VersionOne.VisualStudio.VSPackage.Controllers;
using VersionOne.VisualStudio.VSPackage.Controls;
using VersionOne.VisualStudio.DataLayer;
using VersionOne.VisualStudio.VSPackage.Dependencies;
using VersionOne.VisualStudio.VSPackage.Events;
using VersionOne.VisualStudio.VSPackage.Logging;
using VersionOne.VisualStudio.VSPackage.Settings;

namespace VersionOne.VisualStudio.VSPackage {
    /// <summary>
    /// This class implements the tool window exposed by this package and hosts a user control.
    ///
    /// In Visual Studio tool windows are composed of a frame (implemented by the shell) and a pane, 
    /// usually implemented by the package implementer.
    ///
    /// This class derives from the ToolWindowPane class provided from the MPF in order to use its 
    /// implementation of the IVsWindowPane interface.
    /// </summary>
    [Guid("6b9496c7-4fbe-4807-bd6b-ccfe96d183c3")]
    public class TaskWindow : ToolWindowPane, IParentWindow {
        // This is the user control hosted by the tool window; it is exposed to the base class 
        // using the Window property. Note that, even if this class implements IDispose, we are
        // not calling Dispose on this object. This is because ToolWindowPane calls Dispose on 
        // the object returned by the Window property.

        /// <summary>
        /// Standard constructor for the tool window.
        /// </summary>
        public TaskWindow() : base(null) {
            // Set the window title reading it from the resources.
            Caption = Resources.ToolWindowTitle;
            // Set the image that will appear on the tab of the window frame
            // when docked with an other window
            // The resource ID correspond to the one defined in the resx file
            // while the Index is the offset in the bitmap strip. Each image in
            // the strip being 16x16.
            BitmapResourceID = 300;
            BitmapIndex = 1;

            ServiceLocator.Instance.Container.Bind<IParentWindow>().ToConstant(this).Named("Workitems");
        }

        /// <summary>
        /// This property returns the handle to the user control that should be hosted in the Tool Window.
        /// </summary>
        override public IWin32Window Window {
            get { return ServiceLocator.Instance.Get<IUIComponentFactory>().GetWorkitemView(); }
        }

        public object GetVsService(Type serviceType) {
            return GetService(serviceType);
        }
    }
}