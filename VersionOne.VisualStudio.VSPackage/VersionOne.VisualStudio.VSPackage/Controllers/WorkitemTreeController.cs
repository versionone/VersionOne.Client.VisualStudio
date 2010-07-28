using System;
using System.Diagnostics;
using System.Windows.Forms;

using VersionOne.VisualStudio.VSPackage.Controls;
using VersionOne.VisualStudio.DataLayer;
using VersionOne.VisualStudio.VSPackage.Descriptors;
using VersionOne.VisualStudio.VSPackage.Events;
using VersionOne.VisualStudio.VSPackage.Settings;

namespace VersionOne.VisualStudio.VSPackage.Controllers {
    public class WorkitemTreeController {
        private IWorkitemTreeView view;
        private readonly IDataLayer dataLayer;
        private readonly ISettings settings;
        private readonly IEventDispatcher eventDispatcher;
        private StoryTreeModel model;

        public WorkitemTreeController(IDataLayer dataLayer, ISettings settings, IEventDispatcher eventDispatcher) {
            this.dataLayer = dataLayer;
            this.settings = settings;
            this.eventDispatcher = eventDispatcher;
            eventDispatcher.ModelChanged += eventDispatcher_ModelChanged;
            eventDispatcher.WorkitemPropertiesUpdated += eventDispatcher_WorkitemPropertiesUpdated;
        }

        private void eventDispatcher_ModelChanged(object sender, ModelChangedArgs e) {
            UpdateViewTitle();
            view.Model = model;
            UpdateViewData();
            bool mayAddSecondaryItems = ShouldEnableAddSecondaryItemCommand();
            view.AddTaskCommandEnabled = mayAddSecondaryItems;
            view.AddTestCommandEnabled = mayAddSecondaryItems;
        }

        private void eventDispatcher_WorkitemPropertiesUpdated(object sender, WorkitemPropertiesUpdatedArgs e) {
            if (e.Source == PropertyUpdateSource.WorkitemView) {
                view.RefreshProperties();
            } else if (e.Source == PropertyUpdateSource.WorkitemPropertyView) {
                model.InvokeStructureChanged();
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
            model = new StoryTreeModel(dataLayer);
            view.Model = model;
            view.ReconfigureTreeColumns();
            UpdateViewData();
            bool mayAddSecondaryItems = ShouldEnableAddSecondaryItemCommand();
            view.AddTaskCommandEnabled = mayAddSecondaryItems;
            view.AddTestCommandEnabled = mayAddSecondaryItems;
            view.AddDefectCommandEnabled = true;
        }

        #region Item commands from context menu

        public void CommitItem() {
            WorkitemDescriptor descriptor = view.CurrentWorkitemDescriptor;
            if(descriptor != null) {
				try {
					descriptor.Workitem.CommitChanges();
				} catch (ValidatorException ex) {
					view.ShowErrorMessage("Workitem cannot be saved because the following required fields are empty:" + ex.Message);
				}
                
                eventDispatcher.InvokeModelChanged(null, ModelChangedArgs.WorkitemChanged);
            }
        }

        public void RevertItem() {
            WorkitemDescriptor descriptor = view.CurrentWorkitemDescriptor;
            if(descriptor != null) {
                Workitem item = descriptor.Workitem;
                item.RevertChanges();
                
                if (item.IsVirtual) {
                    eventDispatcher.InvokeModelChanged(null, ModelChangedArgs.WorkitemChanged);
                }
                
                view.RefreshProperties();
                view.Refresh();
            }
        }

        public void SignupItem() {
            WorkitemDescriptor descriptor = view.CurrentWorkitemDescriptor;
            if(descriptor != null) {
                descriptor.Workitem.Signup();
                eventDispatcher.InvokeModelChanged(null, ModelChangedArgs.WorkitemChanged);
            }
        }
        
        public void QuickCloseItem() {
            WorkitemDescriptor descriptor = view.CurrentWorkitemDescriptor;
            if(descriptor != null) {
                try {
                    descriptor.Workitem.QuickClose();
                } catch (ValidatorException ex) {
                    view.ShowErrorMessage("Workitem cannot be closed because some required fields are empty:" + ex.Message);
                }
                eventDispatcher.InvokeModelChanged(null, ModelChangedArgs.WorkitemChanged);
            }
        }

        public void CloseItem() {
            WorkitemDescriptor descriptor = view.CurrentWorkitemDescriptor;
            if (descriptor != null) {
                DialogResult result = view.ShowCloseWorkitemDialog(descriptor.Workitem);
                if (result == DialogResult.OK) {
                    eventDispatcher.InvokeModelChanged(null, ModelChangedArgs.WorkitemChanged);
                }
            }
        }

        #endregion

        private void UpdateViewTitle() {
            Project currentProject = dataLayer.CurrentProject;
            string title = Resources.ToolWindowTitle;
            if (currentProject == null) {
                view.Title = title;
            } else {
                view.Title = string.Format("{0} - {1}", title, currentProject.GetProperty(Entity.NameProperty));
            }
        }

        #region Command handlers

        public void HandleRefreshCommand() {
            try {
                dataLayer.Reconnect();
            } catch (DataLayerException ex) {
                view.ResetPropertyView();
                Debug.WriteLine("Refresh failed: " + ex.Message);
            }
            eventDispatcher.InvokeModelChanged(null, ModelChangedArgs.SettingsChanged);
        }

        // TODO refactor
        public void HandleSaveCommand() {
            try {
                try {
                    dataLayer.CommitChanges();
                    dataLayer.Reconnect();
                } catch(ValidatorException ex) {
                    view.ShowValidationInformationDialog(ex.Message);
                }
            } catch (DataLayerException ex) {
                view.ResetPropertyView();
                Console.WriteLine(ex);
				view.ShowErrorMessage("Failed to save changes.");
            } catch (NullReferenceException ex) {
                //Workaround
                view.ResetPropertyView();
                Console.WriteLine(ex);
                view.ShowErrorMessage("Failed to save changes.");
            }
            eventDispatcher.InvokeModelChanged(null, ModelChangedArgs.SettingsChanged);
        }

        public void HandleTreeSelectionChanged() {
            view.RefreshProperties();
            bool mayAddSecondaryItems = ShouldEnableAddSecondaryItemCommand();
            view.AddTaskCommandEnabled = mayAddSecondaryItems;
            view.AddTestCommandEnabled = mayAddSecondaryItems;
        }

        public void HandleFilteringByOwner(bool onlyMyTasks) {
            settings.ShowMyTasks = onlyMyTasks;
            settings.StoreSettings();
            dataLayer.ShowAllTasks = !onlyMyTasks;
            eventDispatcher.InvokeModelChanged(null, ModelChangedArgs.WorkitemChanged);
        }

        #endregion

        private bool ShouldEnableAddSecondaryItemCommand() {
            if (view.CurrentWorkitemDescriptor == null) {
                return false;
            }

            Workitem selectedItem = view.CurrentWorkitemDescriptor.Workitem;
            Workitem parentItem = selectedItem.Parent;

            return selectedItem.IsPrimary || (parentItem != null && parentItem.IsPrimary);
        }

        #region Add Workitems

        public void AddTask() {
            AddSecondaryWorkitem(Entity.TaskPrefix);

        }

        public void AddDefect() {
            Workitem defect = dataLayer.CreateWorkitem(Entity.DefectPrefix, null);
            eventDispatcher.InvokeModelChanged(null, ModelChangedArgs.ProjectChanged);
            view.SelectWorkitem(defect);
            view.Refresh();
            view.RefreshProperties();
        }

        public void AddTest() {
            AddSecondaryWorkitem(Entity.TestPrefix);
        }

        private void AddSecondaryWorkitem(string type) {
            Workitem selectedItem = view.CurrentWorkitemDescriptor.Workitem;
            Workitem parent = selectedItem.IsPrimary ? selectedItem : selectedItem.Parent;

            Workitem item = dataLayer.CreateWorkitem(type, parent);

            eventDispatcher.InvokeModelChanged(null, ModelChangedArgs.WorkitemChanged);
            view.ExpandCurrentNode();
            view.SelectWorkitem(item);
            view.Refresh();
            view.RefreshProperties();
        }

        #endregion
    }
}