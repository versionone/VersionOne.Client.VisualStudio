using System.Windows.Forms;

namespace VersionOne.VisualStudio.VSPackage.PropertyEditors {
    public class PropertyGridListBox : ListBox {
        int originalSelectedIndex = -1;

        public override int SelectedIndex {
            get {
                return base.SelectedIndex;
            }
            set {
                if (originalSelectedIndex == -1) {
                    originalSelectedIndex = value;
                }

                base.SelectedIndex = value;
            }
        }

        protected override bool ProcessDialogKey(Keys keyData) {
            if (keyData == Keys.Escape) {
                SelectedIndex = originalSelectedIndex;
            }

            return base.ProcessDialogKey(keyData);
        }
    }
}