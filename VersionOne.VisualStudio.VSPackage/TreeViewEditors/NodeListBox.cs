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
using System.Linq;

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

        [Editor(typeof(StringCollectionEditor), typeof(UITypeEditor)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public List<ValueId> DropDownItems { get; set; }

        private Control parentTree;
        public Control ParentTree {
            set { parentTree = value; }
        }

        #endregion

        public NodeListBox() {
            DropDownItems = new List<ValueId>();
        }

        protected override Size CalculateEditorSize(EditorContext context) {
            return Parent.UseColumns ? context.Bounds.Size : new Size(EditorWidth, context.Bounds.Height);
        }

        protected override Control CreateEditor(TreeNodeAdv node) {
            var listBox = new ListBox {SelectionMode = SelectionMode.MultiExtended};

            if (DropDownItems != null) {
                listBox.Items.AddRange(DropDownItems.Where(i => i.Inactive == false).ToArray());
            }

            var value = GetValue(node);
            var propertyValues = value as PropertyValues;

            SetSelectionItems(listBox, propertyValues);

            listBox.Click += ListBoxClick;

            SetEditControlProperties(listBox, node);
            listBox.IntegralHeight = false;
            listBox.ScrollAlwaysVisible = true;            
            return listBox;
        }

        protected override void DisposeEditor(Control editor)
        {
            // TODO: What needs disposing?
            // throw new NotImplementedException();
        }

        private static void SetSelectionItems(ListBox listBox, PropertyValues propertyValues)
        {
            if (propertyValues == null)
            {
                return;
            }
            foreach (var item in propertyValues)
            {
                if (!listBox.SelectedItems.Contains(item) && item.Inactive)
                {
                    listBox.Items.Add(item);
                }

                listBox.SelectedItems.Add(item);
            }
        }

        public override void UpdateEditor(Control control) {
            var editor = control as ListBox;
            EnsureControlVisibility(editor);
            editor.Refresh();
        }

        private void EnsureControlVisibility(ListBox editor) {
            var preferredHeight = editor.ItemHeight * Math.Min(editor.Items.Count, ListBoxItemsNumber);
            editor.Height = Math.Min(preferredHeight, parentTree.Height);

            if(parentTree.Height - editor.Bounds.Top < editor.Height) {
                editor.Bounds = new Rectangle(editor.Bounds.Left, parentTree.Height - editor.Height, editor.Bounds.Width, editor.Bounds.Height);
            }

            editor.BringToFront();
        }

        protected override void DoApplyChanges(TreeNodeAdv node, Control editor) {
            var lstEditor = (ListBox) editor; 
            var values = new PropertyValues(lstEditor.SelectedItems);
            SetValue(node, values);
        }

        public override void MouseUp(TreeNodeAdvMouseEventArgs args) {
            if (args.Node != null && args.Node.IsSelected) {
                base.MouseUp(args);
            }
        }

        private void ListBoxClick(object sender, EventArgs e)
        {
            if (Control.ModifierKeys != Keys.Control)
            {
                EndEdit(true);
            }
        }
    }
}