using System;
using VersionOne.SDK.APIClient;
using VersionOne.VisualStudio.DataLayer.Entities;

namespace VersionOne.VisualStudio.DataLayer {
    public class VirtualWorkitem : Workitem {
        internal VirtualWorkitem(Asset asset, Workitem parent, IEntityContainer entityContainer) : base(asset, parent, entityContainer) {
            this.IsVirtual = true;
        }

        public override bool IsVirtual
        {
            get;
            set;
        }

        public override bool CanQuickClose {
            get {
                if (this.IsVirtual)
                {
                    return false;
                }
                else
                {
                    return base.CanQuickClose;
                }
            
            }
        }

        public override bool CanSignup {
            get {
                if (this.IsVirtual)
                {
                    return false;
                }
                else
                {
                    return base.CanSignup;
                }
            }
        }

        public override void Close() {
            if (this.IsVirtual)
            {
                return;
            }
            else
            {
                base.Close();
            }
        }

        public override void QuickClose() {

            if (this.IsVirtual)
            {
                return;
            }
            else
            {
                base.QuickClose();
            }
        }

        public override void Signup() {
            if (this.IsVirtual)
            {
                return;
            }
            else
            {
                base.Signup();
            }
        }

        public override void CommitChanges() {
            try {
                EntityContainer.Commit(this);
                
                foreach (var child in Children) {
                    child.SetProperty("Parent", Asset.Oid);
                }

                this.Asset = EntityContainer.Refresh(this);
            } catch (APIException ex) {
                Logger.Error("Failed to commit changes.", ex);
            }
        }

        public Workitem ToWorkitem()
        {
            return new Workitem(this.Asset, this.Parent, EntityContainer);
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