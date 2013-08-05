using System;
using System.Windows.Forms;
using VersionOne.VisualStudio.DataLayer;
using VersionOne.VisualStudio.DataLayer.Logging;
using VersionOne.VisualStudio.VSPackage.Controllers;
using VersionOne.VisualStudio.VSPackage.Events;
using VersionOne.VisualStudio.VSPackage.Settings;

namespace VersionOne.VisualStudio.VSPackage.Forms {
    public partial class OptionsDialog : Form {
        private readonly OptionsPageController controller;
        
        public OptionsDialog() {
            InitializeComponent();

            var loggerFactory = ServiceLocator.Instance.Get<ILoggerFactory>();
            var dataLayer  = ServiceLocator.Instance.Get<IDataLayer>();
	        var settings = ServiceLocator.Instance.Get<ISettings>();
	        var eventDispatcher = ServiceLocator.Instance.Get<IEventDispatcher>();
            
            controller = new OptionsPageController(loggerFactory, dataLayer, settings, eventDispatcher);
            controller.RegisterView(optionsPage);
            controller.PrepareView();
            controller.Prepare();

            controller.UpdateData();

            btnOk.Click += btnOk_Click;
        }

        private void btnOk_Click(object sender, EventArgs e) {
            controller.HandleSaveCommand();
            controller.Unsubscribe();
            Close();
        }
    }
}