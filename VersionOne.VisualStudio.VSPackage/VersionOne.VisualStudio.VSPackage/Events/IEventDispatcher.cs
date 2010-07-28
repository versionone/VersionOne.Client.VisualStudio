using System;

namespace VersionOne.VisualStudio.VSPackage.Events {
    public interface IEventDispatcher {
        event EventHandler<ModelChangedArgs> ModelChanged;
        event EventHandler<WorkitemPropertiesUpdatedArgs> WorkitemPropertiesUpdated;

        void InvokeModelChanged(object sender, ModelChangedArgs e);
        void InvokeWorkitemPropertiesUpdated(object sender, WorkitemPropertiesUpdatedArgs e);
    }
}