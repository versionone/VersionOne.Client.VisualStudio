using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell.Interop;
using VersionOne.VisualStudio.DataLayer;
using VersionOne.VisualStudio.VSPackage.Descriptors;

using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
using Microsoft.VisualStudio.Shell;

namespace VersionOne.VisualStudio.VSPackage.Controls {
    public partial class V1UserControl : UserControl {
        protected IOleServiceProvider vsServiceProvider;
        protected ITrackSelection trackSel;
        protected readonly IDataLayer dataLayer;
        protected readonly IParentWindow ParentWindow;

        private IContainer components;
        private readonly ErrorMessageControl errorMessage;
        private IVsWindowFrame propertiesFrame;

        [Obsolete ("Only for designer.")]
        public V1UserControl() {
            InitializeComponent();
            errorMessage = new ErrorMessageControl();
            dataLayer = ApiDataLayer.Instance;
        }

        public V1UserControl(IParentWindow parent) {
            ParentWindow = parent;
            InitializeComponent();
            errorMessage = new ErrorMessageControl();
            dataLayer = ApiDataLayer.Instance;
        }

        public bool CheckSettingsAreValid() {
            if (Controls.Contains(errorMessage)) {
                Controls.Remove(errorMessage);
            }

            if (!dataLayer.IsConnected) {
                DisplayErrors();
                return false;
            }

            foreach (Control control in Controls) {
                control.Show();
            }

            return true;
        }

        protected void DisplayErrors() {
            if (Controls.Contains(errorMessage)) {
                Controls.Remove(errorMessage);
            }

            foreach (Control control in Controls) {
				control.Hide();
            }

        	Controls.Add(errorMessage);
        	errorMessage.Dock = DockStyle.Fill;
		}

        protected void HideErrors() {
            if (Controls.Contains(errorMessage)) {
                Controls.Remove(errorMessage);
            }

            foreach (Control control in Controls) {
                control.Show();
            }
        }

        protected override object GetService(Type service) {
            object obj = null;
            if (ParentWindow != null) {
                obj = ParentWindow.GetVsService(service);
            }
            if (obj == null) {
                obj = base.GetService(service);
            }
            return obj;
        }

        protected T GetService<T>(Type serviceType) where T : class {
            return GetService(serviceType) as T;
        }

        #region Properties window related
        protected string currentWorkitemId;

        protected void UpdatePropertyView(WorkitemDescriptor selectedItem) {
            //Try to get PropertiesFrame
            if (propertiesFrame == null) {
                IVsUIShell shell = GetService<IVsUIShell>(typeof(SVsUIShell));
                if (shell != null) {
                    Guid guidPropertyBrowser = new Guid(ToolWindowGuids.PropertyBrowser);
                    shell.FindToolWindow((uint)__VSFINDTOOLWIN.FTW_fForceCreate, ref guidPropertyBrowser, out propertiesFrame);
                }
            }

			if (propertiesFrame == null) {
                return;
            }
            int visible;
            propertiesFrame.IsOnScreen(out visible);
			if (visible == 1) {
                propertiesFrame.ShowNoActivate(); // Show() in original
            }

            SelectionContainer selectionContainer = new SelectionContainer();
            if (selectedItem != null) {
                selectionContainer.SelectedObjects = new object[] { selectedItem };
                currentWorkitemId = selectedItem.Entity.Id;
            } else {
                currentWorkitemId = null;
            }

            ITrackSelection track = GetService<ITrackSelection>(typeof(STrackSelection));
            if (track != null) {
                track.OnSelectChange(selectionContainer);
            }
        }

        public void ResetPropertyView() {
            UpdatePropertyView(null);
        }

        #endregion

        protected static void CenterControl(Control other, Control parent) {
            int x = (parent.Location.X + parent.Size.Width / 2) - other.Size.Width / 2;
            int y = (parent.Location.Y + parent.Size.Height / 2) - other.Size.Height / 2;
            other.Location = new Point(x, y);
        }

        // Now it is using this as parent
        protected void CenterControl(Control other) {
            int x = (Location.X + Size.Width / 2) - other.Size.Width / 2;
            int y = (Location.Y + Size.Height / 2) - other.Size.Height / 2;
            other.Location = new Point(x, y);
        }

    }
}