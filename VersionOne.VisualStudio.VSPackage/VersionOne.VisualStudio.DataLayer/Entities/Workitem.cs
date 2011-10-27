using System;
using System.Collections.Generic;
using VersionOne.SDK.APIClient;

namespace VersionOne.VisualStudio.DataLayer.Entities {
    public class Workitem : Entity {
        public Workitem Parent { get; private set; }

        // TODO move exception off the property, convert to switch using const strings
        public override string TypePrefix {
            get {
                if (Asset.AssetType.Token == DataLayer.TaskType.Token) {
                    return TaskPrefix;
                } else if (Asset.AssetType.Token == DataLayer.StoryType.Token) {
                    return StoryPrefix;
                } else if (Asset.AssetType.Token == DataLayer.TestType.Token) {
                    return TestPrefix;
                } else if (Asset.AssetType.Token == DataLayer.DefectType.Token) {
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
            get { return Asset.HasChanged || DataLayer.GetEffort(Asset) != 0; }
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
                if (DataLayer.ShowAllTasks || DataLayer.AssetPassesShowMyTasksFilter(childAsset)) {
                    Children.Add(WorkitemFactory.CreateWorkitem(childAsset, this));
                    Children.Sort(new WorkitemComparer(DataLayer.TestType.Token, DataLayer.TaskType.Token));
                }
            }
            Children.TrimExcess();
        }

        public override bool IsPropertyReadOnly(string propertyName) {
            var fullName = TypePrefix + '.' + propertyName;
            
            try {
                return DataLayer.IsEffortTrackingRelated(propertyName) && AreEffortTrackingPropertiesReadOnly();
            } catch (Exception ex) {
                Logger.Warning("Cannot get property: " + fullName, ex);
                return true;
            }
        }

        private bool AreEffortTrackingPropertiesReadOnly() {
            var storyLevel = DataLayer.StoryTrackingLevel;
            var defectLevel = DataLayer.DefectTrackingLevel;

            switch (TypePrefix) {
                case StoryPrefix:
                    return storyLevel != EffortTrackingLevel.PrimaryWorkitem && storyLevel != EffortTrackingLevel.Both;
                case DefectPrefix:
                    return defectLevel != EffortTrackingLevel.PrimaryWorkitem && defectLevel != EffortTrackingLevel.Both;
                case TaskPrefix:
                case TestPrefix:
                    EffortTrackingLevel parentLevel;
                    
                switch(Parent.TypePrefix) {
                        case StoryPrefix:
                            parentLevel = storyLevel;
                            break;
                        case DefectPrefix:
                            parentLevel = defectLevel;
                            break;
                        default:
                            throw new InvalidOperationException("Unexpected parent asset type.");
                    }
                    return parentLevel != EffortTrackingLevel.SecondaryWorkitem && parentLevel != EffortTrackingLevel.Both;
                default:
                    throw new NotSupportedException("Unexpected asset type.");
            }
        }

        public virtual void CommitChanges() {
            try {
                DataLayer.CommitAsset(Asset);
            } catch (APIException ex) {
                Logger.Error("Failed to commit changes.", ex);
            }
        }

        public bool IsMine() {
            var owners = (PropertyValues)GetProperty(OwnersProperty);
            return owners.ContainsOid(DataLayer.MemberOid);
        }

        public virtual bool CanQuickClose {
            get {
                try {
                    return (bool) GetProperty("CheckQuickClose");
                } catch (KeyNotFoundException ex) {
                    Logger.Error("QuickClose not supported.", ex);
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
                DataLayer.ExecuteOperation(Asset, Asset.AssetType.GetOperation("QuickClose"));
                DataLayer.CleanupWorkitem(this);
            } catch (APIException ex) {
               Logger.Error("Failed to QuickClose.", ex);
            }
        }

        public virtual bool CanSignup {
            get {
                try {
                    return (bool)GetProperty("CheckQuickSignup");
                } catch (KeyNotFoundException ex) {
                    Logger.Error("QuickSignup not supported.", ex);
                    return false;
                }
            }
        }

        /// <summary>
        /// Performs QuickSignup operation.
        /// </summary>
        public virtual void Signup() {
            try {
                DataLayer.ExecuteOperation(Asset, Asset.AssetType.GetOperation("QuickSignup"));
                DataLayer.RefreshAsset(this);
            } catch (APIException ex) {
               Logger.Error("Failed to QuickSignup.", ex);
            }
        }

        /// <summary>
        /// Performs Inactivate operation.
        /// </summary>
        public virtual void Close() {
            DataLayer.ExecuteOperation(Asset, Asset.AssetType.GetOperation("Inactivate"));
            DataLayer.CleanupWorkitem(this);
        }

        public virtual void RevertChanges() {
            DataLayer.RevertAsset(Asset);
        }
    }
}