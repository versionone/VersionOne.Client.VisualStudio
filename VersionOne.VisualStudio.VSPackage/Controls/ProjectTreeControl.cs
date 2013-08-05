using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Ninject;
using VersionOne.VisualStudio.DataLayer;
using VersionOne.VisualStudio.DataLayer.Entities;
using VersionOne.VisualStudio.VSPackage.Controllers;
using VersionOne.VisualStudio.VSPackage.Dependencies;
using VersionOne.VisualStudio.VSPackage.Descriptors;
using VersionOne.VisualStudio.VSPackage.Events;
using VersionOne.VisualStudio.VSPackage.Settings;

namespace VersionOne.VisualStudio.VSPackage.Controls {
    [Guid("e450464a-c543-4d91-9a09-1d6cff120c06")]
    public partial class ProjectTreeControl : V1UserControl, IProjectTreeView {        
        private TreeView tvProjects;
        private Label lblLoading;

        private readonly Configuration configuration;
        private readonly ISettings settings;
        private readonly ComponentResolver<IParentWindow> parentWindowResolver; 

        private bool updating;

        public IEnumerable<Project> Projects { get; set; } 

        public ProjectTreeController Controller { get; set; }

        public ProjectTreeControl(Configuration configuration, ISettings settings, IDataLayer dataLayer, [Named("Projects")] ComponentResolver<IParentWindow> parentWindowResolver) 
                : base(parentWindowResolver, dataLayer) {
            InitializeComponent();

            this.configuration = configuration;
            this.settings = settings;
            this.parentWindowResolver = parentWindowResolver;

            tvProjects.AfterSelect += tvProjects_AfterSelect;
            btnRefresh.Click += btnRefresh_Click;
            
            VisibleChanged += (sender, e) => RefreshProperties();
            CursorChanged += (sender, e) => RefreshProperties();
            Enter += (sender, e) => RefreshProperties();
        }

        private void btnRefresh_Click(object sender, EventArgs e) {
            Controller.HandleRefreshAction();
        }

        private void tvProjects_AfterSelect(object sender, TreeViewEventArgs e) {
            var project = (Project) e.Node.Tag;
            Controller.HandleProjectSelected(project);
        }

        public override void SetAccessibleControlsEnabled(bool enabled) {
            tsMenu.Enabled = enabled;
            tvProjects.Enabled = enabled;
        }

        public void UpdateData() {
            if(updating) {
                return;
            }

            updating = true;
            tvProjects.BeginUpdate();
            tvProjects.Nodes.Clear();

            if(!CheckSettingsAreValid()) {
                ResetPropertyView();
            } else {
                Controller.HandleProjectsRequest();
                return;
            }

            tvProjects.EndUpdate();
            updating = false;
        }

        public void CompleteProjectsRequest() {
            var node = AddNodesRecursively(tvProjects.Nodes, Projects);

            if(node == null && tvProjects.Nodes.Count > 0) {
                node = tvProjects.Nodes[0];
            }

            var project = node != null ? (Project) node.Tag : null;

            tvProjects.SelectedNode = node;
            tvProjects.EndUpdate();

            if(!parentWindowResolver.CanResolve && project != null) {
                Controller.HandleProjectSelected(project);
            }

            updating = false;
        }

        private TreeNode AddNodesRecursively(TreeNodeCollection rootNodes, IEnumerable<Project> v1Roots) {
            TreeNode res = null;

            foreach (var v1Root in v1Roots) {
                var node = rootNodes.Add(v1Root.GetProperty(Entity.NameProperty) as string);
                node.Tag = v1Root;
                var res2 = AddNodesRecursively(node.Nodes, v1Root.Children);
                
                if (res2 != null) {
                    res = res2;
                } else if (v1Root.Id == settings.SelectedProjectId) {
                    res = node;
                }
            }

            return res;
        }

        public void RefreshProperties() {
            var node = tvProjects.SelectedNode;
            
            if (node != null) {
                UpdatePropertyView(new WorkitemDescriptor((Entity) node.Tag, configuration.ProjectTree.Columns, PropertyUpdateSource.ProjectPropertyView, true));
            }
        }
    }
}