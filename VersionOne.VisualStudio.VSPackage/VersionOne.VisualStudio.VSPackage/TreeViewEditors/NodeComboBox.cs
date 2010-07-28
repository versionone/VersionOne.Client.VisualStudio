using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing.Design;
using Aga.Controls;
using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;
using VersionOne.VisualStudio.DataLayer;

namespace VersionOne.VisualStudio.VSPackage.TreeViewEditors {
    public class NodeComboBox : BaseTextControl {
        #region Properties

        private int editorWidth = 100;
        [DefaultValue(100)]
        public int EditorWidth {
            get { return editorWidth; }
            set { editorWidth = value; }
        }

        private List<ValueId> dropDownItems;
        [Editor(typeof(StringCollectionEditor), typeof(UITypeEditor)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public List<ValueId> DropDownItems {
            get { return dropDownItems; }
            set { dropDownItems = value; }
        }

        #endregion

        public NodeComboBox() {
            dropDownItems = new List<ValueId>();
        }

        protected override Size CalculateEditorSize(EditorContext context) {
            if (Parent.UseColumns) {
                return context.Bounds.Size;
            }
            return new Size(EditorWidth, context.Bounds.Height);
        }

        protected override Control CreateEditor(TreeNodeAdv node) {
            ComboBox comboBox = new ComboBox();

            if (DropDownItems != null) {
                comboBox.Items.AddRange(DropDownItems.ToArray());
            }

            object value = GetValue(node);
            ValueId property = (ValueId)value;

            int index = 0;
            // TODO
            if(property != null) {
                foreach (object item in comboBox.Items) {
                    if(item.Equals(value)) {
                        comboBox.SelectedIndex = index;
                        break;
                    }
                    index++;
                }
            }
            
            comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox.DropDownClosed += EditorDropDownClosed;
            SetEditControlProperties(comboBox, node);
            return comboBox;
        }

        void EditorDropDownClosed(object sender, EventArgs e) {
            EndEdit(true);
        }

        public override void UpdateEditor(Control control) {
            (control as ComboBox).DroppedDown = true;
        }

        protected override void DoApplyChanges(TreeNodeAdv node, Control editor) {
            ComboBox cboEditor = (ComboBox) editor;
            ValueId item = (ValueId) cboEditor.SelectedItem;
            SetValue(node, item);
        }

        public override void MouseUp(TreeNodeAdvMouseEventArgs args) {
            //Workaround of specific ComboBox control behaviour
            if (args.Node != null && args.Node.IsSelected) {
                base.MouseUp(args);
            }
        }
    }
}