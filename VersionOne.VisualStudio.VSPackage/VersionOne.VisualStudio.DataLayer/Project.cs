using System.Collections.Generic;
using VersionOne.SDK.APIClient;

namespace VersionOne.VisualStudio.DataLayer {
    public class Project : Entity {
        protected internal Project Parent;

        public override string TypePrefix {
            get { return ProjectPrefix; }
        }

        public readonly List<Project> Children = new List<Project>();

        internal Project(Asset asset, Project parent) : this(asset) {
            Parent = parent;
        }

        private Project(Asset asset) : base(asset) {
            // the following check is for unit tests
            if(asset == null || asset.Children == null) {
                return;
            }

            foreach (Asset childAsset in asset.Children) {
                Children.Add(WorkitemFactory.Instance.CreateProject(childAsset, this));
            }
            Children.TrimExcess();
        }

        public override bool IsPropertyReadOnly(string propertyName) {
            return true;
        }
    }
}
