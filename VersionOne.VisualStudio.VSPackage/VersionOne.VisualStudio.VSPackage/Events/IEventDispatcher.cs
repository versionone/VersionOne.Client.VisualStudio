using System;

namespace VersionOne.VisualStudio.VSPackage.Events {
    public interface IEventDispatcher {
        event EventHandler<ModelChangedArgs> ModelChanged;
        void Notify(object sender, ModelChangedArgs e);
    }
}