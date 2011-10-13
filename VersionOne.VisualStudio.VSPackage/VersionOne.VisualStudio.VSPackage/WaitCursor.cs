using System;
using System.Windows.Forms;

namespace VersionOne.VisualStudio.VSPackage {
    public class WaitCursor : IDisposable {
        private readonly Control control;

        public WaitCursor(Control control) {
            this.control = control;
        }

        public IDisposable Show() {
            control.Cursor = Cursors.WaitCursor;
            control.Enabled = false;
            return this;
        }

        public void Dispose() {
            control.Cursor = Cursors.Default;
            control.Enabled = true;
        }
    }
}