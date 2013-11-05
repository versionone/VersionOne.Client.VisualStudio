using System.Linq;
using System.Collections.Generic;
using VersionOne.SDK.APIClient;
using VersionOne.VisualStudio.DataLayer.Entities;

namespace VersionOne.VisualStudio.DataLayer {
    public class AssetCache : IAssetCache {
        private readonly string[] allowedTypeTokens = {Entity.StoryType, Entity.DefectType, Entity.TaskType, Entity.TestType};

        private IList<Asset> AllAssets { get; set; }
        internal readonly IDictionary<Asset, double> Efforts = new Dictionary<Asset, double>();
        
        private readonly IDataLayerInternal dataLayer;

        public bool IsSet { get { return AllAssets != null; } }

        internal AssetCache(IDataLayerInternal dataLayer) {
            this.dataLayer = dataLayer;
        }

        internal void Set(IEnumerable<Asset> assets) {
            AllAssets = assets.Where(asset => allowedTypeTokens.Contains(asset.AssetType.Token)).ToList();
        }

        public void Drop() {
            AllAssets = null;
            Efforts.Clear();
        }

        public void Add(Workitem item) {
            AllAssets.Add(item.Asset);
        }

        public IList<Workitem> GetWorkitems(bool showAll) {
            return AllAssets.Where(asset => showAll || dataLayer.AssetPassesShowMyTasksFilter(asset))
                            .Select(asset => WorkitemFactory.CreateWorkitem(asset, null, this))
                            .ToList();
        }

        public AssetCache ToInternalCache() {
            return this;
        }

        #region IEntityContainer methods - called by contained items

        public void Cleanup(Workitem item) {
            if(item.Parent != null && AllAssets.Contains(item.Parent.Asset)) {
                item.Parent.Asset.Children.Remove(item.Asset);
            }

            AllAssets.Remove(item.Asset);
            Efforts.Remove(item.Asset);
            
            foreach(var child in item.Asset.Children) {
                Efforts.Remove(child);
            }
        }

        public Asset Refresh(Workitem item) {
            return dataLayer.RefreshAsset(item, AllAssets);
        }

        public void AddEffort(Entity item, double newValue) {
            if(Efforts.ContainsKey(item.Asset)) {
                if(newValue.CompareTo(0) == 0) {
                    Efforts.Remove(item.Asset);
                } else {
                    Efforts[item.Asset] = newValue;
                }
            } else {
                if(newValue.CompareTo(0) != 0) {
                    Efforts.Add(item.Asset, newValue);
                }
            }
        }

        public double? GetEffort(Entity item) {
            return Efforts.ContainsKey(item.Asset) ? Efforts[item.Asset] : (double?) null;
        }

        public void Commit(Workitem item) {
            dataLayer.CommitAsset(Efforts, item.Asset);
        }

        public void Revert(Workitem item) {
            item.Asset.RejectChanges();
            Efforts.Remove(item.Asset);
        }

        #endregion
    }
}