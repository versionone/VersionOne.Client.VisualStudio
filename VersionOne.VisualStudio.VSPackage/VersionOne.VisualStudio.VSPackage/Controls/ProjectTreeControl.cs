using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using VersionOne.VisualStudio.DataLayer.Entities;
using VersionOne.VisualStudio.VSPackage.Controllers;
using VersionOne.VisualStudio.DataLayer;
using VersionOne.VisualStudio.VSPackage.Descriptors;
using VersionOne.VisualStudio.VSPackage.Events;
using VersionOne.VisualStudio.VSPackage.Settings;

using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace VersionOne.VisualStudio.VSPackage.Controls {
    [Guid("e450464a-c543-4d91-9a09-1d6cff120c06")]
    public partial class ProjectTreeControl : V1UserControl, IProjectTreeView {
        private ProjectTreeController controller;
        
        private TreeView tvProjects;
        private Label lblLoading;
        
        private bool updating;

        public ProjectTreeController Controller {
            get { return controller; }
            set { controller = value; }
        }

        public ProjectTreeControl(ProjectsWindow parent) : base(parent) {
            InitializeComponent();
            tvProjects.AfterSelect += tvProjects_AfterSelect;
            btnRefresh.Click += btnRefresh_Click;
            
            VisibleChanged += delegate { RefreshProperties(); };
            CursorChanged += delegate { RefreshProperties(); };
            Enter += delegate { RefreshProperties(); };
        }

        private void btnRefresh_Click(object sender, EventArgs e) {
            controller.HandleRefreshAction();
        }

        private void tvProjects_AfterSelect(object sender, TreeViewEventArgs e) {
            Project project = (Project) e.Node.Tag;
            controller.HandleProjectSelected(project);
        }

        public void UpdateData() {
            try {
                if(updating) {
                    return;
                }
                updating = true;
                tvProjects.BeginUpdate();
                tvProjects.Nodes.Clear();
                if (!CheckSettingsAreValid()) {
                    ResetPropertyView();
                } else {
                    TreeNode node = AddNodesRecursively(tvProjects.Nodes, dataLayer.GetProjectTree());
                    if (node == null && tvProjects.Nodes.Count > 0) {
                         node = tvProjects.Nodes[0];
                    }
                    tvProjects.SelectedNode = node;
                }
                tvProjects.EndUpdate();
                updating = false;
            } catch (DataLayerException) {
				ResetPropertyView();
            }
        }

        private static TreeNode AddNodesRecursively(TreeNodeCollection rootNodes, IEnumerable<Project> v1Roots) {
            TreeNode res = null;
            foreach (Project v1Root in v1Roots) {
                TreeNode node = rootNodes.Add(v1Root.GetProperty(Entity.NameProperty) as string);
                node.Tag = v1Root;
                TreeNode res2 = AddNodesRecursively(node.Nodes, v1Root.Children);
                if (res2 != null) {
                    res = res2;
                } else if (v1Root.Id == SettingsImpl.Instance.SelectedProjectId) {
                    res = node;
                }
            }
            return res;
        }

        public void RefreshProperties() {
            TreeNode node = tvProjects.SelectedNode;
            if (node != null) {
                UpdatePropertyView(new WorkitemDescriptor((Entity) node.Tag, Configuration.Instance.ProjectTree.Columns, PropertyUpdateSource.ProjectPropertyView, true));
            }
        }
    }
}