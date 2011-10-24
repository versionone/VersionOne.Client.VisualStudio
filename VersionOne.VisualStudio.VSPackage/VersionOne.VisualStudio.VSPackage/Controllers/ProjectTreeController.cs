using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using VersionOne.VisualStudio.DataLayer.Entities;
using VersionOne.VisualStudio.VSPackage.Controls;
using VersionOne.VisualStudio.DataLayer;
using VersionOne.VisualStudio.VSPackage.Events;
using VersionOne.VisualStudio.VSPackage.Settings;

namespace VersionOne.VisualStudio.VSPackage.Controllers {
    public class ProjectTreeController {
        private readonly ISettings settings;
        private readonly IDataLayer dataLayer;
        private readonly IEventDispatcher eventDispatcher;

        public IProjectTreeView View { get; private set; }

        public ProjectTreeController(IDataLayer dataLayer, ISettings settings, IEventDispatcher eventDispatcher) {
            this.dataLayer = dataLayer;
            this.settings = settings;
            this.eventDispatcher = eventDispatcher;
            
            eventDispatcher.ModelChanged += eventDispatcher_ModelChanged;
        }

        public void RegisterView(IProjectTreeView view) {
            View = view;
            View.Controller = this;
        }

        public void PrepareView() {
            View.UpdateData();
        }

        public void HandleRefreshAction() {
            View.UpdateData();
        }

        public void HandleProjectSelected(Project project) {
            if(settings.SelectedProjectId != project.Id) {
                settings.SelectedProjectId = project.Id;
                settings.StoreSettings();
                dataLayer.CurrentProject = project;
                View.RefreshProperties();
                eventDispatcher.InvokeModelChanged(this, ModelChangedArgs.ProjectChanged);
            }
        }

        private void eventDispatcher_ModelChanged(object sender, ModelChangedArgs e) {
            if (View == null || (e != null && e.Scope < ModelChangedArgs.ProjectChanged.Scope)) {
                return;
            }

            View.UpdateData();
        }

        public void HandleProjectsRequest() {
            new BackgroundTaskRunner(View.GetWaitCursor()).Run(
                () => View.Projects = dataLayer.GetProjectTree(),
                () => View.CompleteProjectsRequest(),
                ex => {
                    if (ex is DataLayerException) {
                        View.ResetPropertyView();
                        Debug.WriteLine("Refresh failed: " + ex.Message);
                    }
                });
        }
    }
}