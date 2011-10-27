using System;
using VersionOne.SDK.APIClient;
using VersionOne.VisualStudio.DataLayer.Entities;

namespace VersionOne.VisualStudio.DataLayer {
    internal static class WorkitemFactory {
        public static Workitem CreateWorkitem(AssetFactory assetFactory, string assetType, Workitem parent) {
            Asset asset;

            switch (assetType) {
                case Entity.TaskPrefix:
                case Entity.TestPrefix:
                    asset = assetFactory.CreateAssetForSecondaryWorkitem(assetType, parent);
                    var workitem = CreateWorkitem(asset, parent);
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

        public static Project CreateProject(Asset asset, Project parent) {
            return new Project(asset, parent);
        }

        public static Workitem CreateWorkitem(Asset asset, Workitem parent) {
            return asset.Oid.IsNull ? CreateVirtualWorkitem(asset, parent) : CreatePersistentWorkitem(asset, parent);
        }

        private static Workitem CreatePersistentWorkitem(Asset asset, Workitem parent) {
            return new Workitem(asset, parent);
        }

        private static Workitem CreateVirtualWorkitem(Asset asset, Workitem parent) {
            return new VirtualWorkitem(asset, parent);
        }
    }
}