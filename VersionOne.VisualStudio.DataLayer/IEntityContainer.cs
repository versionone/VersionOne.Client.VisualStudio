using VersionOne.SDK.APIClient;
using VersionOne.VisualStudio.DataLayer.Entities;

namespace VersionOne.VisualStudio.DataLayer {
    public interface IEntityContainer {
        void Cleanup(Workitem item);
        Asset Refresh(Workitem item);
        void AddEffort(Entity item, double newValue);
        double? GetEffort(Entity item);
        void Commit(Workitem item);
        void Revert(Workitem item);
    }
}