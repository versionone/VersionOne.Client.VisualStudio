using System;
using VersionOne.SDK.APIClient;
using VersionOne.VisualStudio.DataLayer.Entities;

namespace VersionOne.VisualStudio.DataLayer {
    internal class WorkitemFactory {
        private static WorkitemFactory instance;

        public static WorkitemFactory Instance {
            get { return instance ?? (instance = new WorkitemFactory()); }
        }

        public Workitem CreateWorkitem(AssetFactory assetFactory, string assetType, Workitem parent) {
            Asset asset;

            switch (assetType) {
                case Entity.TaskPrefix:
                case Entity.TestPrefix:
                    asset = assetFactory.CreateAssetForSecondaryWorkitem(assetType, parent);
                    Workitem workitem = CreateWorkitem(asset, parent);
                    parent.Children.Add(workitem);
                    break;
                case Entity.DefectPrefix:
                    asset = assetFactory.CreateAssetForPrimaryWorkitem(Entity.DefectPrefix);
                    break;
                default:
                    throw new NotSupportedException(assetType + " is not supported.");
            }

            return CreateWorkitem(asset, parent);
        }

        public Project CreateProject(Asset asset, Project parent) {
            return new Project(asset, parent);
        }

        public Workitem CreateWorkitem(Asset asset, Workitem parent) {
            if(asset.Oid.IsNull) {
                return CreateVirtualWorkitem(asset, parent);
            } 

            return CreatePersistentWorkitem(asset, parent);
        }

        private static Workitem CreatePersistentWorkitem(Asset asset, Workitem parent) {
            return new Workitem(asset, parent);
        }

        private static Workitem CreateVirtualWorkitem(Asset asset, Workitem parent) {
            return new VirtualWorkitem(asset, parent);
        }
    }
}