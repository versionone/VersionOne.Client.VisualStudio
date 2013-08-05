using System;

namespace VersionOne.VisualStudio.VSPackage.Settings {
    public class AssetDetailSettings {
        private ColumnSetting[] taskColumns;
        private ColumnSetting[] storyColumns;
        private ColumnSetting[] testColumns;
        private ColumnSetting[] defectColumns;

        public ColumnSetting[] TaskColumns {
            get { return taskColumns; }
            set { taskColumns = value; }
        }

        public ColumnSetting[] StoryColumns {
            get { return storyColumns; }
            set { storyColumns = value; }
        }

        public ColumnSetting[] TestColumns {
            get { return testColumns; }
            set { testColumns = value; }
        }

        public ColumnSetting[] DefectColumns {
            get { return defectColumns; }
            set { defectColumns = value; }
        }

        public ColumnSetting[] GetColumns(string type) {
            switch (type) {
                case "Story":
                    return storyColumns;
                case "Task":
                    return taskColumns;
                case "Defect":
                    return defectColumns;
                case "Test":
                    return testColumns;
                default:
                    throw new ArgumentException("Unknown type: " + type);
            }
        }
    }

}
