using System;
using VersionOne.VisualStudio.DataLayer;
using VersionOne.VisualStudio.VSPackage.Events;
using VersionOne.VisualStudio.VSPackage.Settings;

namespace VersionOne.VisualStudio.VSPackage.Controllers {
    public abstract class BaseController {
        protected readonly ISettings Settings;
        protected readonly IDataLayer DataLayer;
        protected readonly IEventDispatcher EventDispatcher;

        protected BaseController(IDataLayer dataLayer, ISettings settings, IEventDispatcher eventDispatcher) {
            Settings = settings;
            DataLayer = dataLayer;
            EventDispatcher = eventDispatcher;
        }

        protected void RunTaskAsync(IWaitCursor waitCursor, Action task, Action onComplete, Action<Exception> onError = null) {
            GetTaskRunner(waitCursor).Run(task, onComplete, onError);
        }

        protected virtual ITaskRunner GetTaskRunner(IWaitCursor waitCursor) {
            return new BackgroundTaskRunner(waitCursor);
        }
    }
}