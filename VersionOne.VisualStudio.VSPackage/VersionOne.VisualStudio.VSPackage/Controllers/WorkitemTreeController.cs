using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
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

        public WorkitemTreeController(IDataLayer dataLayer, ISettings settings, IEventDispatcher eventDispatcher) : base(dataLayer, settings, eventDispatcher) {
            eventDispatcher.ModelChanged += eventDispatcher_ModelChanged;
            eventDispatcher.WorkitemPropertiesUpdated += eventDispatcher_WorkitemPropertiesUpdated;
        }

        private void eventDispatcher_ModelChanged(object sender, ModelChangedArgs e) {
            UpdateViewTitle();
            view.Model = model;
            UpdateViewData();
            var mayAddSecondaryItems = ShouldEnableAddSecondaryItemCommand();
            view.AddTaskCommandEnabled = mayAddSecondaryItems;
            view.AddTestCommandEnabled = mayAddSecondaryItems;
        }

        private void eventDispatcher_WorkitemPropertiesUpdated(object sender, WorkitemPropertiesUpdatedArgs e) {
            switch (e.Source) {
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
				try {
					descriptor.Workitem.CommitChanges();
				} catch (ValidatorException ex) {
					view.ShowErrorMessage("Workitem cannot be saved because the following required fields are empty:" + ex.Message);
				}
                
                EventDispatcher.InvokeModelChanged(null, ModelChangedArgs.WorkitemChanged);
            }
        }

        public void RevertItem() {
            var descriptor = view.CurrentWorkitemDescriptor;
            
            if(descriptor != null) {
                var item = descriptor.Workitem;
                item.RevertChanges();
                
                if (item.IsVirtual) {
                    EventDispatcher.InvokeModelChanged(null, ModelChangedArgs.WorkitemChanged);
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
                    () => EventDispatcher.InvokeModelChanged(null, ModelChangedArgs.WorkitemChanged));
            }
        }
        
        public void QuickCloseItem() {
            var descriptor = view.CurrentWorkitemDescriptor;

            if(descriptor != null) {
                RunTaskAsync(view.GetWaitCursor(),
                             () => descriptor.Workitem.QuickClose(),
                             () => EventDispatcher.InvokeModelChanged(null, ModelChangedArgs.WorkitemChanged),
                             ex => {
                                 if(ex is ValidatorException) {
                                     view.ShowErrorMessage(
                                         "Workitem cannot be closed because some required fields are empty:" +
                                         ex.Message);
                                 }
                             });
            }
        }

        // TODO refactor to async
        public void CloseItem() {
            var descriptor = view.CurrentWorkitemDescriptor;
            
            if(descriptor != null) {
                var result = view.ShowCloseWorkitemDialog(descriptor.Workitem);
                
                if (result == DialogResult.OK) {
                    EventDispatcher.InvokeModelChanged(null, ModelChangedArgs.WorkitemChanged);
                }
            }
        }

        #endregion

        private void UpdateViewTitle() {
            var currentProject = DataLayer.CurrentProject;            
            view.Title = currentProject == null ? Resources.ToolWindowTitle : string.Format("{0} - {1}", Resources.ToolWindowTitle, currentProject.GetProperty(Entity.NameProperty));
        }

        #region Command handlers

        public void HandleRefreshCommand() {
            RunTaskAsync(view.GetWaitCursor(),
                () => DataLayer.Reconnect(),
                () => EventDispatcher.InvokeModelChanged(null, ModelChangedArgs.SettingsChanged),
                ex => {
                    if(ex is DataLayerException) {
                        view.ResetPropertyView();
                        Debug.WriteLine("Refresh failed: " + ex.Message);
                    }
                });
        }

        // TODO refactor
        public void HandleSaveCommand() {
            try {
                DataLayer.CommitChanges();
                DataLayer.Reconnect();
            } catch(ValidatorException ex) {
                view.ShowValidationInformationDialog(ex.Message);
            } catch(DataLayerException ex) {
                view.ResetPropertyView();
                view.ShowErrorMessage("Failed to save changes.");
            } catch(NullReferenceException ex) {
                //Workaround
                view.ResetPropertyView();
                view.ShowErrorMessage("Failed to save changes.");
            }

            EventDispatcher.InvokeModelChanged(null, ModelChangedArgs.SettingsChanged);
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
            EventDispatcher.InvokeModelChanged(null, ModelChangedArgs.WorkitemChanged);
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
            EventDispatcher.InvokeModelChanged(null, ModelChangedArgs.ProjectChanged);
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

            EventDispatcher.InvokeModelChanged(null, ModelChangedArgs.WorkitemChanged);
            view.ExpandCurrentNode();
            view.SelectWorkitem(item);
            view.Refresh();
            view.RefreshProperties();
        }

        #endregion

        // TODO implement async request
        public ICollection<Workitem> GetWorkitems() {
            try {
                return DataLayer.GetWorkitems();
            } catch(DataLayerException) {
                // TODO show message?
                return null;
            }
        }
    }
}