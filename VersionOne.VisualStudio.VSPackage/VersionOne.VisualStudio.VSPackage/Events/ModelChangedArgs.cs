using System;

namespace VersionOne.VisualStudio.VSPackage.Events {
    public class ModelChangedArgs : EventArgs {
        public static readonly ModelChangedArgs SettingsChanged = new ModelChangedArgs("SettingsChanged", 3); 
        public static readonly ModelChangedArgs ProjectChanged = new ModelChangedArgs("ProjectChanged", 2);
        public static readonly ModelChangedArgs WorkitemChanged = new ModelChangedArgs("WorkitemsChanged", 1); 

        private readonly string name;
        private readonly int scope;

        public int Scope {
            get { return scope; }
        }

        private ModelChangedArgs(string name, int scope) {
            this.name = name;
            this.scope = scope;
        }

        public override string ToString() {
            return name; 
        }
    }
}