using System;
using VersionOne.SDK.APIClient;
using VersionOne.VisualStudio.DataLayer.Entities;

namespace VersionOne.VisualStudio.DataLayer {
    public class VirtualWorkitem : Workitem {
        internal VirtualWorkitem(Asset asset, Workitem parent, IEntityContainer entityContainer) : base(asset, parent, entityContainer) { }

        public override bool IsVirtual {
            get { return true; }
        }

        public override bool CanQuickClose {
            get { return false; }
        }

        public override bool CanSignup {
            get { return false; }
        }

        public override void Close() {
            throw new NotSupportedException("Cannot close non-saved workitem.");
        }

        public override void QuickClose() {
            throw new NotSupportedException("Cannot close non-saved workitem.");
        }

        public override void Signup() {
            throw new NotSupportedException("Cannot signup to non-saved workitem.");
        }

        public override void CommitChanges() {
            try {
                EntityContainer.Commit(this);
                
                foreach (var child in Children) {
                    child.SetProperty("Parent", Asset.Oid);
                }

                EntityContainer.Refresh(this);
            } catch (APIException ex) {
                Logger.Error("Failed to commit changes.", ex);
            }
        }

        public override void RevertChanges() {
            EntityContainer.Cleanup(this);
        }

        public override bool Equals(object obj) {
            if(obj == null || obj.GetType() != typeof(VirtualWorkitem)) {
                return false;
            }

            var another = (VirtualWorkitem) obj;
            return Asset.Equals(another.Asset);
        }

        public override int GetHashCode() {
            return Asset.GetHashCode();
        }
    }
}