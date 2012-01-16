using System;
using System.ComponentModel;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell.Interop;
using VersionOne.VisualStudio.DataLayer;
using VersionOne.VisualStudio.VSPackage.Dependencies;
using VersionOne.VisualStudio.VSPackage.Descriptors;
using Microsoft.VisualStudio.Shell;

namespace VersionOne.VisualStudio.VSPackage.Controls {
    // TODO move IDataLayer reference to controllers
    public partial class V1UserControl : UserControl, IWaitCursorProvider {
        protected readonly IDataLayer DataLayer;
        protected readonly ComponentResolver<IParentWindow> ParentWindowResolver;

        private IContainer components;
        private readonly ErrorMessageControl errorMessage;
        private IVsWindowFrame propertiesFrame;

        [Obsolete ("Only for designer.")]
        public V1UserControl() {
            InitializeComponent();
            errorMessage = new ErrorMessageControl();
            DataLayer = ServiceLocator.Instance.Get<IDataLayer>();
        }

        protected V1UserControl(ComponentResolver<IParentWindow> parentResolver, IDataLayer dataLayer) {
            ParentWindowResolver = parentResolver;
            InitializeComponent();
            errorMessage = new ErrorMessageControl();
            DataLayer = dataLayer;
        }

        /// <summary>
        /// Disable accessible controls. We cannot just disable the whole control because spinner won't be animated then.
        /// </summary>
        public virtual void SetAccessibleControlsEnabled(bool enabled) { }

        public IWaitCursor GetWaitCursor() {
            return new WaitCursor(this);
        }

        public bool CheckSettingsAreValid() {
            if(Controls.Contains(errorMessage)) {
                Controls.Remove(errorMessage);
            }

            if(!DataLayer.IsConnected) {
                DisplayErrors();
                return false;
            }

            foreach(Control control in Controls) {
                control.Show();
            }

            return true;
        }

        private void DisplayErrors() {
            if(Controls.Contains(errorMessage)) {
                Controls.Remove(errorMessage);
            }

            foreach(Control control in Controls) {
				control.Hide();
            }

        	Controls.Add(errorMessage);
        	errorMessage.Dock = DockStyle.Fill;
		}

        protected override object GetService(Type service) {
            object obj = null;

            var parentWindow = ParentWindowResolver.Resolve();

            if (parentWindow != null) {
                obj = parentWindow.GetVsService(service);
            }

            return obj ?? base.GetService(service);
        }

        private T GetService<T>(Type serviceType) where T : class {
            return GetService(serviceType) as T;
        }

        #region Properties window related
        
        protected string CurrentWorkitemId;

        protected void UpdatePropertyView(WorkitemDescriptor selectedItem) {
            //Try to get PropertiesFrame
            if(propertiesFrame == null) {
                var shell = GetService<IVsUIShell>(typeof(SVsUIShell));
                
                if(shell != null) {
                    var guidPropertyBrowser = new Guid(ToolWindowGuids.PropertyBrowser);
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

            var selectionContainer = new SelectionContainer();
            
            if (selectedItem != null) {
                selectionContainer.SelectedObjects = new object[] { selectedItem };
                CurrentWorkitemId = selectedItem.Entity.Id;
            } else {
                CurrentWorkitemId = null;
            }

            var track = GetService<ITrackSelection>(typeof(STrackSelection));
            
            if (track != null) {
                track.OnSelectChange(selectionContainer);
            }
        }

        public void ResetPropertyView() {
            UpdatePropertyView(null);
        }

        #endregion
    }
}