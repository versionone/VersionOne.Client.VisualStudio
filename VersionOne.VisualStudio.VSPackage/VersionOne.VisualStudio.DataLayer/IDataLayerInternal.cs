using VersionOne.SDK.APIClient;
using VersionOne.VisualStudio.DataLayer.Logging;

namespace VersionOne.VisualStudio.DataLayer {
    internal interface IDataLayerInternal : IDataLayer {
        double? GetEffort(Asset asset);
        void AddEffort(Asset asset, double effort);

        bool IsEffortTrackingRelated(string propertyName);

        bool AssetPassesShowMyTasksFilter(Asset asset);

        /// <summary>
        /// Get logger currently used in DataLayer
        /// </summary>
        ILogger Logger { get; }
    }
}