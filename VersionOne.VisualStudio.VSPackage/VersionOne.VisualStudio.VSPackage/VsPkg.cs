// TODO move required code from here to V1Tracker class.

//using System;
//using System.Diagnostics;
//using System.Globalization;
//using System.Reflection;
//using System.Runtime.InteropServices;
//using System.ComponentModel.Design;

//using Microsoft.VisualStudio;
//using Microsoft.VisualStudio.Shell.Interop;
//using Microsoft.VisualStudio.Shell;

//using VersionOne.VisualStudio.VSPackage.DataLayer;
//using VersionOne.VisualStudio.VSPackage.Events;
//using VersionOne.VisualStudio.VSPackage.Settings;

//namespace VersionOne.VisualStudio.VSPackage {
//    /// <summary>
//    /// This is the class that implements the package exposed by this assembly.
//    ///
//    /// The minimum requirement for a class to be considered a valid package for Visual Studio
//    /// is to implement the IVsPackage interface and register itself with the shell.
//    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
//    /// to do it: it derives from the Package class that provides the implementation of the 
//    /// IVsPackage interface and uses the registration attributes defined in the framework to 
//    /// register itself and its components with the shell.
//    /// </summary>
//    // This attribute tells the registration utility (regpkg.exe) that this class needs
//    // to be registered as package.
//    [PackageRegistration(UseManagedResourcesOnly = true)]
//    // A Visual Studio component can be registered under different regitry roots; for instance
//    // when you debug your package you want to register it in the experimental hive. This
//    // attribute specifies the registry root to use if no one is provided to regpkg.exe with
//    // the /root switch.
//    [DefaultRegistryRoot("Software\\Microsoft\\VisualStudio\\8.0")]
//    // This attribute is used to register the informations needed to show the this package
//    // in the Help/About dialog of Visual Studio.
//    [InstalledProductRegistration(true, "#110", "#112", "1.0", IconResourceID = 400)]
//    // In order be loaded inside Visual Studio in a machine that has not the VS SDK installed, 
//    // package needs to have a valid load key (it can be requested at 
//    // http://msdn.microsoft.com/vstudio/extend/). This attributes tells the shell that this 
//    // package has a load key embedded in its resources.
//    [ProvideLoadKey("Standard", "1.0", "VersionOne.VSPackage", "VersionOne", 525)]
//    // This attribute is needed to let the shell know that this package exposes some menus.
//    [ProvideMenuResource(1000, 1)]
//    [ProvideOptionPageAttribute(typeof(OptionsPage), "VersionOne", "Settings", 101, 107, true)]
//    // This attribute registers a tool window exposed by this package.
//    [ProvideToolWindow(typeof(TaskWindow))]
//    [ProvideToolWindow(typeof(ProjectsWindow))]
//    [Guid(GuidList.guidVersionOne_VSPackagePkgString)]
//    public sealed class VersionOne_VSPackage : Package, IVsInstalledProduct {
//        /// <summary>
//        /// Default constructor of the package.
//        /// Inside this method you can place any initialization code that does not require 
//        /// any Visual Studio service because at this point the package object is created but 
//        /// not sited yet inside Visual Studio environment. The place to do all the other 
//        /// initialization is the Initialize method.
//        /// </summary>
//        public VersionOne_VSPackage() {
//            Configuration cfg = Configuration.Instance;
//            ISettings settings = SettingsImpl.Instance;
//            IDataLayer dataLayer = ApiDataLayer.Instance;
//            IEventDispatcher eventDispatcher = EventDispatcher.Instance;
//            try {
//                //Setup DataLayer
//                dataLayer.ApiVersion = cfg.APIVersion;
//                AddProperties(cfg);

//                dataLayer.CurrentProjectId = settings.SelectedProjectId;
//                dataLayer.ShowAllTasks = !settings.ShowMyTasks;
//                dataLayer.Connect(settings.ApplicationUrl, settings.Username, settings.Password, settings.IntegratedAuth);
//                eventDispatcher.InvokeModelChanged(this, ModelChangedArgs.SettingsChanged);
//            } catch (DataLayerException ex) {
//                Debug.WriteLine("Error while loading V1Package: " + ex.Message);
//                Debug.WriteLine("\t" + ex.StackTrace);
//            }
//            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this.ToString()));
//        }

//        private static void AddProperties(Configuration cfg) {
//            foreach (ColumnSetting column in cfg.AssetDetail.TaskColumns) {
//                AddProperty(column, Entity.TaskPrefix);
//            }
//            foreach (ColumnSetting column in cfg.AssetDetail.StoryColumns) {
//                AddProperty(column, Entity.StoryPrefix);
//            }
//            foreach (ColumnSetting column in cfg.AssetDetail.DefectColumns) {
//                AddProperty(column, Entity.DefectPrefix);
//            }
//            foreach (ColumnSetting column in cfg.AssetDetail.TestColumns) {
//                AddProperty(column, Entity.TestPrefix);
//            }
//            foreach (ColumnSetting column in cfg.GridSettings.Columns) {
//                AddProperty(column, Entity.TaskPrefix);
//                AddProperty(column, Entity.StoryPrefix);
//                AddProperty(column, Entity.DefectPrefix);
//                AddProperty(column, Entity.TestPrefix);
//            }
//            foreach (ColumnSetting column in cfg.ProjectTree.Columns) {
//                AddProperty(column, Entity.ProjectPrefix);
//            }
//        }

//        private static void AddProperty(ColumnSetting column, string prefix) {
//            ApiDataLayer.Instance.AddProperty(column.Attribute, prefix, column.Type == "List" || column.Type == "Multi");
//        }

//        /// <summary>
//        /// This function is called when the user clicks the menu item that shows the 
//        /// tool window. See the Initialize method to see how the menu item is associated to 
//        /// this function using the OleMenuCommandService service and the MenuCommand class.
//        /// </summary>
//        private void ShowToolWindow(object sender, EventArgs e) {
//            // Get the instance number 0 of this tool window. This window is single instance so this instance
//            // is actually the only one.
//            // The last flag is set to true so that if the tool window does not exists it will be created.
//            ToolWindowPane window = FindToolWindow(typeof(TaskWindow), 0, true);
//            if ((null == window) || (null == window.Frame)) {
//                throw new COMException(Resources.CanNotCreateWindow);
//            }
//            IVsWindowFrame windowFrame = (IVsWindowFrame)window.Frame;
//            ErrorHandler.ThrowOnFailure(windowFrame.Show());
//        }

//        private void ShowProjectWindow(object sender, EventArgs e) {
//            // Get the instance number 0 of this tool window. This window is single instance so this instance
//            // is actually the only one.
//            // The last flag is set to true so that if the tool window does not exists it will be created.
//            ToolWindowPane window = FindToolWindow(typeof(ProjectsWindow), 0, true);
//            if ((null == window) || (null == window.Frame)) {
//                throw new COMException(Resources.CanNotCreateWindow);
//            }
//            IVsWindowFrame windowFrame = (IVsWindowFrame)window.Frame;
//            ErrorHandler.ThrowOnFailure(windowFrame.Show());
//        }

//        /////////////////////////////////////////////////////////////////////////////
//        // Overriden Package Implementation

//        #region Package Members
//        /// <summary>
//        /// Initialization of the package; this method is called right after the package is sited, so this is the place
//        /// where you can put all the initilaization code that rely on services provided by VisualStudio.
//        /// </summary>
//        protected override void Initialize() {
//            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
//            base.Initialize();

//            // Add our command handlers for menu (commands must exist in the .ctc file)
//            OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
//            if (null != mcs) {
//                // Create the command for the tool window
//                CommandID toolwndCommandID = new CommandID(GuidList.guidVersionOne_VSPackageCmdSet, (int)PkgCmdIDList.cmdidV1Tasks);
//                MenuCommand menuToolWin = new MenuCommand(ShowToolWindow, toolwndCommandID);
//                mcs.AddCommand(menuToolWin);

//                // Create the 'Projects List' command for the tool window
//                CommandID toolwndCommandIDPrj = new CommandID(GuidList.guidVersionOne_VSPackageCmdSet, (int)PkgCmdIDList.cmdidV1Project);
//                MenuCommand menuPrjWin = new MenuCommand(ShowProjectWindow, toolwndCommandIDPrj);
//                mcs.AddCommand(menuPrjWin);
//            }
//        }
//        #endregion

//        #region IVsInstalledProduct Members
//        public int IdBmpSplash(out uint pIdBmp) {
//            pIdBmp = 400;
//            return VSConstants.S_OK;
//        }

//        public int OfficialName(out string pbstrName) {
//            pbstrName = "VersionOne Tracker";
//            return VSConstants.S_OK;
//        }

//        public int ProductID(out string pbstrPID) {
//            pbstrPID = "(" + Assembly.GetExecutingAssembly().GetName().Version + ")";
//            return VSConstants.S_OK;
//        }

//        public int ProductDetails(out string pbstrProductDetails) {
//            pbstrProductDetails = "VersionOne Package for Microsoft Visual Studio. For more information about VersionOne, see the VersionOne website at http://www.versionone.com.";
//            return VSConstants.S_OK;
//        }

//        public int IdIcoLogoForAboutbox(out uint pIdIco) {
//            pIdIco = 400;
//            return VSConstants.S_OK;
//        }
//        #endregion
//    }
//}