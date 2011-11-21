using System.Windows.Forms;

namespace VersionOne.VisualStudio.VSPackage.Forms {
    public partial class ValidationDialog : Form {
        public ValidationDialog(string message) {
            InitializeComponent();
            txtErrors.Text = message;
        }
    }
}