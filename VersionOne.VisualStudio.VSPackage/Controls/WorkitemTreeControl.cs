using System;
using System.Collections.Generic;
using System.Drawing;
using Aga.Controls.Tree.NodeControls;
using Ninject;
using VersionOne.VisualStudio.DataLayer.Entities;
using VersionOne.VisualStudio.VSPackage.Controllers;
using VersionOne.VisualStudio.VSPackage.Dependencies;
using VersionOne.VisualStudio.VSPackage.Descriptors;
using Aga.Controls.Tree;
using VersionOne.VisualStudio.DataLayer;
using System.Windows.Forms;

using VersionOne.VisualStudio.VSPackage.Events;
using VersionOne.VisualStudio.VSPackage.Forms;
using VersionOne.VisualStudio.VSPackage.Settings;
using VersionOne.VisualStudio.VSPackage.TreeViewEditors;

using NodeComboBox = VersionOne.VisualStudio.VSPackage.TreeViewEditors.NodeComboBox;

namespace VersionOne.VisualStudio.VSPackage.Controls {
    /// <summary>
    /// Task List control displaying V1 tasks for the currently selected Project.
    /// </summary>
    public partial class WorkitemTreeControl : V1UserControl, IWorkitemTreeView {        
        private readonly Dictionary<string, string> columnToAttributeMappings = new Dictionary<string, string>();
        
        private StoryTreeModel storyTreeModel;

        private bool addTaskEnabled;
        private bool addDefectEnabled;
        private bool addTestEnabled;

        private readonly Configuration configuration;
        private readonly ISettings settings;

        public WorkitemTreeController Controller { get; set; }

        public string Title {
            get {
                var parentWindow = ParentWindowResolver.Resolve();
                return parentWindow != null ? parentWindow.Caption : null;
            }
            set {
                var parentWindow = ParentWindowResolver.Resolve();

                if(parentWindow != null) {
                    parentWindow.Caption = value;
                }
            }
        }

        public WorkitemDescriptor CurrentWorkitemDescriptor {
            get { return tvWorkitems.SelectedNode == null ? null : tvWorkitems.SelectedNode.Tag as WorkitemDescriptor; }
        }

        public StoryTreeModel Model {
            get { return storyTreeModel; }
            set {
                storyTreeModel = value;
                tvWorkitems.BeginUpdate();
                tvWorkitems.Model = null;
                tvWorkitems.Model = value;
                tvWorkitems.EndUpdate();
            }
        }

        public bool AddTaskCommandEnabled {
            get { return addTaskEnabled; }
            set {
                addTaskEnabled = value;
                btnAddTask.Enabled = value;
            }
        }

        public bool AddDefectCommandEnabled {
            get { return addDefectEnabled; }
            set {
                addDefectEnabled = value;
                btnAddDefect.Enabled = value;
            }
        }

        public bool AddTestCommandEnabled {
            get { return addTestEnabled; }
            set {
                addTestEnabled = value;
                btnAddTest.Enabled = value;
            }
        }

        private readonly IWaitCursor waitCursor;

        public WorkitemTreeControl(Configuration configuration, ISettings settings, IDataLayer dataLayer, [Named("Workitems")] ComponentResolver<IParentWindow> parentWindowResolver) 
                : base(parentWindowResolver, dataLayer) {
            InitializeComponent();

            this.configuration = configuration;
            this.settings = settings;

            btnOnlyMyTasks.Checked = settings.ShowMyTasks;

            tvWorkitems.SelectionChanged += tvWorkitems_SelectionChanged;
            btnSave.Click += toolBtnSave_Click;
            btnRefresh.Click += toolBtnRefresh_Click;
            btnOnlyMyTasks.CheckedChanged += btnShowMyTasks_CheckedChanged;
            
            miSave.Click += miSave_Click;
            miRevert.Click += miRevert_Click;
            miSignup.Click += miSignup_Click;
            miQuickClose.Click += miQuickClose_Click;
            miClose.Click += miClose_Click;
            miNewTask.Click += AddTask_Click;
            miNewDefect.Click += AddDefect_Click;
            miNewTest.Click += AddTest_Click;
            tvWorkitems.ContextMenu.Popup += ContextMenu_Popup;

            btnAddTask.Click += AddTask_Click;
            btnAddDefect.Click += AddDefect_Click;
            btnAddTest.Click += AddTest_Click;

            VisibleChanged += (sender, e) => RefreshProperties();
            CursorChanged += (sender, e) => RefreshProperties();
            Enter += (sender, e) => RefreshProperties();

            waitCursor = GetWaitCursor();

            tvWorkitems.AsyncExpanding = true;
            tvWorkitems.LoadOnDemand = true;
            tvWorkitems.Expanding += tvWorkitems_Expanding;
            tvWorkitems.Expanded += tvWorkitems_Expanded;
        }

        private void tvWorkitems_Expanded(object sender, TreeViewAdvEventArgs e) {
            if(!IsHandleCreated) {
                return;
            }

            Invoke(new Action(() => waitCursor.Hide()));
        }

        private void tvWorkitems_Expanding(object sender, TreeViewAdvEventArgs e) {
            if(!IsHandleCreated) {
                return;
            }

            Invoke(new Action(() => waitCursor.Show()));
        }

        public override void SetAccessibleControlsEnabled(bool enabled) {
            tvWorkitems.Enabled = enabled;
            tsMenu.Enabled = enabled;
        }

        public void ShowErrorMessage(string message) {
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public void ShowValidationInformationDialog(string message) {
            new ValidationDialog(message).ShowDialog(this);
        }

        public void ReconfigureTreeColumns() {
			if (!DataLayer.IsConnected) {
				return;
			}

            tvWorkitems.HideEditor();
            tvWorkitems.Columns.Clear();
            tvWorkitems.NodeControls.Clear();
            columnToAttributeMappings.Clear();

			foreach (var column in configuration.GridSettings.Columns) {
                if (column.EffortTracking && !DataLayer.EffortTracking.TrackEffort) {
					continue;
				}

			    var dataPropertyName = DataLayer.LocalizerResolve(column.Name);

                columnToAttributeMappings.Add(dataPropertyName, column.Attribute);

			    var treeColumn = new TreeColumn(dataPropertyName, column.Width) { SortOrder = SortOrder.None, TooltipText = dataPropertyName };

			    switch(column.Type) {
                    case "String":
                    case "Effort":
                        var textEditor = new CustomNodeTextBox();
                        ConfigureEditor(textEditor, dataPropertyName);
                        textEditor.IsColumnReadOnly = column.ReadOnly;
                        textEditor.ParentColumn = treeColumn;
                        textEditor.IsEditEnabledValueNeeded += CheckCellEditability;
                        tvWorkitems.NodeControls.Add(textEditor);
                        break;
                    case "List":
                        var listEditor = new NodeComboBox();
                        ConfigureEditor(listEditor, dataPropertyName);
                        listEditor.EditEnabled = !column.ReadOnly;
                        listEditor.ParentColumn = treeColumn;
                        listEditor.IsEditEnabledValueNeeded += CheckCellEditability;
                        tvWorkitems.NodeControls.Add(listEditor);
                        break;
                    case "Multi":
			            var listBoxEditor = new NodeListBox {ParentTree = tvWorkitems};
			            ConfigureEditor(listBoxEditor, dataPropertyName);
                        listBoxEditor.EditEnabled = !column.ReadOnly;
                        listBoxEditor.ParentColumn = treeColumn;
                        listBoxEditor.IsEditEnabledValueNeeded += CheckCellEditability;
                        tvWorkitems.NodeControls.Add(listBoxEditor);
                        break;
                    default:
                        throw new NotSupportedException();
                }

				tvWorkitems.Columns.Add(treeColumn);
			}

            AddStateIcon();
        }

        private void AddStateIcon() {
            if (tvWorkitems.Columns.Count > 0) {
                var nodeIcon = new NodeStateIcon {DataPropertyName = "Icon", ParentColumn = tvWorkitems.Columns[0]};
                tvWorkitems.NodeControls.Insert(0, nodeIcon);
            }
        }

    	private static void ConfigureEditor(BaseTextControl editor, string propertyName) {
            editor.DataPropertyName = propertyName;
            editor.IncrementalSearchEnabled = true;
            editor.LeftMargin = 3;
            editor.Trimming = StringTrimming.EllipsisCharacter;
            editor.UseCompatibleTextRendering = true;
        }

        /// <summary> 
        /// Let this control process the mnemonics.
        /// </summary>
        protected override bool ProcessDialogChar(char charCode) {
            // If we're the top-level form or control, we need to do the mnemonic handling
            if (charCode != ' ' && ProcessMnemonic(charCode)) {
                return true;
            }
            return base.ProcessDialogChar(charCode);
        }

        // TODO refactor, move logic to Controller
        private void UpdateMenuItemsVisibility(Workitem workitem) {
            miSave.Enabled = workitem != null && workitem.HasChanges && !(workitem.Parent != null && workitem.Parent.IsVirtual);
            miRevert.Enabled = workitem != null && workitem.HasChanges;
            miSignup.Enabled = workitem != null && !workitem.IsMine() && workitem.CanSignup && !workitem.IsVirtual;
            miQuickClose.Enabled = workitem != null && workitem.CanQuickClose && !workitem.IsVirtual;
            miClose.Enabled = workitem != null && !workitem.IsVirtual;
            miNewTask.Enabled = workitem != null && AddTaskCommandEnabled;
            miNewTest.Enabled = workitem != null && AddTestCommandEnabled;
            miNewDefect.Enabled = AddDefectCommandEnabled;
        }

        #region Event handlers

        private void ContextMenu_Popup(object sender, EventArgs e) {
            tvWorkitems.HideEditor();
            var selectedNodeExists = tvWorkitems.SelectedNode != null;

            var item = selectedNodeExists ? ((WorkitemDescriptor) tvWorkitems.SelectedNode.Tag).Workitem : null;
            UpdateMenuItemsVisibility(item);
        }

        private void AddTask_Click(object sender, EventArgs e) {
            Controller.AddTask();
        }

        private void AddDefect_Click(object sender, EventArgs e) {
            Controller.AddDefect();
        }

        private void AddTest_Click(object sender, EventArgs e) {
            Controller.AddTest();
        }

        private void tvWorkitems_SelectionChanged(object sender, EventArgs e) {
            Controller.HandleTreeSelectionChanged();
        }

        private void btnShowMyTasks_CheckedChanged(object sender, EventArgs e) {
            tvWorkitems.HideEditor();
            Controller.HandleFilteringByOwner(btnOnlyMyTasks.Checked);
        }

        private void toolBtnRefresh_Click(object sender, EventArgs e) {
            tvWorkitems.HideEditor();
            Controller.HandleRefreshCommand();
        }

        private void toolBtnSave_Click(object sender, EventArgs e) {
            tvWorkitems.HideEditor();
            Controller.HandleSaveCommand();
        }

        private void miRevert_Click(object sender, EventArgs e) {
            Controller.RevertItem();
        }

        private void miSignup_Click(object sender, EventArgs e) {
            Controller.SignupItem();
        }

        private void miSave_Click(object sender, EventArgs e) {
			Controller.CommitItem();
        }

        private void miQuickClose_Click(object sender, EventArgs e) {
            Controller.QuickCloseItem();
        }

        private void miClose_Click(object sender, EventArgs e) {
            if(CurrentWorkitemDescriptor != null) {
                var workitem = CurrentWorkitemDescriptor.Workitem;
                var result = new CloseWorkitemDialog(workitem).ShowDialog(this);
                
                if(result == DialogResult.OK) {
                    Controller.CloseItem(workitem);
                }
            }
        }

        private void CheckCellEditability(object sender, NodeControlValueEventArgs e) {
            var descriptor = (WorkitemDescriptor) e.Node.Tag;
            var item = descriptor.Workitem;

            var columnName = (sender as BaseTextControl).DataPropertyName;
            var propertyName = columnToAttributeMappings[columnName];
            var isReadOnly = item.IsPropertyReadOnly(propertyName);
            
            if(sender is CustomNodeTextBox) {
                var textBox = (CustomNodeTextBox) sender;
                textBox.IsPropertyReadOnly = isReadOnly;
                textBox.EditorContextMenu = textBox.IsReadOnly ? CreateReadonlyTextBoxContextMenu(textBox) : null;
            }

            if(sender is NodeComboBox) {
                var editor = (NodeComboBox) sender;
                var dataSource = DataLayer.GetListPropertyValues(item.TypePrefix + propertyName);
                editor.DropDownItems.Clear();
                editor.DropDownItems.AddRange(dataSource);
            }

            if(sender is NodeListBox) {
                var editor = (NodeListBox) sender;
                var dataSource = DataLayer.GetListPropertyValues(item.TypePrefix + propertyName);
                editor.DropDownItems.Clear();
                editor.DropDownItems.AddRange(dataSource);
            }
        }

        #endregion


        private static ContextMenu CreateReadonlyTextBoxContextMenu(NodeTextBox textBox) {
            var menu = new ContextMenu();
            var miCopyValue = new MenuItem("Copy");
            // VP
            //miCopyValue.Click += (sender, e) => textBox.Copy(((NodeTextBox)textBox).EditorTextBox);
            menu.MenuItems.Add(miCopyValue);
            return menu;
        }

        public void RefreshProperties() {
            var node = tvWorkitems.SelectedNode;
            WorkitemDescriptor descriptor = null;
            
            if (node != null) {
                var oldDescriptor = (WorkitemDescriptor) node.Tag;
                descriptor = oldDescriptor.GetDetailedDescriptor(
                    configuration.AssetDetail.GetColumns(oldDescriptor.Workitem.TypePrefix), 
                    PropertyUpdateSource.WorkitemPropertyView);
            }

            UpdatePropertyView(descriptor);
        }

        public void SetSelection() {
            if (tvWorkitems.SelectedNode != null) {
                return;
            }

            if (CurrentWorkitemId != null) {
                var selectedNode = tvWorkitems.FindNodeByMather(node => {
                                                                    var desc = node.Tag as WorkitemDescriptor;
                                                                    return desc != null && desc.Workitem.Id == CurrentWorkitemId;
                                                                });
                if (selectedNode != null) {
                    tvWorkitems.SelectedNode = selectedNode;
                    return;
                }
            }
            
            var root = tvWorkitems.Root;
            
            if (root.Children.Count > 0) {
                tvWorkitems.SelectedNode = root.Children[0];
            } else {
                ResetPropertyView();
            }
        }

        public void SelectWorkitem(Workitem item) {
            var foundNode = tvWorkitems.FindNodeByMather(node => {
                                                             var descriptor = node.Tag as WorkitemDescriptor;
                                                             return descriptor != null && item.Equals(descriptor.Workitem);
                                                         });
            if(foundNode != null) {
                tvWorkitems.SelectedNode = foundNode;
            }
        }

        public void ExpandCurrentNode() {
            if (tvWorkitems.SelectedNode != null) {
                tvWorkitems.SelectedNode.Expand(false);
            }
        }
    }
}