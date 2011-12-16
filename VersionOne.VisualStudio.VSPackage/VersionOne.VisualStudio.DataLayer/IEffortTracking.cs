namespace VersionOne.VisualStudio.DataLayer {
    public interface IEffortTracking {
        bool RequiredReload { get; }
        EffortTrackingLevel DefectTrackingLevel { get; }
        EffortTrackingLevel StoryTrackingLevel { get; }
        bool TrackEffort { get; }
        void Init();
        void Drop();
    }
}