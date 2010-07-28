using System;
using System.Globalization;
using System.Windows.Forms;
using VersionOne.VisualStudio.DataLayer;
using VersionOne.VisualStudio.VSPackage.Events;

namespace VersionOne.VisualStudio.VSPackage.Forms {
    public partial class CloseWorkitemDialog : Form {
        private readonly Workitem workitem;
        private readonly IDataLayer dataLayer = ApiDataLayer.Instance;
        private readonly IEventDispatcher eventDispatcher = EventDispatcher.Instance;
        
        public CloseWorkitemDialog(Workitem workitem) {
            this.workitem = workitem;

            InitializeComponent();
            BindControls();

            btnOK.Click +=  btnOK_Click;
        }

        private void BindControls() {
            try {
                object toDo = workitem.GetProperty(Entity.ToDoProperty);
                if(toDo != null) {
                    txtToDo.Text = ((double)toDo).ToString("0.00", CultureInfo.CurrentCulture);
                }
                PropertyValues statuses = dataLayer.GetListPropertyValues(workitem.TypePrefix + Entity.StatusProperty);
                cboStatus.Items.AddRange(statuses.ToArray());
                cboStatus.SelectedItem = workitem.GetProperty(Entity.StatusProperty);
                Text = "Close " + workitem.TypePrefix;
            } catch (DataLayerException ex) {
                MessageBox.Show("Failed to close. \n" + ex.Message, "Server communication error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DialogResult = DialogResult.Cancel;
                Close();
            }
        }

        private void btnOK_Click(object sender, EventArgs e) {
            try {
                workitem.SetProperty(Entity.StatusProperty, cboStatus.SelectedItem);
                workitem.CommitChanges();
                workitem.Close();
                eventDispatcher.InvokeModelChanged(null, ModelChangedArgs.WorkitemChanged);
            } catch (ValidatorException ex) {
                MessageBox.Show("Workitem cannot be closed because some required fields are empty:" + ex.Message, "Required fields validation", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DialogResult = DialogResult.Cancel;
            } catch (DataLayerException ex) {
                MessageBox.Show("Failed to close. \n" + ex.Message, "Server communication error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DialogResult = DialogResult.Cancel;
            }
        }
    }
}