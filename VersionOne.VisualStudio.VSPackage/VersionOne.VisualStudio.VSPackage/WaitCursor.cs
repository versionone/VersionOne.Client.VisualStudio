using System;
using System.Windows.Forms;
using VersionOne.VisualStudio.VSPackage.Controls;

namespace VersionOne.VisualStudio.VSPackage {
    public class WaitCursor : IWaitCursor {
        private readonly V1UserControl control;
        private readonly WaitSpinnerControl spinnerControl;

        public WaitCursor(V1UserControl control) {
            this.control = control;
            this.spinnerControl = new WaitSpinnerControl();
        }

        public IDisposable Show() {
            control.Cursor = Cursors.WaitCursor;
            
            control.Controls.Add(spinnerControl);
            spinnerControl.Left = Math.Max(0, (control.Width - spinnerControl.Width) / 2);
            spinnerControl.Top = (control.Height - spinnerControl.Height) / 2;
            spinnerControl.BringToFront();

            control.SetAccessibleControlsEnabled(false);
            return this;
        }

        public void Dispose() {
            control.Controls.Remove(spinnerControl);
            control.Cursor = Cursors.Default;
            control.SetAccessibleControlsEnabled(true);
        }
    }
}