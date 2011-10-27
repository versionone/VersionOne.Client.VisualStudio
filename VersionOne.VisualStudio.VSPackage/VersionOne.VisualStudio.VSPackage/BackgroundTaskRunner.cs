using System;
using System.ComponentModel;

namespace VersionOne.VisualStudio.VSPackage {
    /// <summary>
    /// A wrapper that simplifies execution of long running tasks in background thread so that UI would not be frozen.
    /// </summary>
    public partial class BackgroundTaskRunner : Component, ITaskRunner {
        private readonly IWaitCursor waitCursor;
        
        public BackgroundTaskRunner(IWaitCursor waitCursor) {
            this.waitCursor = waitCursor;
            InitializeComponent();
        }

        public void Run(Action task, Action onComplete, Action<Exception> onError = null) {
            worker.DoWork += (sender, e) => task.Invoke();
            worker.RunWorkerCompleted += (sender, e) => {
                                             if (e.Error != null && onError != null) {
                                                 onError.Invoke(e.Error);
                                             }

                                             onComplete.Invoke();
                                             waitCursor.Hide();
                                         };

            waitCursor.Show();
            worker.RunWorkerAsync();
        }
    }
}