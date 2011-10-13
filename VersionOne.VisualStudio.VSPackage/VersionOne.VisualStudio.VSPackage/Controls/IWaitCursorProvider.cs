using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VersionOne.VisualStudio.VSPackage.Controls {
    public interface IWaitCursorProvider {
        WaitCursor GetWaitCursor();
    }
}