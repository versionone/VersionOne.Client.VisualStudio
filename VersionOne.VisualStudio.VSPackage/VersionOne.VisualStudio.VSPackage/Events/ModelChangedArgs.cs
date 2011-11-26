using System;

namespace VersionOne.VisualStudio.VSPackage.Events {
    public class ModelChangedArgs : EventArgs {
        public readonly EventReceiver Receiver;
        public readonly EventContext Context;

        public ModelChangedArgs(EventReceiver receiver, EventContext context) {
            Receiver = receiver;
            Context = context;
        }

        public override bool Equals(object obj) {
            if(obj == null || obj.GetType() != GetType()) {
                return false;
            }

            var other = (ModelChangedArgs) obj;
            return other.Receiver == Receiver && other.Context == Context;
        }

        public override int GetHashCode() {
            return Receiver.GetHashCode() + Context.GetHashCode();
        }
    }
}