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
            return Parent.UseColumns ? context.Bounds.Size : new Size(EditorWidth, context.Bounds.Height);
        }

        protected override Control CreateEditor(TreeNodeAdv node) {
            var comboBox = new ComboBox();

            if (DropDownItems != null) {
                comboBox.Items.AddRange(DropDownItems.ToArray());
            }

            var value = GetValue(node);
            var property = (ValueId) value;

            var index = 0;
            // TODO
            
            if(property != null) {
                foreach (var item in comboBox.Items) {
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

        protected override void DisposeEditor(Control editor)
        {
            // TODO: What needs disposing?
            // throw new NotImplementedException();
        }

        void EditorDropDownClosed(object sender, EventArgs e) {
            EndEdit(true);
        }

        public override void UpdateEditor(Control control) {
            (control as ComboBox).DroppedDown = true;
        }

        protected override void DoApplyChanges(TreeNodeAdv node, Control editor) {
            var cboEditor = (ComboBox) editor;
            var item = (ValueId) cboEditor.SelectedItem;
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