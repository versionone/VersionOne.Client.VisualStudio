using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Aga.Controls.Tree;
using VersionOne.VisualStudio.DataLayer.Entities;
using VersionOne.VisualStudio.VSPackage.Controllers;
using VersionOne.VisualStudio.VSPackage.Descriptors;
using VersionOne.VisualStudio.VSPackage.Events;
using VersionOne.VisualStudio.VSPackage.Settings;

namespace VersionOne.VisualStudio.VSPackage {
    public class StoryTreeModel : ITreeModel {
        private readonly WorkitemTreeController controller;
        private readonly Configuration configuration;

        public StoryTreeModel(WorkitemTreeController controller, Configuration configuration) {
            this.controller = controller;
            this.configuration = configuration;
        }

        public IEnumerable GetChildren(TreePath treePath) {
            if (!controller.CanRetrieveData) {
                return null;
            }

            if(treePath.IsEmpty()) {
                var workitems = controller.GetWorkitems();
                return workitems != null ? WrapWorkitems(workitems) : new WorkitemDescriptor[0];
            }

            var descriptor = (WorkitemDescriptor) treePath.LastNode;
            return WrapWorkitems(descriptor.Workitem.Children);
        }

        // TODO support primary items
        private IEnumerable<WorkitemDescriptor> WrapWorkitems(IEnumerable<Workitem> workitems) {
            return workitems.Select(workitem => new WorkitemDescriptor(workitem, configuration.GridSettings.Columns, PropertyUpdateSource.WorkitemView, false)).ToList();
        }
        
        public bool IsLeaf(TreePath treePath) {
            var descriptor = (WorkitemDescriptor) treePath.LastNode;
            return descriptor.Workitem.Children.Count == 0;
        }

#pragma warning disable 67

        public event EventHandler<TreeModelEventArgs> NodesChanged;
        public event EventHandler<TreeModelEventArgs> NodesInserted;
        public event EventHandler<TreeModelEventArgs> NodesRemoved;
        public event EventHandler<TreePathEventArgs> StructureChanged;

#pragma warning restore 67

        public void InvokeStructureChanged(){
            StructureChanged(this, new TreePathEventArgs());
        }
    }
}