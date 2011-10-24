using System;

namespace VersionOne.VisualStudio.VSPackage {
    public interface IWaitCursor : IDisposable {
        IDisposable Show();
    }
}