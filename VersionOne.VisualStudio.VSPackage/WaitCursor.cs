using System;
using System.Windows.Forms;
using VersionOne.VisualStudio.VSPackage.Controls;

namespace VersionOne.VisualStudio.VSPackage {
    public class WaitCursor : IWaitCursor {
        private readonly V1UserControl control;
        private readonly WaitSpinnerControl spinnerControl;

        public WaitCursor(V1UserControl control) {
            this.control = control;
            spinnerControl = new WaitSpinnerControl();
        }

        public void Show() {
            if(control == null || !control.IsHandleCreated) {
                return;
            }

            control.Cursor = Cursors.WaitCursor;

            control.Controls.Add(spinnerControl);
            spinnerControl.Left = Math.Max(0, (control.Width - spinnerControl.Width)/2);
            spinnerControl.Top = (control.Height - spinnerControl.Height)/2;
            spinnerControl.BringToFront();

            control.SetAccessibleControlsEnabled(false);
        }

        public void Hide() {
            control.Controls.Remove(spinnerControl);
            control.Cursor = Cursors.Default;
            control.SetAccessibleControlsEnabled(true);
        }
    }
}