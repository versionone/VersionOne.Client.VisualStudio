using System.Windows.Forms;
using System.Drawing;
using Aga.Controls.Tree;

namespace VersionOne.VisualStudio.VSPackage.Controls {
    partial class WorkitemTreeControl {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing) {
            if (disposing) {
                if (components != null) {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.tsMenu = new System.Windows.Forms.ToolStrip();
            this.btnRefresh = new System.Windows.Forms.ToolStripButton();
            this.btnSave = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnOnlyMyTasks = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnAddStory = new System.Windows.Forms.ToolStripButton();
            this.btnAddDefect = new System.Windows.Forms.ToolStripButton();
            this.btnAddTask = new System.Windows.Forms.ToolStripButton();
            this.btnAddTest = new System.Windows.Forms.ToolStripButton();
            this.ctxTreeViewMenu = new System.Windows.Forms.ContextMenu();
            this.miSave = new System.Windows.Forms.MenuItem();
            this.miRevert = new System.Windows.Forms.MenuItem();
            this.miSeparator2 = new System.Windows.Forms.MenuItem();
            this.miSignup = new System.Windows.Forms.MenuItem();
            this.miSeparator3 = new System.Windows.Forms.MenuItem();
            this.miClose = new System.Windows.Forms.MenuItem();
            this.miQuickClose = new System.Windows.Forms.MenuItem();
            this.miSeparator4 = new System.Windows.Forms.MenuItem();
            this.miNewTask = new System.Windows.Forms.MenuItem();
            this.miNewDefect = new System.Windows.Forms.MenuItem();
            this.miNewTest = new System.Windows.Forms.MenuItem();
            this.miSeparator5 = new System.Windows.Forms.MenuItem();
            this.miProperties = new System.Windows.Forms.MenuItem();
            this.miOpenInVersionOne = new System.Windows.Forms.MenuItem();
            this.lblLoading = new System.Windows.Forms.Label();
            this.tvWorkitems = new Aga.Controls.Tree.TreeViewAdv();
            this.tsMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // tsMenu
            // 
            this.tsMenu.GripMargin = new System.Windows.Forms.Padding(0);
            this.tsMenu.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnRefresh,
            this.btnSave,
            this.toolStripSeparator1,
            this.btnOnlyMyTasks,
            this.toolStripSeparator2,
            this.btnAddStory,
            this.btnAddDefect,
            this.btnAddTask,
            this.btnAddTest});
            this.tsMenu.Location = new System.Drawing.Point(0, 0);
            this.tsMenu.Name = "tsMenu";
            this.tsMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.tsMenu.Size = new System.Drawing.Size(783, 25);
            this.tsMenu.TabIndex = 2;
            this.tsMenu.Text = "tsMenu";
            // 
            // btnRefresh
            // 
            this.btnRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnRefresh.Image = global::VersionOne.VisualStudio.VSPackage.Resources.RefreshIcon;
            this.btnRefresh.ImageTransparentColor = System.Drawing.Color.Black;
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnRefresh.Size = new System.Drawing.Size(23, 22);
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnRefresh.ToolTipText = "Refresh";
            // 
            // btnSave
            // 
            this.btnSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnSave.Image = global::VersionOne.VisualStudio.VSPackage.Resources.SaveIcon;
            this.btnSave.ImageTransparentColor = System.Drawing.Color.Black;
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(23, 22);
            this.btnSave.Text = "Save";
            this.btnSave.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnSave.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnSave.ToolTipText = "Save";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // btnOnlyMyTasks
            // 
            this.btnOnlyMyTasks.CheckOnClick = true;
            this.btnOnlyMyTasks.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnOnlyMyTasks.Image = global::VersionOne.VisualStudio.VSPackage.Resources.MemberIcon;
            this.btnOnlyMyTasks.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnOnlyMyTasks.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnOnlyMyTasks.Name = "btnOnlyMyTasks";
            this.btnOnlyMyTasks.Size = new System.Drawing.Size(23, 22);
            this.btnOnlyMyTasks.Text = "Show only my tasks";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // btnAddStory
            // 
            this.btnAddStory.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnAddStory.Enabled = false;
            this.btnAddStory.Image = global::VersionOne.VisualStudio.VSPackage.Resources.AddStoryIcon;
            this.btnAddStory.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnAddStory.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAddStory.Name = "btnAddStory";
            this.btnAddStory.Size = new System.Drawing.Size(23, 22);
            this.btnAddStory.Text = "Add Story";
            this.btnAddStory.Visible = false;
            // 
            // btnAddDefect
            // 
            this.btnAddDefect.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnAddDefect.Enabled = false;
            this.btnAddDefect.Image = global::VersionOne.VisualStudio.VSPackage.Resources.AddDefectIcon;
            this.btnAddDefect.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnAddDefect.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAddDefect.Name = "btnAddDefect";
            this.btnAddDefect.Size = new System.Drawing.Size(23, 22);
            this.btnAddDefect.Text = "Add Defect";
            // 
            // btnAddTask
            // 
            this.btnAddTask.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnAddTask.Enabled = false;
            this.btnAddTask.Image = global::VersionOne.VisualStudio.VSPackage.Resources.AddTaskIcon;
            this.btnAddTask.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnAddTask.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAddTask.Name = "btnAddTask";
            this.btnAddTask.Size = new System.Drawing.Size(23, 22);
            this.btnAddTask.Text = "Add Task";
            // 
            // btnAddTest
            // 
            this.btnAddTest.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnAddTest.Enabled = false;
            this.btnAddTest.Image = global::VersionOne.VisualStudio.VSPackage.Resources.AddTestIcon;
            this.btnAddTest.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnAddTest.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAddTest.Name = "btnAddTest";
            this.btnAddTest.Size = new System.Drawing.Size(24, 22);
            this.btnAddTest.Text = "Add Test";
            // 
            // ctxTreeViewMenu
            // 
            this.ctxTreeViewMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.miSave,
            this.miRevert,
            this.miSeparator2,
            this.miSignup,
            this.miSeparator3,
            this.miClose,
            this.miQuickClose,
            this.miSeparator4,
            this.miNewTask,
            this.miNewDefect,
            this.miNewTest,
            this.miSeparator5,
            this.miProperties,
            this.miOpenInVersionOne});
            // 
            // miSave
            // 
            this.miSave.Index = 0;
            this.miSave.Text = "Save";
            // 
            // miRevert
            // 
            this.miRevert.Index = 1;
            this.miRevert.Text = "Revert changes";
            // 
            // miSeparator2
            // 
            this.miSeparator2.Index = 2;
            this.miSeparator2.Text = "-";
            // 
            // miSignup
            // 
            this.miSignup.Index = 3;
            this.miSignup.Text = "Sign Me Up";
            // 
            // miSeparator3
            // 
            this.miSeparator3.Index = 4;
            this.miSeparator3.Text = "-";
            // 
            // miClose
            // 
            this.miClose.Index = 5;
            this.miClose.Text = "Close";
            // 
            // miQuickClose
            // 
            this.miQuickClose.Index = 6;
            this.miQuickClose.Text = "Quick Close";
            // 
            // miSeparator4
            // 
            this.miSeparator4.Index = 7;
            this.miSeparator4.Text = "-";
            // 
            // miNewTask
            // 
            this.miNewTask.Index = 8;
            this.miNewTask.Text = "New Task";
            // 
            // miNewDefect
            // 
            this.miNewDefect.Index = 9;
            this.miNewDefect.Text = "New Defect";
            // 
            // miNewTest
            // 
            this.miNewTest.Index = 10;
            this.miNewTest.Text = "New Test";
            //
            //miSeparator5
            //
            this.miSeparator5.Index = 11;
            this.miSeparator5.Text = "-";
            //
            //miProperties
            //
            this.miProperties.Index = 12;
            this.miProperties.Text = "Properties";
            //
            //miOpenInVersionOne
            //
            this.miOpenInVersionOne.Index = 13;
            this.miOpenInVersionOne.Text = "Open in VersionOne";
            // 
            // lblLoading
            // 
            this.lblLoading.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblLoading.Location = new System.Drawing.Point(256, 71);
            this.lblLoading.Name = "lblLoading";
            this.lblLoading.Size = new System.Drawing.Size(137, 89);
            this.lblLoading.TabIndex = 0;
            this.lblLoading.Text = "Loading...";
            this.lblLoading.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tvWorkitems
            // 
            this.tvWorkitems.BackColor = System.Drawing.SystemColors.Window;
            this.tvWorkitems.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tvWorkitems.ContextMenu = this.ctxTreeViewMenu;
            this.tvWorkitems.DefaultToolTipProvider = null;
            this.tvWorkitems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvWorkitems.DragDropMarkColor = System.Drawing.Color.Black;
            this.tvWorkitems.FullRowSelect = true;
            this.tvWorkitems.GridLineStyle = ((Aga.Controls.Tree.GridLineStyle)((Aga.Controls.Tree.GridLineStyle.Horizontal | Aga.Controls.Tree.GridLineStyle.Vertical)));
            this.tvWorkitems.LineColor = System.Drawing.SystemColors.ControlDark;
            this.tvWorkitems.Location = new System.Drawing.Point(0, 25);
            this.tvWorkitems.Margin = new System.Windows.Forms.Padding(0);
            this.tvWorkitems.Model = null;
            this.tvWorkitems.Name = "tvWorkitems";
            this.tvWorkitems.SelectedNode = null;
            this.tvWorkitems.Size = new System.Drawing.Size(783, 422);
            this.tvWorkitems.TabIndex = 4;
            this.tvWorkitems.Text = "treeViewAdv1";
            this.tvWorkitems.UseColumns = true;
            // 
            // WorkitemTreeControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.tvWorkitems);
            this.Controls.Add(this.tsMenu);
            this.Controls.Add(this.lblLoading);
            this.ForeColor = System.Drawing.SystemColors.Control;
            this.Name = "WorkitemTreeControl";
            this.Size = new System.Drawing.Size(783, 447);
            this.tsMenu.ResumeLayout(false);
            this.tsMenu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ToolStrip tsMenu;
		private ToolStripButton btnRefresh;
        private ToolStripButton btnSave;
        private ToolStripSeparator toolStripSeparator1;
		private ContextMenu ctxTreeViewMenu;
        private Label lblLoading;
        private TreeViewAdv tvWorkitems;
        private MenuItem miSave;
        private MenuItem miRevert;
        private MenuItem miSeparator2;
        private MenuItem miSignup;
        private MenuItem miSeparator3;
        private MenuItem miClose;
        private MenuItem miQuickClose;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripButton btnAddStory;
        private ToolStripButton btnAddDefect;
        private ToolStripButton btnAddTask;
        private ToolStripButton btnAddTest;
        private ToolStripButton btnOnlyMyTasks;
        private MenuItem miSeparator4;
        private MenuItem miNewTask;
        private MenuItem miNewDefect;
        private MenuItem miNewTest;
        private MenuItem miSeparator5;
        private MenuItem miProperties;
        private MenuItem miOpenInVersionOne;
    }
}
