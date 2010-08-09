using System;
using VersionOne.SDK.APIClient;

namespace VersionOne.VisualStudio.DataLayer {
    public class VirtualWorkitem : Workitem {
        internal VirtualWorkitem(Asset asset, Workitem parent) : base(asset, parent) { }

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
                dataLayer.CommitAsset(Asset);
                foreach (Workitem child in Children) {
                    child.SetProperty("Parent", Asset.Oid);
                }
                dataLayer.RefreshAsset(this);
            } catch (APIException ex) {
                Logger.Error("Failed to commit changes.", ex);
            }
        }

        public override void RevertChanges() {
            dataLayer.CleanupWorkitem(this);
        }

        public override bool Equals(object obj) {
            if(obj == null || obj.GetType() != typeof(VirtualWorkitem)) {
                return false;
            }

            VirtualWorkitem another = (VirtualWorkitem) obj;
            return Asset.Equals(another.Asset);
        }

        public override int GetHashCode() {
            return Asset.GetHashCode();
        }
    }
}
