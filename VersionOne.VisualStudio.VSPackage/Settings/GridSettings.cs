namespace VersionOne.VisualStudio.VSPackage.Settings {
    public class GridSettings {
        private string attributeSelection;
        private string storyAttributeSelection;
        private ColumnSetting[] columns;

        public string StoryAttributeSelection {
            get { return storyAttributeSelection; }
            set { storyAttributeSelection = value; }
        }

        public string AttributeSelection {
            get { return attributeSelection; }
            set { attributeSelection = value; }
        }

        public ColumnSetting[] Columns {
            get { return columns; }
            set { columns = value; }
        }
    }
}
