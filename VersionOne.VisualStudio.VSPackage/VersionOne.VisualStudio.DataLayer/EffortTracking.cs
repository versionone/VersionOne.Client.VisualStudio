using System;
using System.Collections.Generic;

using VersionOne.SDK.APIClient;

namespace VersionOne.VisualStudio.DataLayer {
    public class EffortTracking : IEffortTracking {
        private static readonly IList<string> effortTrackingAttributes = new List<string> {
            "DetailEstimate",
            "ToDo",
            "Done",
            "Effort",
            "Actuals",
        };
        private IV1Configuration v1Config;
        //TODO atm I can't find better way to recognize when we need reload items.
        public bool RequiredReload { get; private set; }

        public EffortTrackingLevel DefectTrackingLevel { get; private set; }
        public EffortTrackingLevel StoryTrackingLevel { get; private set; }
        public bool TrackEffort { get; private set; }

        public EffortTracking(IV1Configuration v1Config) {
            this.v1Config = v1Config;
        }

        public void Init() {
            TrackEffort = v1Config.EffortTracking;
            StoryTrackingLevel = TranslateEffortTrackingLevel(v1Config.StoryTrackingLevel);
            DefectTrackingLevel = TranslateEffortTrackingLevel(v1Config.DefectTrackingLevel);
            RequiredReload = false;
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
            return effortTrackingAttributes.Contains(attributeName);
        }

        public void Drop() {
            RequiredReload = true;
        }
    }
}