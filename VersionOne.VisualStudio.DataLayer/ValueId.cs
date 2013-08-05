using VersionOne.SDK.APIClient;

namespace VersionOne.VisualStudio.DataLayer {
    public class ValueId {
        internal readonly Oid Oid;
        private readonly string name;

        public ValueId() : this(Oid.Null, string.Empty) {}

        internal ValueId(Oid oid, string name) {
            Oid = oid.Momentless;
            this.name = name;
        }

        public override string ToString() {
            return name;
        }

        public bool Equals(ValueId obj) {
            if (ReferenceEquals(null, obj)) {
                return false;
            }
            return ReferenceEquals(this, obj) || Equals(obj.Oid, Oid);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) {
                return false;
            }
            if (ReferenceEquals(this, obj)) {
                return true;
            }
            return obj.GetType() == typeof (ValueId) && Equals((ValueId) obj);
        }

        public override int GetHashCode() {
            return Oid.GetHashCode();
        }
    }
}