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
        private readonly IVersionOneConnector connector;
        private IV1Configuration configuration;

        public EffortTrackingLevel DefectTrackingLevel { get; private set; }
        public EffortTrackingLevel StoryTrackingLevel { get; private set; }
        public bool TrackEffort { get; private set; }

        public EffortTracking(IVersionOneConnector connector) {
            this.connector = connector;
            this.configuration = connector.V1Configuration;
        }

        public void Init() {
            TrackEffort = configuration.EffortTracking;
            StoryTrackingLevel = TranslateEffortTrackingLevel(configuration.StoryTrackingLevel);
            DefectTrackingLevel = TranslateEffortTrackingLevel(configuration.DefectTrackingLevel);
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
            configuration = connector.LoadV1Configuration();
            Init();
        }

        public bool AreEffortTrackingPropertiesReadOnly(Workitem workitem) {
            switch (workitem.TypePrefix) {
                case Entity.StoryType:
                    return StoryTrackingLevel != EffortTrackingLevel.PrimaryWorkitem && StoryTrackingLevel != EffortTrackingLevel.Both;
                case Entity.DefectType:
                    return DefectTrackingLevel != EffortTrackingLevel.PrimaryWorkitem && DefectTrackingLevel != EffortTrackingLevel.Both;
                case Entity.TaskType:
                case Entity.TestType:
                    var parentLevel = GetParentLevel(workitem);
                    return parentLevel != EffortTrackingLevel.SecondaryWorkitem && parentLevel != EffortTrackingLevel.Both;
                default:
                    throw new NotSupportedException("Unexpected asset type.");
            }
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