using System.Collections.Generic;
using VersionOne.SDK.APIClient;
using VersionOne.VisualStudio.DataLayer.Entities;
using VersionOne.VisualStudio.DataLayer.Logging;

namespace VersionOne.VisualStudio.DataLayer {
    public interface IDataLayerInternal : IDataLayer {
        bool AssetPassesShowMyTasksFilter(Asset asset);

        Oid MemberOid { get; }

        void ExecuteOperation(Asset asset, IOperation operation);
        void CommitAsset(IDictionary<Asset, double> efforts, Asset asset);
        Asset RefreshAsset(Workitem workitem, IList<Asset> containingAssetCollection);

        IAssetType ProjectType { get; }
        IAssetType TaskType { get; }
        IAssetType TestType { get; }
        IAssetType DefectType { get; }
        IAssetType StoryType { get; }
        IDictionary<string, IAssetType> Types { get; }

        /// <summary>
        /// Get logger currently used in DataLayer
        /// </summary>
        ILogger Logger { get; }
    }
}