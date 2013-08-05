using System;
using System.Globalization;
using System.Windows.Forms;
using VersionOne.VisualStudio.DataLayer;
using VersionOne.VisualStudio.DataLayer.Entities;

namespace VersionOne.VisualStudio.VSPackage.Forms {
    public partial class CloseWorkitemDialog : Form {
        private readonly Workitem workitem;
        private readonly IDataLayer dataLayer = ServiceLocator.Instance.Get<IDataLayer>();
        
        public CloseWorkitemDialog(Workitem workitem) {
            this.workitem = workitem;

            InitializeComponent();
            BindControls();

            btnOK.Click +=  btnOK_Click;
        }

        private void BindControls() {
            try {
                var toDo = workitem.GetProperty(Entity.ToDoProperty);
                
                if(toDo != null) {
                    txtToDo.Text = ((double) toDo).ToString("0.00", CultureInfo.CurrentCulture);
                }
                
                var statuses = dataLayer.GetListPropertyValues(workitem.TypePrefix + Entity.StatusProperty);
                cboStatus.Items.AddRange(statuses.ToArray());
                cboStatus.SelectedItem = workitem.GetProperty(Entity.StatusProperty);
                Text = "Close " + workitem.TypePrefix;
            } catch(DataLayerException ex) {
                MessageBox.Show("Failed to close. \n" + ex.Message, "Server communication error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DialogResult = DialogResult.Cancel;
                Close();
            }
        }

        private void btnOK_Click(object sender, EventArgs e) {
            workitem.SetProperty(Entity.StatusProperty, cboStatus.SelectedItem);
        }
    }
}