using System;
using VersionOne.VisualStudio.DataLayer;
using VersionOne.VisualStudio.VSPackage.Events;
using VersionOne.VisualStudio.VSPackage.Settings;

namespace VersionOne.VisualStudio.VSPackage.Controllers {
    public abstract class BaseController {
        protected readonly ISettings Settings;
        protected readonly IDataLayer DataLayer;
        protected readonly IEventDispatcher EventDispatcher;

        protected abstract EventReceiver ReceiverType { get; }

        protected BaseController(IDataLayer dataLayer, ISettings settings, IEventDispatcher eventDispatcher) {
            Settings = settings;
            DataLayer = dataLayer;
            EventDispatcher = eventDispatcher;
        }

        public void Prepare() {
            EventDispatcher.ModelChanged += ModelChanged;
        }

        public void Unsubscribe() {
            EventDispatcher.ModelChanged -= ModelChanged;
        }

        private void ModelChanged(object sender, ModelChangedArgs e) {
            if(ShouldHandleModelChangedEvent(e)) {
                HandleModelChanged(sender, e);
            }
        }

        protected virtual void HandleModelChanged(object sender, ModelChangedArgs e) { }

        private bool ShouldHandleModelChangedEvent(ModelChangedArgs e) {
            return ReceiverType == e.Receiver || e.Receiver == EventReceiver.All;
        }

        protected void RunTaskAsync(IWaitCursor waitCursor, Action task, Action onComplete, Action<Exception> onError = null) {
            GetTaskRunner(waitCursor).Run(task, onComplete, onError);
        }

        protected virtual ITaskRunner GetTaskRunner(IWaitCursor waitCursor) {
            return new BackgroundTaskRunner(waitCursor);
        }
    }
}