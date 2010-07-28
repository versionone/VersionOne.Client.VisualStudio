namespace VersionOne.VisualStudio.DataLayer {
    public enum EffortTrackingLevel {
        PrimaryWorkitem = 1,
        SecondaryWorkitem = 2,
        Both = PrimaryWorkitem | SecondaryWorkitem,
    }
}
