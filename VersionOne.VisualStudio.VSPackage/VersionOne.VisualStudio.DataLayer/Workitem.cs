using System;
using System.Collections.Generic;

using VersionOne.SDK.APIClient;

namespace VersionOne.VisualStudio.DataLayer {
    public class Workitem : Entity {
        private Workitem parent;

        public Workitem Parent {
            get { return parent; }
            protected internal set { parent = value; }
        }

        // TODO move exception off the property
        public override string TypePrefix {
            get {
                if (Asset.AssetType.Token == dataLayer.TaskType.Token) {
                    return TaskPrefix;
                } else if (Asset.AssetType.Token == dataLayer.StoryType.Token) {
                    return StoryPrefix;
                } else if (Asset.AssetType.Token == dataLayer.TestType.Token) {
                    return TestPrefix;
                } else if (Asset.AssetType.Token == dataLayer.DefectType.Token) {
                    return DefectPrefix;
                } else {
                    throw new ArgumentException("Illegal asset.");
                }
            }
        }

        /// <summary>
        /// List of child Workitems.
        /// </summary>
        public readonly List<Workitem> Children = new List<Workitem>();

        public bool HasChanges {
            get { return Asset.HasChanged || dataLayer.GetEffort(Asset) != 0; }
        }

        public virtual bool IsPrimary {
            get {
                return TypePrefix == StoryPrefix || TypePrefix == DefectPrefix;
            }
        }

        public virtual bool IsVirtual {
            get { return false; }
        }

        internal Workitem(Asset asset, Workitem parent) : this(asset) {
            Parent = parent;
        }

        private Workitem(Asset asset) : base(asset) {
            // the following check is for unit tests
            if(asset == null || asset.Children == null) {
                return;
            }
            foreach (Asset childAsset in asset.Children) {
                if (dataLayer.ShowAllTasks || dataLayer.AssetPassesShowMyTasksFilter(childAsset)) {
                    Children.Add(WorkitemFactory.Instance.CreateWorkitem(childAsset, this));
                }
            }
            Children.TrimExcess();
        }

        public override bool IsPropertyReadOnly(string propertyName) {
            string fullName = TypePrefix + '.' + propertyName;
            try {
                if (dataLayer.IsEffortTrackingRelated(propertyName)) {
                    return AreEffortTrackingPropertiesReadOnly();
                }

                return false;
            } catch (Exception ex) {
                ApiDataLayer.Warning("Cannot get property: " + fullName, ex);
                return true;
            }
        }

        private bool AreEffortTrackingPropertiesReadOnly() {
            EffortTrackingLevel storyLevel = dataLayer.StoryTrackingLevel;
            EffortTrackingLevel defectLevel = dataLayer.DefectTrackingLevel;

            switch (TypePrefix) {
                case StoryPrefix:
                    return storyLevel != EffortTrackingLevel.PrimaryWorkitem && storyLevel != EffortTrackingLevel.Both;
                case DefectPrefix:
                    return defectLevel != EffortTrackingLevel.PrimaryWorkitem && defectLevel != EffortTrackingLevel.Both;
                case TaskPrefix:
                case TestPrefix:
                    EffortTrackingLevel parentLevel;
                    if (Parent.TypePrefix == StoryPrefix) {
                        parentLevel = storyLevel;
                    } else if (Parent.TypePrefix == DefectPrefix) {
                        parentLevel = defectLevel;
                    } else {
                        throw new InvalidOperationException("Unexpected parent asset type.");
                    }
                    return parentLevel != EffortTrackingLevel.SecondaryWorkitem && parentLevel != EffortTrackingLevel.Both;
                default:
                    throw new NotSupportedException("Unexpected asset type.");
            }
        }


        public bool PropertyChanged(string propertyName) {
            IAttributeDefinition attrDef = Asset.AssetType.GetAttributeDefinition(propertyName);
            return Asset.GetAttribute(attrDef).HasChanged;
        }

        public virtual void CommitChanges() {
            try {
                dataLayer.CommitAsset(Asset);
            } catch (APIException ex) {
                throw ApiDataLayer.Warning("Failed to commit changes.", ex);
            }
        }

        public bool IsMine() {
            PropertyValues owners = (PropertyValues)GetProperty(OwnersProperty);
            return owners.ContainsOid(dataLayer.MemberOid);
        }

        public virtual bool CanQuickClose {
            get {
                try {
                    return (bool) GetProperty("CheckQuickClose");
                } catch (KeyNotFoundException ex) {
                    ApiDataLayer.Warning("QuickClose not supported.", ex);
                    return false;
                }
            }
        }

        /// <summary>
        /// Performs QuickClose operation.
        /// </summary>
        public virtual void QuickClose() {
            CommitChanges();
            try {
                dataLayer.ExecuteOperation(Asset, Asset.AssetType.GetOperation("QuickClose"));
                dataLayer.CleanupWorkitem(this);
            } catch (APIException ex) {
                throw ApiDataLayer.Warning("Failed to QuickClose.", ex);
            }
        }

        public virtual bool CanSignup {
            get {
                try {
                    return (bool)GetProperty("CheckQuickSignup");
                } catch (KeyNotFoundException ex) {
                    ApiDataLayer.Warning("QuickSignup not supported.", ex);
                    return false;
                }
            }
        }

        /// <summary>
        /// Performs QuickSignup operation.
        /// </summary>
        public virtual void Signup() {
            try {
                dataLayer.ExecuteOperation(Asset, Asset.AssetType.GetOperation("QuickSignup"));
                dataLayer.RefreshAsset(this);
            } catch (APIException ex) {
                throw ApiDataLayer.Warning("Failed to QuickSignup.", ex);
            }
        }

        /// <summary>
        /// Performs Inactivate operation.
        /// </summary>
        public virtual void Close() {
            dataLayer.ExecuteOperation(Asset, Asset.AssetType.GetOperation("Inactivate"));
            dataLayer.CleanupWorkitem(this);
        }

        public virtual void RevertChanges() {
            dataLayer.RevertAsset(Asset);
        }
    }
}