namespace VersionOne.VisualStudio.DataLayer {
    internal class AttributeInfo {
        public readonly string Attr;
        public readonly string Prefix;
        public readonly bool IsList;

        public AttributeInfo(string attr, string prefix, bool isList) {
            Attr = attr;
            Prefix = prefix;
            IsList = isList;
        }

        public override string ToString() {
            return string.Format("{0}.{1} (List:{2})", Prefix, Attr, IsList);
        }
    }
}
