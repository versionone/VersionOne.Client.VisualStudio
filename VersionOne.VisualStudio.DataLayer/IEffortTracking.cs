using VersionOne.VisualStudio.DataLayer.Entities;

namespace VersionOne.VisualStudio.DataLayer {
    public interface IEffortTracking {
        EffortTrackingLevel DefectTrackingLevel { get; }
        EffortTrackingLevel StoryTrackingLevel { get; }
        bool TrackEffort { get; }
        bool AreEffortTrackingPropertiesReadOnly(Workitem workitem);
        void Init();
        void Refresh();
    }
}