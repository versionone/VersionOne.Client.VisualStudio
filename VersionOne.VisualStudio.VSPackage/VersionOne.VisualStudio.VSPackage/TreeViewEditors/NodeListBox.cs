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
    public class NodeListBox : BaseTextControl {
        #region Properties
        
        private const int ListBoxItemsNumber = 5;

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

        private Control parentTree;
        public Control ParentTree {
            set { parentTree = value; }
        }

        #endregion

        public NodeListBox() {
            dropDownItems = new List<ValueId>();
        }

        protected override Size CalculateEditorSize(EditorContext context) {
            if (Parent.UseColumns) {
                return context.Bounds.Size;
            }
            return new Size(EditorWidth, context.Bounds.Height);
        }

        protected override Control CreateEditor(TreeNodeAdv node) {
            ListBox listBox = new ListBox();
            listBox.SelectionMode = SelectionMode.MultiExtended;

            if (DropDownItems != null) {
                listBox.Items.AddRange(DropDownItems.ToArray());
            }

            object value = GetValue(node);
            PropertyValues propertyValues = value as PropertyValues;

            if (propertyValues != null) {
                foreach (ValueId item in propertyValues) {
                    listBox.SelectedItems.Add(item);
                }
            }

            listBox.LostFocus += EditorDropDownClosed;
            SetEditControlProperties(listBox, node);
            listBox.IntegralHeight = false;
            listBox.ScrollAlwaysVisible = true;
            return listBox;
        }

        private void EditorDropDownClosed(object sender, EventArgs e) {
            EndEdit(true);
        }

        public override void UpdateEditor(Control control) {
            ListBox editor = control as ListBox;
            EnsureControlVisibility(editor);
            editor.Refresh();
        }

        private void EnsureControlVisibility(ListBox editor) {
            int preferredHeight = editor.ItemHeight * Math.Min(editor.Items.Count, ListBoxItemsNumber);
            editor.Height = Math.Min(preferredHeight, parentTree.Height);
            if(parentTree.Height - editor.Bounds.Top < editor.Height) {
                editor.Bounds = new Rectangle(editor.Bounds.Left, parentTree.Height - editor.Height, editor.Bounds.Width, editor.Bounds.Height);
            }
            editor.BringToFront();
        }

        protected override void DoApplyChanges(TreeNodeAdv node, Control editor) {
            ListBox lstEditor = (ListBox) editor; 
            PropertyValues values = new PropertyValues(lstEditor.SelectedItems);
            SetValue(node, values);
        }

        public override void MouseUp(TreeNodeAdvMouseEventArgs args) {
            if (args.Node != null && args.Node.IsSelected) {
                base.MouseUp(args);
            }
        }
    }
}