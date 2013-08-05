using System;

namespace VersionOne.VisualStudio.VSPackage.Events {
    public class EventDispatcher : IEventDispatcher {
        public event EventHandler<ModelChangedArgs> ModelChanged;

        public void Notify(object sender, ModelChangedArgs e) {
            if(ModelChanged != null) {
                ModelChanged(sender, e);
            }
        }
    }
}