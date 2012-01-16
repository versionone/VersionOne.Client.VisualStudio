using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using VersionOne.VisualStudio.DataLayer;
using VersionOne.VisualStudio.VSPackage.Descriptors;

namespace VersionOne.VisualStudio.VSPackage.PropertyEditors {
    public class ListPropertyEditor : UITypeEditor {
        protected readonly PropertyGridListBox ListBox = new PropertyGridListBox();
        protected readonly IDataLayer DataLayer = ServiceLocator.Instance.Get<IDataLayer>();
        private IWindowsFormsEditorService service;

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
            return UITypeEditorEditStyle.DropDown;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
            service = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
            
            if (service != null) {
                ConfigureListBox();
                SetSelection((WorkitemPropertyDescriptor)context.PropertyDescriptor, value);
                service.DropDownControl(ListBox);
                return GetSelection();
            }

            return value;
        }

        protected virtual void ConfigureListBox() {
            ListBox.SelectionMode = SelectionMode.One;
            ListBox.MouseUp += ListBoxMouseUp;
        }

        /// <summary>
        /// Sets specified value to the control.
        /// </summary>
        /// <param name="descriptor">Descriptor of Workitem.</param>
        /// <param name="valueId">Value of the Workitem property.</param>
        protected virtual void SetSelection(WorkitemPropertyDescriptor descriptor, object valueId) {
            var dataSource = DataLayer.GetListPropertyValues(descriptor.Workitem.TypePrefix + descriptor.Attribute);

            foreach (var item in dataSource) {
                ListBox.Items.Add(item);
            }
            
            ListBox.SelectedItem = valueId;
        }

        /// <summary>
        /// Gets the value from control.
        /// </summary>
        /// <returns>Value to be set to Workitem property.</returns>
        protected virtual object GetSelection() {
            return ListBox.SelectedItem;
        }

        private void ListBoxMouseUp(object sender, MouseEventArgs e) {
            var selectedIndex = ListBox.IndexFromPoint(e.X, e.Y);

            if (selectedIndex != -1) {
                CloseDropDown();
            }
        }

        protected void CloseDropDown() {
            if (service != null) {
                service.CloseDropDown();
            }
        }
    }
}