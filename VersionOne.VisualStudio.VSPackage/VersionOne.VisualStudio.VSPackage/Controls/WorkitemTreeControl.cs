using System;
using System.Collections.Generic;
using System.Drawing;
using Aga.Controls.Tree.NodeControls;
using VersionOne.VisualStudio.DataLayer.Entities;
using VersionOne.VisualStudio.VSPackage.Controllers;
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
        private WorkitemTreeController controller;
        
        private readonly Dictionary<string, string> columnToAttributeMappings = new Dictionary<string, string>();
        
        private StoryTreeModel storyTreeModel;

        private bool addTaskEnabled;
        private bool addDefectEnabled;
        private bool addTestEnabled;

        public WorkitemTreeController Controller {
            get { return controller; }
            set { controller = value; }
        }

        public string Title {
            get { return ParentWindow.Caption; }
            set { ParentWindow.Caption = value; }
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

        public WorkitemTreeControl(TaskWindow parent) : base(parent) {
            InitializeComponent();
            
            btnOnlyMyTasks.Checked = SettingsImpl.Instance.ShowMyTasks;

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
            tvWorkitems.ContextMenu.Popup += ContextMenu_Popup;

            btnAddTask.Click += AddTask_Click;
            btnAddDefect.Click += AddDefect_Click;
            btnAddTest.Click += AddTest_Click;

            VisibleChanged += delegate { RefreshProperties(); };
            CursorChanged += delegate { RefreshProperties(); };
            Enter += delegate { RefreshProperties(); };
        }

        public void ShowErrorMessage(string message) {
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public DialogResult ShowCloseWorkitemDialog(Workitem item) {
            return new CloseWorkitemDialog(item).ShowDialog(this);
        }

        public void ShowValidationInformationDialog(string message) {
            new ValidationDialog(message).ShowDialog(this);
        }

        public void ReconfigureTreeColumns() {
			if (!dataLayer.IsConnected) {
				return;
			}
            tvWorkitems.HideEditor();
            tvWorkitems.Columns.Clear();
            tvWorkitems.NodeControls.Clear();
            columnToAttributeMappings.Clear();

			foreach (ColumnSetting column in Configuration.Instance.GridSettings.Columns) {
                if (column.EffortTracking && !ApiDataLayer.Instance.TrackEffort) {
					continue;
				}

			    string dataPropertyName = dataLayer.LocalizerResolve(column.Name);

                columnToAttributeMappings.Add(dataPropertyName, column.Attribute);

			    TreeColumn treeColumn = new TreeColumn(dataPropertyName, column.Width);
				treeColumn.SortOrder = SortOrder.None;
				treeColumn.TooltipText = dataPropertyName;
                
                switch(column.Type) {
                    case "String":
                    case "Effort":
                        CustomNodeTextBox textEditor = new CustomNodeTextBox();
                        ConfigureEditor(textEditor, dataPropertyName);
                        textEditor.IsColumnReadOnly = column.ReadOnly;
                        textEditor.ParentColumn = treeColumn;
                        textEditor.IsEditEnabledValueNeeded += CheckCellEditability;
                        tvWorkitems.NodeControls.Add(textEditor);
                        break;
                    case "List":
                        NodeComboBox listEditor = new NodeComboBox();
                        ConfigureEditor(listEditor, dataPropertyName);
                        listEditor.EditEnabled = !column.ReadOnly;
                        listEditor.ParentColumn = treeColumn;
                        listEditor.IsEditEnabledValueNeeded += CheckCellEditability;
                        tvWorkitems.NodeControls.Add(listEditor);
                        break;
                    case "Multi":
                        NodeListBox listBoxEditor = new NodeListBox();
                        listBoxEditor.ParentTree = tvWorkitems;
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
                NodeStateIcon nodeIcon = new NodeStateIcon();
                nodeIcon.DataPropertyName = "Icon";
                nodeIcon.ParentColumn = tvWorkitems.Columns[0];
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
            bool selectedNodeExists = tvWorkitems.SelectedNode != null;

            Workitem item = selectedNodeExists ? ((WorkitemDescriptor) tvWorkitems.SelectedNode.Tag).Workitem : null;
            UpdateMenuItemsVisibility(item);
        }

        private void AddTask_Click(object sender, EventArgs e) {
            controller.AddTask();
        }

        private void AddDefect_Click(object sender, EventArgs e) {
            controller.AddDefect();
        }

        private void AddTest_Click(object sender, EventArgs e) {
            controller.AddTest();
        }

        private void tvWorkitems_SelectionChanged(object sender, EventArgs e) {
            controller.HandleTreeSelectionChanged();
        }

        private void btnShowMyTasks_CheckedChanged(object sender, EventArgs e) {
            tvWorkitems.HideEditor();
            controller.HandleFilteringByOwner(btnOnlyMyTasks.Checked);
        }

        public void toolBtnRefresh_Click(object sender, EventArgs e) {
            tvWorkitems.HideEditor();
            controller.HandleRefreshCommand();
        }

        private void toolBtnSave_Click(object sender, EventArgs e) {
            tvWorkitems.HideEditor();
            controller.HandleSaveCommand();
        }

        private void miRevert_Click(object sender, EventArgs e) {
            controller.RevertItem();
        }

        private void miSignup_Click(object sender, EventArgs e) {
            controller.SignupItem();
        }

        private void miSave_Click(object sender, EventArgs e) {
			controller.CommitItem();
        }

        private void miQuickClose_Click(object sender, EventArgs e) {
            controller.QuickCloseItem();
        }

        private void miClose_Click(object sender, EventArgs e) {
            controller.CloseItem();
        }

        private void CheckCellEditability(object sender, NodeControlValueEventArgs e) {
            WorkitemDescriptor descriptor = (WorkitemDescriptor)e.Node.Tag;
            Workitem item = descriptor.Workitem;

            string columnName = (sender as BaseTextControl).DataPropertyName;
            string propertyName = columnToAttributeMappings[columnName];
            bool isReadOnly = item.IsPropertyReadOnly(propertyName);
            if(sender is CustomNodeTextBox) {
                CustomNodeTextBox textBox = (CustomNodeTextBox) sender;
                textBox.IsPropertyReadOnly = isReadOnly;
                textBox.EditorContextMenu = textBox.IsReadOnly ? CreateReadonlyTextBoxContextMenu(textBox) : null;
            }

            if(sender is NodeComboBox) {
                NodeComboBox editor = (NodeComboBox) sender;
                PropertyValues dataSource = dataLayer.GetListPropertyValues(item.TypePrefix + propertyName);
                editor.DropDownItems.Clear();
                editor.DropDownItems.AddRange(dataSource);
            }

            if (sender is NodeListBox) {
                NodeListBox editor = (NodeListBox)sender;
                PropertyValues dataSource = dataLayer.GetListPropertyValues(item.TypePrefix + propertyName);
                editor.DropDownItems.Clear();
                editor.DropDownItems.AddRange(dataSource);
            }
        }

        #endregion


        private static ContextMenu CreateReadonlyTextBoxContextMenu(NodeTextBox textBox) {
            ContextMenu menu = new ContextMenu();
            MenuItem miCopyValue = new MenuItem("Copy");
            miCopyValue.Click += delegate { textBox.Copy(); };
            menu.MenuItems.Add(miCopyValue);
            return menu;
        }

        public void RefreshProperties() {
            TreeNodeAdv node = tvWorkitems.SelectedNode;
            WorkitemDescriptor descriptor = null;
            if (node != null) {
                WorkitemDescriptor oldDescriptor = (WorkitemDescriptor)node.Tag;
                descriptor = oldDescriptor.GetDetailedDescriptor(
                    Configuration.Instance.AssetDetail.GetColumns(oldDescriptor.Workitem.TypePrefix), 
                    PropertyUpdateSource.WorkitemPropertyView);
            }
            UpdatePropertyView(descriptor);
        }

        public void SetSelection() {
            if (tvWorkitems.SelectedNode != null) {
                return;
            }
            if (currentWorkitemId != null) {
                Predicate<TreeNodeAdv> matcher = delegate(TreeNodeAdv node) {
                    WorkitemDescriptor desc = node.Tag as WorkitemDescriptor;
                    return desc != null && desc.Workitem.Id == currentWorkitemId;
                };
                TreeNodeAdv selectedNode = tvWorkitems.FindNodeByMather(matcher);
                if (selectedNode != null) {
                    tvWorkitems.SelectedNode = selectedNode;
                    return;
                }
            }
            TreeNodeAdv root = tvWorkitems.Root;
            if (root.Children.Count > 0) {
                tvWorkitems.SelectedNode = root.Children[0];
            } else {
                ResetPropertyView();
            }
        }

        public void SelectWorkitem(Workitem item) {
            Predicate<TreeNodeAdv> matcher = delegate(TreeNodeAdv node) {
                                                 WorkitemDescriptor descriptor = node.Tag as WorkitemDescriptor;
                                                 return descriptor != null && item.Equals(descriptor.Workitem);
                                             };
            TreeNodeAdv foundNode = tvWorkitems.FindNodeByMather(matcher);
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