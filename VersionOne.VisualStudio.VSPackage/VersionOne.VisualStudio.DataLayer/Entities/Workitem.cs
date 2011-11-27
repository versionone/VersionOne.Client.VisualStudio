using System;
using System.Collections.Generic;
using System.Linq;
using VersionOne.SDK.APIClient;

namespace VersionOne.VisualStudio.DataLayer.Entities {
    public class Workitem : Entity {
        private static readonly string[] SupportedTypes = {StoryType, DefectType, TaskType, TestType};

        public Workitem Parent { get; private set; }

        public override string TypePrefix {
            get { return Asset.AssetType.Token; }
        }

        /// <summary>
        /// List of child Workitems.
        /// </summary>
        public readonly List<Workitem> Children = new List<Workitem>();

        public bool HasChanges {
            get { return Asset.HasChanged || EntityContainer.GetEffort(this) != 0; }
        }

        public virtual bool IsPrimary {
            get {
                return TypePrefix == StoryType || TypePrefix == DefectType;
            }
        }

        public virtual bool IsVirtual {
            get { return false; }
        }

        internal Workitem(Asset asset, Workitem parent, IEntityContainer entityContainer) : base(asset, entityContainer) {
            Parent = parent;

            // the following check is for unit tests
            if(asset == null || asset.Children == null) {
                return;
            }

            if(!SupportedTypes.Contains(asset.AssetType.Token)) {
                throw new ArgumentException(string.Format("Illegal asset type, '{0}' is not supported.", asset.AssetType.Token));
            }

            foreach (var childAsset in asset.Children.Where(childAsset => DataLayer.ShowAllTasks || DataLayer.AssetPassesShowMyTasksFilter(childAsset))) {
                Children.Add(WorkitemFactory.CreateWorkitem(childAsset, this, entityContainer));
                Children.Sort(new WorkitemComparer(TestType, TaskType));
            }

            Children.TrimExcess();
        }

        public override bool IsPropertyReadOnly(string propertyName) {
            var fullName = TypePrefix + '.' + propertyName;
            
            try {
                return DataLayer.IsEffortTrackingRelated(propertyName) && AreEffortTrackingPropertiesReadOnly();
            } catch (Exception ex) {
                Logger.Warn("Cannot get property: " + fullName, ex);
                return true;
            }
        }

        private bool AreEffortTrackingPropertiesReadOnly() {
            var storyLevel = DataLayer.StoryTrackingLevel;
            var defectLevel = DataLayer.DefectTrackingLevel;

            switch (TypePrefix) {
                case StoryType:
                    return storyLevel != EffortTrackingLevel.PrimaryWorkitem && storyLevel != EffortTrackingLevel.Both;
                case DefectType:
                    return defectLevel != EffortTrackingLevel.PrimaryWorkitem && defectLevel != EffortTrackingLevel.Both;
                case TaskType:
                case TestType:
                    EffortTrackingLevel parentLevel;

                    switch(Parent.TypePrefix) {
                        case StoryType:
                            parentLevel = storyLevel;
                            break;
                        case DefectType:
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
                EntityContainer.Commit(this);
                EntityContainer.Refresh(this);
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
                EntityContainer.Cleanup(this);
            } catch (APIException ex) {
               Logger.Error("Failed to QuickClose.", ex);
            }
        }

        public virtual bool CanSignup {
            get {
                try {
                    return (bool) GetProperty("CheckQuickSignup");
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
                EntityContainer.Refresh(this);
            } catch (APIException ex) {
               Logger.Error("Failed to QuickSignup.", ex);
            }
        }

        /// <summary>
        /// Performs Inactivate operation.
        /// </summary>
        public virtual void Close() {
            DataLayer.ExecuteOperation(Asset, Asset.AssetType.GetOperation("Inactivate"));
            EntityContainer.Cleanup(this);
        }

        public virtual void RevertChanges() {
            EntityContainer.Revert(this);
        }
    }
}