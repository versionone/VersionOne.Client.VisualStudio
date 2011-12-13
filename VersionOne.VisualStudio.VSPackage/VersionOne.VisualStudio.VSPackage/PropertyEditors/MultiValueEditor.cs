using System;
using System.Windows.Forms;
using VersionOne.VisualStudio.DataLayer;
using VersionOne.VisualStudio.VSPackage.Descriptors;

namespace VersionOne.VisualStudio.VSPackage.PropertyEditors {
    public class MultiValueEditor : ListPropertyEditor {
        protected override void ConfigureListBox() {
            ListBox.SelectionMode = SelectionMode.MultiExtended;
            ListBox.Click += ListBoxClick;
        }

        protected override void SetSelection(WorkitemPropertyDescriptor descriptor, object propertyValues) {
            var propertyName = descriptor.Workitem.TypePrefix + descriptor.Attribute;

            foreach (var item in DataLayer.GetListPropertyValues(propertyName)) {
                ListBox.Items.Add(item);
            }

            foreach (var valueId in (PropertyValues) propertyValues) {
                ListBox.SelectedItems.Add(valueId);
            }
        }

        protected override object GetSelection() {
            return new PropertyValues(ListBox.SelectedItems);
        }

        private void ListBoxClick(object sender, EventArgs e) {
            if (Control.ModifierKeys != Keys.Control) {
                CloseDropDown();
            }
        }
    }
}