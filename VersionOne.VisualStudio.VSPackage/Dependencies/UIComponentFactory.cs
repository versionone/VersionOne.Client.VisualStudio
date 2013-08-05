using System;
using System.Windows.Forms;
using Ninject;
using VersionOne.VisualStudio.DataLayer.Logging;
using VersionOne.VisualStudio.VSPackage.Controllers;
using VersionOne.VisualStudio.VSPackage.Controls;

namespace VersionOne.VisualStudio.VSPackage.Dependencies {
    internal class UIComponentFactory : IUIComponentFactory {
        private readonly IKernel container;
        private readonly ILogger logger;

        private readonly Lazy<WorkitemTreeControl> workitemTreeControl;
        private readonly Lazy<ProjectTreeControl> projectTreeControl; 

        public UIComponentFactory(IKernel container, ILoggerFactory loggerFactory) {
            this.container = container;
            this.logger = loggerFactory.GetLogger("UIComponentFactory");

            workitemTreeControl = new Lazy<WorkitemTreeControl>(CreateWorkitemTreeControl, false);
            projectTreeControl = new Lazy<ProjectTreeControl>(CreateProjectTreeControl, false);
        }

        private WorkitemTreeControl CreateWorkitemTreeControl() {
            var controller = container.Get<WorkitemTreeController>();
            var control = container.Get<WorkitemTreeControl>();            
            
            controller.Register(control);
            controller.PrepareView();
            controller.Prepare();

            return control;
        }

        private ProjectTreeControl CreateProjectTreeControl() {
            var controller = container.Get<ProjectTreeController>();
            var control = container.Get<ProjectTreeControl>();

            controller.RegisterView(control);
            controller.PrepareView();
            controller.Prepare();

            return control;
        }

        public Control GetWorkitemView() {
            if(!projectTreeControl.IsValueCreated) {
                GetProjectView();
                logger.Debug("Created Project Tree view on first request to Workitem Tree");
            }

            return workitemTreeControl.Value;
        }

        public Control GetProjectView() {
            return projectTreeControl.Value;
        }
    }
}