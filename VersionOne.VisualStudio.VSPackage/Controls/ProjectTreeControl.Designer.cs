using System.ComponentModel;

namespace VersionOne.VisualStudio.VSPackage.Controls {
    public partial class ProjectTreeControl {
        private IContainer components;

           #region Component Designer generated code
        private void InitializeComponent() {
            this.tvProjects = new System.Windows.Forms.TreeView();
            this.lblLoading = new System.Windows.Forms.Label();
            this.tsMenu = new System.Windows.Forms.ToolStrip();
            this.btnRefresh = new System.Windows.Forms.ToolStripButton();
            this.tsMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // tvProjects
            // 
            this.tvProjects.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tvProjects.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvProjects.FullRowSelect = true;
            this.tvProjects.HideSelection = false;
            this.tvProjects.Location = new System.Drawing.Point(0, 25);
            this.tvProjects.Name = "tvProjects";
            this.tvProjects.Size = new System.Drawing.Size(182, 303);
            this.tvProjects.TabIndex = 1;
            // 
            // lblLoading
            // 
            this.lblLoading.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblLoading.Location = new System.Drawing.Point(0, 0);
            this.lblLoading.Name = "lblLoading";
            this.lblLoading.Size = new System.Drawing.Size(132, 201);
            this.lblLoading.TabIndex = 2;
            this.lblLoading.Text = "Loading...";
            this.lblLoading.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tsMenu
            // 
            this.tsMenu.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnRefresh});
            this.tsMenu.Location = new System.Drawing.Point(0, 0);
            this.tsMenu.Name = "tsMenu";
            this.tsMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.tsMenu.Size = new System.Drawing.Size(182, 25);
            this.tsMenu.TabIndex = 0;
            this.tsMenu.Text = "toolStrip1";
            // 
            // btnRefresh
            // 
            this.btnRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnRefresh.Image = global::VersionOne.VisualStudio.VSPackage.Resources.RefreshIcon;
            this.btnRefresh.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnRefresh.ImageTransparentColor = System.Drawing.Color.Black;
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(23, 22);
            this.btnRefresh.Text = "Refresh";
            // 
            // ProjectTreeControl
            // 
            this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.Controls.Add(this.tvProjects);
            this.Controls.Add(this.tsMenu);
            this.Controls.Add(this.lblLoading);
            this.Name = "ProjectTreeControl";
            this.Size = new System.Drawing.Size(182, 328);
            this.tsMenu.ResumeLayout(false);
            this.tsMenu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        protected override void Dispose(bool disposing) {
            if (disposing && components != null)
                components.Dispose();
            base.Dispose(disposing);
        }
        #endregion

		private System.Windows.Forms.ToolStrip tsMenu;
		private System.Windows.Forms.ToolStripButton btnRefresh;
    }
}
