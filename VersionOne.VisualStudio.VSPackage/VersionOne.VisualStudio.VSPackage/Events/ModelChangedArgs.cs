using System;

namespace VersionOne.VisualStudio.VSPackage.Events {
    public class ModelChangedArgs : EventArgs {
        public readonly EventReceiver Receiver;
        public readonly EventContext Context;

        public ModelChangedArgs(EventReceiver receiver, EventContext context) {
            Receiver = receiver;
            Context = context;
        }
    }
}