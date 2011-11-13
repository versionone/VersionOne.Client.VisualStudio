using System;
using System.Collections.Generic;
using System.Diagnostics;
using VersionOne.VisualStudio.DataLayer.Entities;
using VersionOne.VisualStudio.VSPackage.Controls;
using VersionOne.VisualStudio.DataLayer;
using VersionOne.VisualStudio.VSPackage.Events;
using VersionOne.VisualStudio.VSPackage.Settings;

namespace VersionOne.VisualStudio.VSPackage.Controllers {
    public class WorkitemTreeController : BaseController {
        private IWorkitemTreeView view;
        private StoryTreeModel model;

        public bool CanRetrieveData { get { return DataLayer.IsConnected; } }

        protected override EventReceiver ReceiverType {
            get { return EventReceiver.WorkitemView; }
        }

        public WorkitemTreeController(IDataLayer dataLayer, ISettings settings, IEventDispatcher eventDispatcher) : base(dataLayer, settings, eventDispatcher) { }

        protected override void HandleModelChanged(object sender, ModelChangedArgs e) {
            HandleModelChanged();

            switch (e.Context) {
                case EventContext.WorkitemPropertiesUpdated:
                    // TODO distinguish between sources
                    HandleWorkitemPropertiesUpdated(PropertyUpdateSource.WorkitemPropertyView);
                    break;
                case EventContext.WorkitemsChanged:
                    UpdateViewData();
                    break;
                case EventContext.ProjectSelected:
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
            } catch (DataLayerException) {
				view.ResetPropertyView();
            }
        }

        public void Register(IWorkitemTreeView view) {
            if(view == null) {
                throw new ArgumentNullException("view");
            }

            this.view = view;
            view.Controller = this;
        }

        public void PrepareView() {
            UpdateViewTitle();
            model = new StoryTreeModel(this);
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
                             () => EventDispatcher.Notify(null, new ModelChangedArgs(EventReceiver.WorkitemView, EventContext.WorkitemsChanged)),
                             ex => {
                                 if(ex is ValidatorException) {
                                     view.ShowErrorMessage("Workitem cannot be saved because the following required fields are empty:" + ex.Message);
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
                    () => EventDispatcher.Notify(null, new ModelChangedArgs(EventReceiver.WorkitemView, EventContext.WorkitemsChanged)));
            }
        }
        
        public void QuickCloseItem() {
            var descriptor = view.CurrentWorkitemDescriptor;

            if(descriptor != null) {
                RunTaskAsync(view.GetWaitCursor(),
                             () => descriptor.Workitem.QuickClose(),
                             () => EventDispatcher.Notify(null, new ModelChangedArgs(EventReceiver.WorkitemView, EventContext.WorkitemsChanged)),
                             ex => {
                                 if(ex is ValidatorException) {
                                     view.ShowErrorMessage("Workitem cannot be closed because some required fields are empty: " + ex.Message);
                                 }
                             });
            }
        }

        public void CloseItem(Workitem workitem) {
            RunTaskAsync(view.GetWaitCursor(),
                         () => {
                             workitem.CommitChanges();
                             workitem.Close();
                         },
                         () => EventDispatcher.Notify(null, new ModelChangedArgs(EventReceiver.WorkitemView, EventContext.WorkitemsChanged)),
                         ex => {
                             if(ex.GetType() == typeof(ValidatorException)) {
                                 view.ShowValidationInformationDialog(ex.Message);
                             } else if(ex.GetType() == typeof(DataLayerException)) {
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
            RunTaskAsync(view.GetWaitCursor(),
                () => DataLayer.DropWorkitemCache(),
                () => EventDispatcher.Notify(null, new ModelChangedArgs(EventReceiver.WorkitemView, EventContext.WorkitemsRequested)),
                ex => {
                    if(ex is DataLayerException) {
                        view.ResetPropertyView();
                        Debug.WriteLine("Refresh failed: " + ex.Message);
                    }
                });
        }

        // TODO refactor; NullReferenceException is marked a workaround - investigate on it
        public void HandleSaveCommand() {
            RunTaskAsync(view.GetWaitCursor(),
                         () => {
                             DataLayer.CommitChanges();
                             DataLayer.DropWorkitemCache();
                         },
                         () => EventDispatcher.Notify(null, new ModelChangedArgs(EventReceiver.WorkitemView, EventContext.WorkitemsRequested)),
                         ex => {
                             if(ex.GetType() == typeof(ValidatorException)) {
                                 view.ShowValidationInformationDialog(ex.Message);
                             }

                             if(ex.GetType() == typeof(DataLayerException)) {
                                 view.ResetPropertyView();
                                 view.ShowErrorMessage("Failed to save changes.");
                             }

                             if(ex.GetType() == typeof(NullReferenceException)) {
                                 //Workaround
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
            EventDispatcher.Notify(null, new ModelChangedArgs(EventReceiver.WorkitemView, EventContext.WorkitemsChanged));
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
            AddSecondaryWorkitem(Entity.TaskPrefix);
        }

        public void AddDefect() {
            var defect = DataLayer.CreateWorkitem(Entity.DefectPrefix, null);
            EventDispatcher.Notify(null, new ModelChangedArgs(EventReceiver.WorkitemView, EventContext.WorkitemsChanged));
            view.SelectWorkitem(defect);
            view.Refresh();
            view.RefreshProperties();
        }

        public void AddTest() {
            AddSecondaryWorkitem(Entity.TestPrefix);
        }

        private void AddSecondaryWorkitem(string type) {
            var selectedItem = view.CurrentWorkitemDescriptor.Workitem;
            var parent = selectedItem.IsPrimary ? selectedItem : selectedItem.Parent;

            var item = DataLayer.CreateWorkitem(type, parent);

            EventDispatcher.Notify(null, new ModelChangedArgs(EventReceiver.WorkitemView, EventContext.WorkitemsChanged));
            view.ExpandCurrentNode();
            view.SelectWorkitem(item);
            view.Refresh();
            view.RefreshProperties();
        }

        #endregion

        public ICollection<Workitem> GetWorkitems() {
            try {
                lock(DataLayer) {
                    return DataLayer.GetWorkitems();
                }
            } catch(DataLayerException) {
                view.ShowErrorMessage("Unable to download workitems.");
                return null;
            }
        }
    }
}