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
        private IProjectTreeView view;

        public IProjectTreeView View {
            get { return view; }
        }

        public ProjectTreeController(IDataLayer dataLayer, ISettings settings, IEventDispatcher eventDispatcher) {
            this.dataLayer = dataLayer;
            this.settings = settings;
            this.eventDispatcher = eventDispatcher;
            eventDispatcher.ModelChanged += eventDispatcher_ModelChanged;
        }

        public void RegisterView(IProjectTreeView view) {
            this.view = view;
            this.view.Controller = this;
        }

        public void PrepareView() {
            view.UpdateData();
        }

        public void HandleRefreshAction() {
            view.UpdateData();
        }

        public void HandleProjectSelected(Project project) {
            if (settings.SelectedProjectId != project.Id) {
                settings.SelectedProjectId = project.Id;
                settings.StoreSettings();
                dataLayer.CurrentProject = project;
                view.RefreshProperties();
                eventDispatcher.InvokeModelChanged(this, ModelChangedArgs.ProjectChanged);
            }
        }

        private void eventDispatcher_ModelChanged(object sender, ModelChangedArgs e) {
            if (view == null || (e != null && e.Scope < ModelChangedArgs.ProjectChanged.Scope)) {
                return;
            }
            view.UpdateData();
        }
    }
}