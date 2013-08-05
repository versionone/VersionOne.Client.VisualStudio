using System;
using System.Collections.Generic;

using VersionOne.SDK.APIClient;
using VersionOne.VisualStudio.DataLayer.Entities;

namespace VersionOne.VisualStudio.DataLayer {
    public class EffortTracking : IEffortTracking {
        private static readonly IList<string> EffortTrackingAttributes = new List<string> {
            "DetailEstimate",
            "ToDo",
            "Done",
            "Effort",
            "Actuals",
        };
        private readonly IVersionOneConnector v1Connector;
        private IV1Configuration v1Configuration;

        public EffortTrackingLevel DefectTrackingLevel { get; private set; }
        public EffortTrackingLevel StoryTrackingLevel { get; private set; }
        public bool TrackEffort { get; private set; }

        public EffortTracking(IVersionOneConnector connector) {
            v1Connector = connector;
            v1Configuration = connector.V1Configuration;
        }

        public void Init() {
            TrackEffort = v1Configuration.EffortTracking;
            StoryTrackingLevel = TranslateEffortTrackingLevel(v1Configuration.StoryTrackingLevel);
            DefectTrackingLevel = TranslateEffortTrackingLevel(v1Configuration.DefectTrackingLevel);
        }

        private static EffortTrackingLevel TranslateEffortTrackingLevel(TrackingLevel level) {
            switch (level) {
                case TrackingLevel.On:
                    return EffortTrackingLevel.PrimaryWorkitem;
                case TrackingLevel.Off:
                    return EffortTrackingLevel.SecondaryWorkitem;
                case TrackingLevel.Mix:
                    return EffortTrackingLevel.Both;
                default:
                    throw new NotSupportedException("Unknown tracking level");
            }
        }

        public static bool IsEffortTrackingRelated(string attributeName) {
            return EffortTrackingAttributes.Contains(attributeName);
        }

        public void Refresh() {
            v1Configuration = v1Connector.LoadV1Configuration();
            Init();
        }

        public bool AreEffortTrackingPropertiesReadOnly(Workitem workitem) {
            switch (workitem.TypePrefix) {
                case Entity.StoryType:
                    return AreEffortTrackingPropertiesForPrimaryWorkitemReadOnly(StoryTrackingLevel);
                case Entity.DefectType:
                    return AreEffortTrackingPropertiesForPrimaryWorkitemReadOnly(DefectTrackingLevel);
                case Entity.TaskType:
                case Entity.TestType:
                    var parentLevel = GetParentLevel(workitem);
                    return AreEffortTrackingPropertiesForSecondaryWorkitemReadOnly(parentLevel);
                default:
                    throw new NotSupportedException("Unexpected asset type.");
            }
        }

        private static bool AreEffortTrackingPropertiesForPrimaryWorkitemReadOnly(EffortTrackingLevel effortTrackingLevel) {
            return effortTrackingLevel != EffortTrackingLevel.PrimaryWorkitem && effortTrackingLevel != EffortTrackingLevel.Both;
        }

        private static bool AreEffortTrackingPropertiesForSecondaryWorkitemReadOnly(EffortTrackingLevel parentEffortTrackingLevel) {
            return parentEffortTrackingLevel != EffortTrackingLevel.SecondaryWorkitem && parentEffortTrackingLevel != EffortTrackingLevel.Both;
        }

        private EffortTrackingLevel GetParentLevel(Workitem workitem) {
            switch (workitem.Parent.TypePrefix) {
                case Entity.StoryType:
                    return StoryTrackingLevel;
                case Entity.DefectType:
                    return DefectTrackingLevel;
                default:
                    throw new InvalidOperationException("Unexpected parent asset type.");
            }
        }
    }
}