using System;

namespace VersionOne.VisualStudio.VSPackage.Events {
    public class EventDispatcher : IEventDispatcher {
        private static IEventDispatcher instance;

        public static IEventDispatcher Instance {
            get { return instance ?? (instance = new EventDispatcher()); }
        }

        private EventDispatcher() {}

        public event EventHandler<ModelChangedArgs> ModelChanged;

        public void Notify(object sender, ModelChangedArgs e) {
            if(ModelChanged != null) {
                ModelChanged(sender, e);
            }
        }
    }
}