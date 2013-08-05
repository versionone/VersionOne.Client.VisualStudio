using System;
using System.ComponentModel;

namespace VersionOne.VisualStudio.VSPackage {
    /// <summary>
    /// A wrapper that simplifies execution of long running tasks in background thread so that UI would not be frozen.
    /// </summary>
    public partial class SynchronousTaskRunner : Component, ITaskRunner {
        private readonly IWaitCursor waitCursor;
        
        public SynchronousTaskRunner(IWaitCursor waitCursor) {
            this.waitCursor = waitCursor;
            InitializeComponent();
        }

        /// <summary>
        /// This is synchronous projection of <see cref="BackgroundTaskRunner.Run"/>
        /// </summary>
        public void Run(Action task, Action onComplete, Action<Exception> onError = null) {
            waitCursor.Show();
            
            Exception exception = null;

            try {
                task.Invoke();
            } catch(Exception ex) {
                exception = ex;
            }

            if(onError != null && exception != null) {
                onError.Invoke(exception);
            }

            onComplete.Invoke();
            waitCursor.Hide();
        }
    }
}