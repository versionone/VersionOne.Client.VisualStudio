using System;

namespace VersionOne.VisualStudio.VSPackage {
    public interface ITaskRunner {
        void Run(Action task, Action onComplete, Action<Exception> onError = null);
    }
}