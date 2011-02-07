namespace VersionOne.VisualStudio.DataLayer.Entities {
    /// <summary>
    /// This class is intended to be used in unit tests, allowing not to involve real ApiDataLayer and ApiClient.
    /// </summary>
    public class TestWorkitem : Workitem {
        private readonly string id;
        private bool isPrimary;

        public TestWorkitem(string id, bool isPrimary) : base(null, null) {
            this.id = id;
            this.isPrimary = isPrimary;
        }

        public override string Id {
            get { return id; }
        }

        public override bool IsPrimary {
            get { return isPrimary; }
        }

        public override string ToString() {
            return "Test workitem";
        }
    }
}