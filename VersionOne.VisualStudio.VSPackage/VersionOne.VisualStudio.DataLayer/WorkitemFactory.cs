using System;
using VersionOne.SDK.APIClient;
using VersionOne.VisualStudio.DataLayer.Entities;

namespace VersionOne.VisualStudio.DataLayer {
    internal static class WorkitemFactory {
        public static Workitem CreateWorkitem(AssetFactory assetFactory, string assetType, Workitem parent, IEntityContainer entityContainer) {
            Asset asset;

            switch (assetType) {
                case Entity.TaskType:
                case Entity.TestType:
                    asset = assetFactory.CreateAssetForSecondaryWorkitem(assetType, parent);
                    var workitem = CreateWorkitem(asset, parent, entityContainer);
                    parent.Children.Add(workitem);
                    break;
                case Entity.DefectType:
                    asset = assetFactory.CreateAssetForPrimaryWorkitem(Entity.DefectType);
                    break;
                default:
                    throw new NotSupportedException(assetType + " is not supported.");
            }

            return CreateWorkitem(asset, parent, entityContainer);
        }

        public static Project CreateProject(Asset asset, Project parent) {
            return new Project(asset, parent);
        }

        public static Workitem CreateWorkitem(Asset asset, Workitem parent, IEntityContainer entityContainer) {
            return asset.Oid.IsNull ? CreateVirtualWorkitem(asset, parent, entityContainer) : CreatePersistentWorkitem(asset, parent, entityContainer);
        }

        private static Workitem CreatePersistentWorkitem(Asset asset, Workitem parent, IEntityContainer entityContainer) {
            return new Workitem(asset, parent, entityContainer);
        }

        private static Workitem CreateVirtualWorkitem(Asset asset, Workitem parent, IEntityContainer entityContainer) {
            return new VirtualWorkitem(asset, parent, entityContainer);
        }
    }
}