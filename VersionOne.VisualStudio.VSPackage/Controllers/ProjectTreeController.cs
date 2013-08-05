using System;
using VersionOne.VisualStudio.DataLayer.Entities;
using VersionOne.VisualStudio.DataLayer.Logging;
using VersionOne.VisualStudio.VSPackage.Controls;
using VersionOne.VisualStudio.DataLayer;
using VersionOne.VisualStudio.VSPackage.Events;
using VersionOne.VisualStudio.VSPackage.Settings;

namespace VersionOne.VisualStudio.VSPackage.Controllers {
    public class ProjectTreeController : BaseController {
        protected override EventReceiver ReceiverType {
            get { return EventReceiver.ProjectView; }
        }

        public IProjectTreeView View { get; private set; }

        public ProjectTreeController(ILoggerFactory loggerFactory, IDataLayer dataLayer, ISettings settings, IEventDispatcher eventDispatcher) : base(loggerFactory, dataLayer, settings, eventDispatcher) { }

        public void RegisterView(IProjectTreeView view) {
            View = view;
            View.Controller = this;
        }

        public void PrepareView() {
            View.UpdateData();
        }

        protected override void HandleModelChanged(object sender, ModelChangedArgs e) {
            switch(e.Context) {
                case EventContext.ProjectsRequested:
                    Logger.Debug("Loading projects...");
                    View.UpdateData();
                    break;
                case EventContext.ProjectSelected:
                    Logger.Debug("Project selected, sending update request to Workitem view");
                    EventDispatcher.Notify(this, new ModelChangedArgs(EventReceiver.WorkitemView, EventContext.ProjectSelected));
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        public void HandleRefreshAction() {
            View.UpdateData();
        }

        public void HandleProjectSelected(Project project) {
            if(Settings.SelectedProjectId != project.Id) {
                Logger.Debug("Selected project is changed, saving new Project ID");
                Settings.SelectedProjectId = project.Id;
                Settings.StoreSettings();
                DataLayer.CurrentProject = project;
                EventDispatcher.Notify(this, new ModelChangedArgs(EventReceiver.WorkitemView, EventContext.WorkitemCacheInvalidated));
                View.RefreshProperties();
            }

            EventDispatcher.Notify(this, new ModelChangedArgs(EventReceiver.ProjectView, EventContext.ProjectSelected));
        }

        public void HandleProjectsRequest() {
            RunTaskAsync(View.GetWaitCursor(),
                () => View.Projects = DataLayer.GetProjectTree(),
                () => View.CompleteProjectsRequest(),
                ex => {
                    if(ex is DataLayerException) {
                        View.ResetPropertyView();
                        Logger.Error("Failed to download Projects", ex);
                    }
                });
        }
    }
}