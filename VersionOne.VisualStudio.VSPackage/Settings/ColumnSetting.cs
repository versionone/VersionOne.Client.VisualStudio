namespace VersionOne.VisualStudio.VSPackage.Settings {
    public class ColumnSetting {
        private string name;
        private string type;
        private string attribute;
        private string category;
        private bool readOnly;
        private bool effortTracking;
        private int width = 100;

        public string Attribute {
            get { return attribute; }
            set { attribute = value; }
        }

        public string Type {
            get { return type; }
            set { type = value; }
        }

        public string Name {
            get { return name; }
            set { name = value; }
        }

        public bool ReadOnly {
            get { return readOnly; }
            set { readOnly = value; }
        }

        public string Category {
            get { return category; }
            set { category = value; }
        }

        public bool EffortTracking {
            get { return effortTracking; }
            set { effortTracking = value; }
        }

        public int Width {
            get { return width; }
            set { width = value; }
        }
    }
}