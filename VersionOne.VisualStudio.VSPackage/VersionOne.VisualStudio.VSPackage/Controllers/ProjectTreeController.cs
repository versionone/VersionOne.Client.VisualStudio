using System.Diagnostics;
using VersionOne.VisualStudio.DataLayer.Entities;
using VersionOne.VisualStudio.VSPackage.Controls;
using VersionOne.VisualStudio.DataLayer;
using VersionOne.VisualStudio.VSPackage.Events;
using VersionOne.VisualStudio.VSPackage.Settings;

namespace VersionOne.VisualStudio.VSPackage.Controllers {
    public class ProjectTreeController : BaseController {
        public IProjectTreeView View { get; private set; }

        public ProjectTreeController(IDataLayer dataLayer, ISettings settings, IEventDispatcher eventDispatcher) : base(dataLayer, settings, eventDispatcher) {
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
            if(Settings.SelectedProjectId != project.Id) {
                Settings.SelectedProjectId = project.Id;
                Settings.StoreSettings();
                DataLayer.CurrentProject = project;
                View.RefreshProperties();
                EventDispatcher.InvokeModelChanged(this, ModelChangedArgs.ProjectChanged);
            }
        }

        private void eventDispatcher_ModelChanged(object sender, ModelChangedArgs e) {
            if (View == null || (e != null && e.Scope < ModelChangedArgs.ProjectChanged.Scope)) {
                return;
            }

            View.UpdateData();
        }

        public void HandleProjectsRequest() {
            RunTaskAsync(View.GetWaitCursor(),
                () => View.Projects = DataLayer.GetProjectTree(),
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