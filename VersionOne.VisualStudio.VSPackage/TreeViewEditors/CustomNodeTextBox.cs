using System.Windows.Forms;
using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;

namespace VersionOne.VisualStudio.VSPackage.TreeViewEditors {
    public class CustomNodeTextBox : NodeTextBox {
        private bool isColumnReadOnly;
        private bool isPropertyReadOnly;
        private ContextMenu contextMenu;
        private TextBox textBox;

        public event PreviewKeyDownEventHandler KeyTextBoxDown;

        public TextBox EditorTextBox {
            get {
                return this.textBox;
            }
        }

        public ContextMenu EditorContextMenu {
            get {
                return contextMenu; 
            }
            set { this.contextMenu = value; }
        }

        public bool IsPropertyReadOnly {
            get { return isPropertyReadOnly; }
            set {
                isPropertyReadOnly = value;
                UpdateEditor();
            }
        }

        public bool IsReadOnly
        {
            get { return IsColumnReadOnly || IsPropertyReadOnly; }
        }

        public bool IsColumnReadOnly
        {
            get { return isColumnReadOnly; }
            set
            {
                isColumnReadOnly = value;
                UpdateEditor();
            }
        }

        private void UpdateEditor() {
            if (EditorTextBox != null && !EditorTextBox.IsDisposed) {
                EditorTextBox.ReadOnly = IsReadOnly;
            }
        }
        protected override TextBox CreateTextBox() {
            this.textBox = new TextBox();
            textBox.ReadOnly = this.IsReadOnly;
            if(contextMenu != null) {
                textBox.ContextMenu = contextMenu;
            }
            if (textBox.ReadOnly)
            {
                textBox.BackColor = System.Drawing.Color.DarkGray;
            }
            this.textBox.PreviewKeyDown += textBox_PreviewKeyDown;
            return textBox;
        }

        void textBox_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.Shift && e.KeyCode == Keys.Tab)
                KeyTextBoxDown(sender, e);
            else
                if (e.KeyCode == Keys.Tab)
                    KeyTextBoxDown(sender, e);
        }               
    }
}