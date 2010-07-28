using System.Windows.Forms;
using VersionOne.VisualStudio.DataLayer;
using VersionOne.VisualStudio.VSPackage.Descriptors;

namespace VersionOne.VisualStudio.VSPackage.PropertyEditors {
    public class MultiValueEditor : ListPropertyEditor {
        protected override void ConfigureListBox() {
            listBox.SelectionMode = SelectionMode.MultiExtended;
        }

        protected override void SetSelection(WorkitemPropertyDescriptor descriptor, object propertyValues) {
            string propertyName = descriptor.Workitem.TypePrefix + descriptor.Attribute;
            foreach (ValueId item in dataLayer.GetListPropertyValues(propertyName)) {
                listBox.Items.Add(item);
            }
            foreach (ValueId valueId in (PropertyValues)propertyValues) {
                listBox.SelectedItems.Add(valueId);
            }
        }

        protected override object GetSelection() {
            return new PropertyValues(listBox.SelectedItems);
        }
    }
}
