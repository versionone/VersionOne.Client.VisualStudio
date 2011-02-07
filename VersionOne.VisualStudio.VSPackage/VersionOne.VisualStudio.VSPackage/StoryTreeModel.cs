using System;
using System.Collections;
using System.Collections.Generic;

using Aga.Controls.Tree;

using VersionOne.VisualStudio.DataLayer;
using VersionOne.VisualStudio.DataLayer.Entities;
using VersionOne.VisualStudio.VSPackage.Descriptors;
using VersionOne.VisualStudio.VSPackage.Events;
using VersionOne.VisualStudio.VSPackage.Settings;

namespace VersionOne.VisualStudio.VSPackage {
    // warnings of unused events are unnecessary here because events are required to implement interface
    #pragma warning disable 67
    
    public class StoryTreeModel : ITreeModel {
        private readonly IDataLayer dataLayer;

        public StoryTreeModel(IDataLayer dataLayer) {
            this.dataLayer = dataLayer;
        }

        public void CommitVirtualItems() {
            //dataLayer.CommitWorkitems(VirtualItems);
        }

        public IEnumerable GetChildren(TreePath treePath) {
            if (!dataLayer.IsConnected) {
                return null;
            }
            if (treePath.IsEmpty()) {
                try {
                    return WrapWorkitems(dataLayer.GetWorkitems());
                } catch (DataLayerException) { 
                    //TODO show message?
                }
            } else {
                WorkitemDescriptor descriptor = (WorkitemDescriptor)treePath.LastNode;
                return WrapWorkitems(descriptor.Workitem.Children);
            }
            return new WorkitemDescriptor[0];
        }

        // TODO support primary items
        private List<WorkitemDescriptor> WrapWorkitems(IList<Workitem> workitems) {
            List<WorkitemDescriptor> items = new List<WorkitemDescriptor>(workitems.Count);
            foreach (Workitem workitem in workitems) {
                WorkitemDescriptor descriptor = new WorkitemDescriptor(workitem, Configuration.Instance.GridSettings.Columns, PropertyUpdateSource.WorkitemView, false);
                items.Add(descriptor);
            }
            
            return items;
        }
        
        public bool IsLeaf(TreePath treePath) {
            WorkitemDescriptor descriptor = (WorkitemDescriptor)treePath.LastNode;
            return descriptor.Workitem.Children.Count == 0;
        }

        public event EventHandler<TreeModelEventArgs> NodesChanged;
        public event EventHandler<TreeModelEventArgs> NodesInserted;
        public event EventHandler<TreeModelEventArgs> NodesRemoved;
        public event EventHandler<TreePathEventArgs> StructureChanged;

        public void InvokeStructureChanged(){
            StructureChanged(this, new TreePathEventArgs());
        }
    }

    #pragma warning restore 67
}