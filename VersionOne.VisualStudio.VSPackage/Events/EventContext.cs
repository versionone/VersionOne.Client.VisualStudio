namespace VersionOne.VisualStudio.VSPackage.Events {
    public enum EventContext {
        V1SettingsChanged,
        ProjectsRequested,
        ProjectSelected,
        WorkitemsRequested,
        WorkitemsChanged,
        WorkitemPropertiesUpdatedFromView,
        WorkitemPropertiesUpdatedFromPropertyView,
        WorkitemCacheInvalidated,
        ProjectPropertiesUpdated,
    }
}