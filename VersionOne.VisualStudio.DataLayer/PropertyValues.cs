using System.Collections;
using System.Collections.Generic;
using System.Text;

using VersionOne.SDK.APIClient;

namespace VersionOne.VisualStudio.DataLayer {
    public class PropertyValues : IEnumerable<ValueId> {
        private readonly Dictionary<Oid, ValueId> dictionary = new Dictionary<Oid, ValueId>();

        public PropertyValues(IEnumerable valueIds) {
            foreach (ValueId id in valueIds) {
                Add(id);
            }
        }

        public PropertyValues() {}

        public IEnumerator<ValueId> GetEnumerator() {
            return dictionary.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public override string ToString() {
            StringBuilder dataBuilder = new StringBuilder();
            bool isFirst = true;
            foreach (ValueId value in this) {
                if (!isFirst) {
                    dataBuilder.Append(", ");
                } else {
                    isFirst = false;
                }
                dataBuilder.Append(value);
            }

            return dataBuilder.ToString();
        }

        internal ValueId Find(Oid oid) {
            return dictionary[oid.Momentless];
        }

        public int Count {
            get { return dictionary.Count; }
        }

        internal bool ContainsOid(Oid value) {
            return dictionary.ContainsKey(value.Momentless);
        }

        public bool Contains(ValueId valueId) {
            return dictionary.ContainsValue(valueId);
        }
 
        public ValueId[] ToArray() {
            ValueId[] values = new ValueId[Count];
            dictionary.Values.CopyTo(values, 0);
            return values;
        }

        internal void Add(ValueId value) {
            dictionary.Add(value.Oid, value);
        }

        internal PropertyValues Subset(IEnumerable oids) {
            PropertyValues result = new PropertyValues();
            foreach (Oid oid in oids) {
                result.Add(Find(oid));
            }

            return result;
        }
    }
}