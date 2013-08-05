using System.Linq;
using System.Collections.Generic;
using VersionOne.SDK.APIClient;

namespace VersionOne.VisualStudio.DataLayer.Entities {
    public class Project : Entity {
        private readonly Project parent;

        public override string TypePrefix {
            get { return ProjectType; }
        }

        public readonly List<Project> Children = new List<Project>();

        internal Project(Asset asset, Project parent) : this(asset) {
            this.parent = parent;
        }

        private Project(Asset asset) : base(asset, null) {
            // the following check is for unit tests
            if(asset == null || asset.Children == null) {
                return;
            }

            Children.AddRange(asset.Children.Select(item => WorkitemFactory.CreateProject(item, this)));
        }

        public override bool IsPropertyReadOnly(string propertyName) {
            return true;
        }
    }
}