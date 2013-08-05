using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using Ninject;
using VersionOne.VisualStudio.DataLayer;
using VersionOne.VisualStudio.DataLayer.Entities;
using VersionOne.VisualStudio.DataLayer.Logging;
using VersionOne.VisualStudio.DataLayer.Settings;
using VersionOne.VisualStudio.VSPackage.Controllers;
using VersionOne.VisualStudio.VSPackage.Controls;
using VersionOne.VisualStudio.VSPackage.Dependencies;
using VersionOne.VisualStudio.VSPackage.Events;
using VersionOne.VisualStudio.VSPackage.Logging;
using VersionOne.VisualStudio.VSPackage.Settings;

namespace VersionOne.VisualStudio.VSPackage {
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    ///
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the 
    /// IVsPackage interface and uses the registration attributes defined in the framework to 
    /// register itself and its components with the shell.
    /// </summary>
    // This attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class is
    // a package.
    [PackageRegistration(UseManagedResourcesOnly = true)]
    // This attribute is used to register the informations needed to show the this package
    // in the Help/About dialog of Visual Studio.
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    // This attribute is needed to let the shell know that this package exposes some menus.
    [ProvideMenuResource("Menus.ctmenu", 1)]
    // In order be loaded inside Visual Studio in a machine that has not the VS SDK installed, 
    // package needs to have a valid load key (it can be requested at 
    // http://msdn.microsoft.com/vstudio/extend/). This attributes tells the shell that this 
    // package has a load key embedded in its resources.
    [ProvideLoadKey("Standard", "1.0", "VersionOne Tracker", "VersionOne", 525)]
    // Register Options page
    [ProvideOptionPage(typeof(OptionsPage), "VersionOne", "Settings", 101, 107, true)]
    // Register Task and project tool windows exposed by this package.
    [ProvideToolWindow(typeof(ProjectsWindow))]
    [ProvideToolWindow(typeof(TaskWindow))]
    [Guid(GuidList.guidVersionOnTrackerPkgString)]
    public sealed class V1Tracker : Package {
        private readonly ApiDataLayer dataLayer;

        /// <summary>
        /// Default constructor of the package.
        /// Inside this method you can place any initialization code that does not require 
        /// any Visual Studio service because at this point the package object is created but 
        /// not sited yet inside Visual Studio environment. The place to do all the other 
        /// initialization is the Initialize method.
        /// </summary>
        public V1Tracker() {
            ServiceLocator.Instance.SetContainer(new StandardKernel());

            dataLayer = new ApiDataLayer();            
            RegisterComponents();

            var cfg = ServiceLocator.Instance.Get<Configuration>();
            var settings = ServiceLocator.Instance.Get<ISettings>();
            var eventDispatcher = ServiceLocator.Instance.Get<IEventDispatcher>();
            var loggerFactory = ServiceLocator.Instance.Get<ILoggerFactory>();

            loggerFactory.MinLogLevel = settings.MinLogLevel;

            var logger = loggerFactory.GetLogger("V1Tracker");

            try {
                //Setup DataLayer
                dataLayer.LoggerFactory = loggerFactory;
                dataLayer.ApiVersion = cfg.APIVersion;
                AddProperties(cfg);

                dataLayer.CurrentProjectId = settings.SelectedProjectId;
                dataLayer.ShowAllTasks = !settings.ShowMyTasks;

                var versionOneSettings = new VersionOneSettings {
                            Path = settings.ApplicationUrl,
                            Username = settings.Username,
                            Password = settings.Password,
                            Integrated = settings.IntegratedAuth,
                            ProxySettings = {
                                                UseProxy = settings.UseProxy,
                                                Url = settings.ProxyUrl,
                                                Domain = settings.ProxyDomain,
                                                Username = settings.ProxyUsername,
                                                Password = settings.ProxyPassword
                                            }
                        };

                dataLayer.Connect(versionOneSettings);
                eventDispatcher.Notify(this, new ModelChangedArgs(EventReceiver.OptionsView, EventContext.V1SettingsChanged));
            } catch(DataLayerException ex) {
                logger.Error("Error while loading V1Package: " + ex.Message, ex);
            }

            logger.Debug("Completed constructor execution.");
        }

        private void RegisterComponents() {
            var container = ServiceLocator.Instance.Container;
            
            container.Bind<IDataLayer>().ToConstant(dataLayer);
            container.Bind<IDataLayerInternal>().ToConstant(dataLayer);

            container.Bind<Configuration>().ToMethod(context => Configuration.Load()).InSingletonScope();
            container.Bind<ISettings>().ToMethod(context => SettingsImpl.Load()).InSingletonScope();
            container.Bind<ILoggerFactory>().To<LoggerFactory>().InSingletonScope();
            container.Bind<IEventDispatcher>().To<EventDispatcher>().InSingletonScope();
            
            container.Bind<IUIComponentFactory>().ToMethod(context => new UIComponentFactory(container, ServiceLocator.Instance.Get<ILoggerFactory>())).InSingletonScope();
            container.Bind<ComponentResolver<IParentWindow>>().ToConstant(new ComponentResolver<IParentWindow>(container, "Projects")).Named("Projects");
            container.Bind<ComponentResolver<IParentWindow>>().ToConstant(new ComponentResolver<IParentWindow>(container, "Workitems")).Named("Workitems");
            
            container.Bind<ProjectTreeController>().ToSelf().InSingletonScope();
            container.Bind<WorkitemTreeController>().ToSelf().InSingletonScope();
            container.Bind<ProjectTreeControl>().ToSelf().InSingletonScope();
            container.Bind<WorkitemTreeControl>().ToSelf().InSingletonScope();
        }

        private void AddProperties(Configuration cfg) {
            LoadOrderProperties();

            foreach(var column in cfg.AssetDetail.TaskColumns) {
                AddProperty(column, Entity.TaskType);
            }

            foreach(var column in cfg.AssetDetail.StoryColumns) {
                AddProperty(column, Entity.StoryType);
            }

            foreach(var column in cfg.AssetDetail.DefectColumns) {
                AddProperty(column, Entity.DefectType);
            }

            foreach(var column in cfg.AssetDetail.TestColumns) {
                AddProperty(column, Entity.TestType);
            }

            foreach(var column in cfg.GridSettings.Columns) {
                AddProperty(column, Entity.TaskType);
                AddProperty(column, Entity.StoryType);
                AddProperty(column, Entity.DefectType);
                AddProperty(column, Entity.TestType);
            }

            foreach(var column in cfg.ProjectTree.Columns) {
                AddProperty(column, Entity.ProjectType);
            }
        }

        /// <summary>
        /// We do not support Order property on UI. That's why configuration.xml is not used.
        /// Order property has special type, it currently can not be set properly.
        /// </summary>
        private void LoadOrderProperties() {
            dataLayer.AddProperty(Entity.OrderProperty, Entity.TestType, false);
            dataLayer.AddProperty(Entity.OrderProperty, Entity.TaskType, false);
        }

        private void AddProperty(ColumnSetting column, string prefix) {
            dataLayer.AddProperty(column.Attribute, prefix, column.Type == "List" || column.Type == "Multi");
        }

        /// <summary>
        /// This function is called when the user clicks the menu item that shows the 
        /// tool window. See the Initialize method to see how the menu item is associated to 
        /// this function using the OleMenuCommandService service and the MenuCommand class.
        /// </summary>
        private void ShowToolWindow<TWindow>(object sender, EventArgs e) where TWindow : ToolWindowPane {
            // Get the instance number 0 of this tool window. This window is single instance so this instance
            // is actually the only one.
            // The last flag is set to true so that if the tool window does not exists it will be created.
            var window = FindToolWindow(typeof(TWindow), 0, true);

            if((null == window) || (null == window.Frame)) {
                throw new NotSupportedException(Resources.CanNotCreateWindow);
            }

            var windowFrame = (IVsWindowFrame)window.Frame;
            ErrorHandler.ThrowOnFailure(windowFrame.Show());
        }

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initilaization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize() {
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", ToString()));
            base.Initialize();

            // Add our command handlers for menu (commands must exist in the .vsct file)
            var mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if(null != mcs) {
                // Create the command for Tasks tool window
                var projectsCommandId = new CommandID(GuidList.guidVersionOneTrackerCmdSet, (int)PkgCmdIDList.CmdidVersionOneProjects);
                var menuItem = new MenuCommand(ShowToolWindow<ProjectsWindow>, projectsCommandId);
                mcs.AddCommand(menuItem);
                // Create the command for the tool window
                var tasksCommandId = new CommandID(GuidList.guidVersionOneTrackerCmdSet, (int)PkgCmdIDList.CmdidVersionOneTasks);
                var menuToolWin = new MenuCommand(ShowToolWindow<TaskWindow>, tasksCommandId);
                mcs.AddCommand(menuToolWin);
            }
        }
    }
}