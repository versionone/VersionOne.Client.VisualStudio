using System.Windows.Forms;
using Aga.Controls.Tree.NodeControls;

namespace VersionOne.VisualStudio.VSPackage.TreeViewEditors {
    public class CustomNodeTextBox : NodeTextBox {
        private bool isColumnReadOnly;
        private bool isPropertyReadOnly;
        private ContextMenu contextMenu;
        
        public TextBox EditorTextBox {
            get {
                return Parent.CurrentEditor as TextBox;
            }
        }

        public ContextMenu EditorContextMenu {
            get { return Parent.CurrentEditor != null ? Parent.CurrentEditor.ContextMenu : null; }
            set { contextMenu = value; }
        }

        public bool IsReadOnly {
            get { return IsColumnReadOnly || IsPropertyReadOnly; }
        }

        public bool IsColumnReadOnly {
            get { return isColumnReadOnly; }
            set {
                isColumnReadOnly = value;
                UpdateEditor();
            }
        }

        public bool IsPropertyReadOnly {
            get { return isPropertyReadOnly; }
            set {
                isPropertyReadOnly = value;
                UpdateEditor();
            }
        }

        private void UpdateEditor() {
            //IsPropertyReadOnly = IsReadOnly;
            //if (EditorTextBox != null && !EditorTextBox.IsDisposed) {
            //    EditorTextBox.ReadOnly = IsReadOnly;
            //}
        }

        protected override TextBox CreateTextBox() {
            TextBox textBox = new TextBox();
            textBox.ReadOnly = IsReadOnly;
            if(contextMenu != null) {
                textBox.ContextMenu = contextMenu;
            }
            return textBox;
        }
    }
}