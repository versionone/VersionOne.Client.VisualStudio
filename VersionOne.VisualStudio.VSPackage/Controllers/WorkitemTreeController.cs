using System;
using System.Collections.Generic;
using VersionOne.VisualStudio.DataLayer.Entities;
using VersionOne.VisualStudio.DataLayer.Logging;
using VersionOne.VisualStudio.VSPackage.Controls;
using VersionOne.VisualStudio.DataLayer;
using VersionOne.VisualStudio.VSPackage.Events;
using VersionOne.VisualStudio.VSPackage.Settings;

namespace VersionOne.VisualStudio.VSPackage.Controllers {
    public class WorkitemTreeController : BaseController {
        private IWorkitemTreeView view;
        private StoryTreeModel model;
        private IAssetCache assetCache;
        private readonly Configuration configuration;

        public bool CanRetrieveData { get { return DataLayer.IsConnected; } }

        protected override EventReceiver ReceiverType {
            get { return EventReceiver.WorkitemView; }
        }

        public WorkitemTreeController(ILoggerFactory loggerFactory, IDataLayer dataLayer, Configuration configuration, ISettings settings, IEventDispatcher eventDispatcher) 
                : base(loggerFactory, dataLayer, settings, eventDispatcher) {
            this.configuration = configuration;
        }

        protected override void HandleModelChanged(object sender, ModelChangedArgs e) {
            switch (e.Context) {
                case EventContext.WorkitemPropertiesUpdatedFromView:
                    HandleWorkitemPropertiesUpdated(PropertyUpdateSource.WorkitemView);
                    break;
                case EventContext.WorkitemPropertiesUpdatedFromPropertyView:
                    HandleWorkitemPropertiesUpdated(PropertyUpdateSource.WorkitemPropertyView);
                    break;
                case EventContext.WorkitemsChanged:
                    model.InvokeStructureChanged();
                    break;
                case EventContext.ProjectSelected:
                    HandleModelChanged();
                    break;
                case EventContext.WorkitemsRequested:
                    HandleModelChanged();
                    break;
                case EventContext.WorkitemCacheInvalidated:
                    assetCache.Drop();
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        private void HandleModelChanged() {
            UpdateViewTitle();
            view.Model = model;
            UpdateViewData();
            var mayAddSecondaryItems = ShouldEnableAddSecondaryItemCommand();
            view.AddTaskCommandEnabled = mayAddSecondaryItems;
            view.AddTestCommandEnabled = mayAddSecondaryItems;
        }

        private void HandleWorkitemPropertiesUpdated(PropertyUpdateSource source) {
            switch (source) {
                case PropertyUpdateSource.WorkitemView:
                    view.RefreshProperties();
                    break;
                case PropertyUpdateSource.WorkitemPropertyView:
                    model.InvokeStructureChanged();
                    break;
            }
        }

        private void UpdateViewData() {
            try {
                if (!view.CheckSettingsAreValid()) {
                    view.ResetPropertyView();
                } else {
                    view.ReconfigureTreeColumns();
                    view.SetSelection();
                }

                view.Refresh();
            } catch (DataLayerException ex) {
                Logger.Error("Failed to update view", ex);
				view.ResetPropertyView();
            }
        }

        public void Register(IWorkitemTreeView view) {
            if(view == null) {
                throw new ArgumentNullException("view");
            }

            this.view = view;
            view.Controller = this;
            assetCache = DataLayer.CreateAssetCache();
        }

        public void PrepareView() {
            UpdateViewTitle();
            model = new StoryTreeModel(this, configuration);
            view.Model = model;
            view.ReconfigureTreeColumns();
            UpdateViewData();
            
            var mayAddSecondaryItems = ShouldEnableAddSecondaryItemCommand();
            view.AddTaskCommandEnabled = mayAddSecondaryItems;
            view.AddTestCommandEnabled = mayAddSecondaryItems;
            view.AddDefectCommandEnabled = true;
        }

        #region Item commands from context menu

        public void CommitItem() {
            var descriptor = view.CurrentWorkitemDescriptor;
            
            if(descriptor != null) {
                RunTaskAsync(view.GetWaitCursor(),
                             () => descriptor.Workitem.CommitChanges(),
                             () => EventDispatcher.Notify(null, new ModelChangedArgs(EventReceiver.WorkitemView, EventContext.WorkitemsRequested)),
                             ex => {
                                 if(ex is ValidatorException) {
                                     view.ShowErrorMessage("Workitem cannot be saved because the following required fields are empty:" + ex.Message);
                                     Logger.Warn("Workitem validation error, item cannot be persisted", ex);
                                 }
                             });
            }
        }

        public void RevertItem() {
            var descriptor = view.CurrentWorkitemDescriptor;
            
            if(descriptor != null) {
                var item = descriptor.Workitem;
                item.RevertChanges();
                
                if (item.IsVirtual) {
                    EventDispatcher.Notify(null, new ModelChangedArgs(EventReceiver.WorkitemView, EventContext.WorkitemsChanged));
                }
                
                view.RefreshProperties();
                view.Refresh();
            }
        }

        public void SignupItem() {
            var descriptor = view.CurrentWorkitemDescriptor;
            
            if(descriptor != null) {
                RunTaskAsync(view.GetWaitCursor(),
                    () => descriptor.Workitem.Signup(),
                    () => EventDispatcher.Notify(null, new ModelChangedArgs(EventReceiver.WorkitemView, EventContext.WorkitemsRequested)));
            }
        }
        
        public void QuickCloseItem() {
            var descriptor = view.CurrentWorkitemDescriptor;

            if(descriptor != null) {
                RunTaskAsync(view.GetWaitCursor(),
                             descriptor.Workitem.QuickClose,
                             () => EventDispatcher.Notify(null, new ModelChangedArgs(EventReceiver.WorkitemView, EventContext.WorkitemsRequested)),
                             ex => {
                                 if(ex is ValidatorException) {
                                     Logger.Warn("Cannot save workitem before closing it because of validation errors", ex);
                                     view.ShowErrorMessage("Workitem cannot be closed because some required fields are empty: " + ex.Message);
                                 }
                             });
            }
        }

        public void CloseItem(Workitem workitem) {
            RunTaskAsync(view.GetWaitCursor(),
                         workitem.Close,
                         () => EventDispatcher.Notify(null, new ModelChangedArgs(EventReceiver.WorkitemView, EventContext.WorkitemsRequested)),
                         ex => {
                             if(ex.GetType() == typeof(ValidatorException)) {
                                 Logger.Warn("Cannot save workitem before closing it because of validation errors", ex);
                                 view.ShowValidationInformationDialog(ex.Message);
                             } else if(ex.GetType() == typeof(DataLayerException)) {
                                 Logger.Error("Failed to close workitem", ex);
                                 view.ShowErrorMessage("Server communication error. Failed to close workitem. " + Environment.NewLine + ex.Message);
                             }
                         });
        }

        #endregion

        private void UpdateViewTitle() {
            var currentProject = DataLayer.CurrentProject;            
            view.Title = currentProject == null ? Resources.ToolWindowTitle : string.Format("{0} - {1}", Resources.ToolWindowTitle, currentProject.GetProperty(Entity.NameProperty));
        }

        #region Command handlers

        public void HandleRefreshCommand() {
            try {
                assetCache.Drop();
                DataLayer.EffortTracking.Refresh();
                DataLayer.UpdateListPropertyValues();
                EventDispatcher.Notify(null, new ModelChangedArgs(EventReceiver.WorkitemView, EventContext.WorkitemsRequested));
            } catch(DataLayerException ex) {
                view.ResetPropertyView();
                Logger.Error("Refresh failed", ex);
            }
        }

        public void HandleSaveCommand() {
            RunTaskAsync(view.GetWaitCursor(),
                         () => {
                             DataLayer.CommitChanges(assetCache);
                             assetCache.Drop();
                         },
                         () => EventDispatcher.Notify(null, new ModelChangedArgs(EventReceiver.WorkitemView, EventContext.WorkitemsRequested)),
                         ex => {
                             if(ex.GetType() == typeof(ValidatorException)) {
                                 Logger.Warn("Cannot save workitems because of validation errors", ex);
                                 view.ShowValidationInformationDialog(ex.Message);
                             }

                             if(ex.GetType() == typeof(DataLayerException)) {
                                 Logger.Error("Failed to save changes", ex);
                                 view.ResetPropertyView();
                                 view.ShowErrorMessage("Failed to save changes.");
                             }
                         });
        }

        public void HandleTreeSelectionChanged() {
            view.RefreshProperties();
            
            var mayAddSecondaryItems = ShouldEnableAddSecondaryItemCommand();
            view.AddTaskCommandEnabled = mayAddSecondaryItems;
            view.AddTestCommandEnabled = mayAddSecondaryItems;
        }

        public void HandleFilteringByOwner(bool onlyMyTasks) {
            Settings.ShowMyTasks = onlyMyTasks;
            Settings.StoreSettings();
            DataLayer.ShowAllTasks = !onlyMyTasks;
            EventDispatcher.Notify(null, new ModelChangedArgs(EventReceiver.WorkitemView, EventContext.WorkitemsRequested));
        }

        #endregion

        private bool ShouldEnableAddSecondaryItemCommand() {
            if (view.CurrentWorkitemDescriptor == null) {
                return false;
            }

            var selectedItem = view.CurrentWorkitemDescriptor.Workitem;
            var parentItem = selectedItem.Parent;

            return selectedItem.IsPrimary || (parentItem != null && parentItem.IsPrimary);
        }

        #region Add Workitems

        public void AddTask() {
            AddSecondaryWorkitem(Entity.TaskType);
        }

        public void AddDefect() {
            var defect = DataLayer.CreateWorkitem(Entity.DefectType, null, assetCache);
            assetCache.Add(defect);
            EventDispatcher.Notify(null, new ModelChangedArgs(EventReceiver.WorkitemView, EventContext.WorkitemsRequested));
            view.SelectWorkitem(defect);
            view.Refresh();
            view.RefreshProperties();
        }

        public void AddTest() {
            AddSecondaryWorkitem(Entity.TestType);
        }

        private void AddSecondaryWorkitem(string type) {
            var selectedItem = view.CurrentWorkitemDescriptor.Workitem;
            var parent = selectedItem.IsPrimary ? selectedItem : selectedItem.Parent;

            var item = DataLayer.CreateWorkitem(type, parent, assetCache);

            EventDispatcher.Notify(null, new ModelChangedArgs(EventReceiver.WorkitemView, EventContext.WorkitemsRequested));
            view.ExpandCurrentNode();
            view.SelectWorkitem(item);
            view.Refresh();
            view.RefreshProperties();
        }

        #endregion

        public ICollection<Workitem> GetWorkitems() {
            try {
                // NOTE this may be totally wrong - DataLayer object is shared among many clients. But it is not thread safe, that's why local lock object is not used
                lock(DataLayer) {
                    DataLayer.GetWorkitems(assetCache);
                    return assetCache.GetWorkitems(!Settings.ShowMyTasks);
                }
            } catch(DataLayerException) {
                view.ShowErrorMessage("Unable to download workitems.");
                return null;
            }
        }
    }
}