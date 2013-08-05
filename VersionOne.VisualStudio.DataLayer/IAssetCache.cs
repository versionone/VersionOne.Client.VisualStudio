using System.Collections.Generic;
using VersionOne.VisualStudio.DataLayer.Entities;

namespace VersionOne.VisualStudio.DataLayer {
    public interface IAssetCache : IEntityContainer {
        bool IsSet { get; }
        void Drop();
        void Add(Workitem item);
        IList<Workitem> GetWorkitems(bool showAll);
        AssetCache ToInternalCache();
    }
}