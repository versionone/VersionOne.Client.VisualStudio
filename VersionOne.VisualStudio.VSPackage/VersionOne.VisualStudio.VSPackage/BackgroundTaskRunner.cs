using System;
using System.ComponentModel;

namespace VersionOne.VisualStudio.VSPackage {
    /// <summary>
    /// A wrapper that simplifies execution of long running tasks in background thread so that UI would not be frozen.
    /// </summary>
    public partial class BackgroundTaskRunner : Component {
        private readonly IWaitCursor waitCursor;
        
        public BackgroundTaskRunner(IWaitCursor waitCursor) {
            this.waitCursor = waitCursor;
            InitializeComponent();
        }

        // TODO in most complex cases, we may need separate actions for: task itself, completion, error handling, successful completion
        public void Run(Action task, Action onComplete, Action<Exception> onError = null) {
            worker.DoWork += (sender, e) => task.Invoke();
            worker.RunWorkerCompleted += (sender, e) => {
                                             if (e.Error != null && onError != null) {
                                                 onError.Invoke(e.Error);
                                             }

                                             onComplete.Invoke();
                                             waitCursor.Dispose();
                                         };

            waitCursor.Show();
            worker.RunWorkerAsync();
        }
    }
}