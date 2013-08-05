using System;

namespace VersionOne.VisualStudio.VSPackage.Events {
    public class WorkitemPropertiesUpdatedArgs : EventArgs {
        public readonly PropertyUpdateSource Source;

        public WorkitemPropertiesUpdatedArgs(PropertyUpdateSource source) {
            Source = source;
        }
    }
}