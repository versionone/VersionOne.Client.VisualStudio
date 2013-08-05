using System.ComponentModel;
using Microsoft.VisualStudio.Shell;
using System.Windows.Forms;
using VersionOne.VisualStudio.DataLayer;
using VersionOne.VisualStudio.DataLayer.Logging;
using VersionOne.VisualStudio.VSPackage.Controllers;
using VersionOne.VisualStudio.VSPackage.Controls;
using System.Runtime.InteropServices;
using VersionOne.VisualStudio.VSPackage.Events;
using VersionOne.VisualStudio.VSPackage.Settings;

namespace VersionOne.VisualStudio.VSPackage {
	[Guid("4E2431D7-8F2C-952E-2696-B2CCAD694217")]
    public partial class OptionsPage : DialogPage {
        // The path to the image file which must be shown.
        private string selectedImagePath = string.Empty;
        private readonly OptionsPageControl optionsControl;
	    private readonly OptionsPageController controller;

	    public OptionsPage() {
	        var loggerFactory = ServiceLocator.Instance.Get<ILoggerFactory>();
            var dataLayer  = ServiceLocator.Instance.Get<IDataLayer>();
	        var settings = ServiceLocator.Instance.Get<ISettings>();
	        var eventDispatcher = ServiceLocator.Instance.Get<IEventDispatcher>();

            controller = new OptionsPageController(loggerFactory, dataLayer, settings, eventDispatcher);
            optionsControl = new OptionsPageControl();
            controller.RegisterView(optionsControl);
            controller.PrepareView();
	        controller.Prepare();
	    }

	    /// <summary>
        /// Gets the window an instance of DialogPage that it uses as its user interface.
        /// </summary>
        /// <devdoc>
        /// The window this dialog page will use for its UI.
        /// This window handle must be constant, so if you are
        /// returning a Windows Forms control you must make sure
        /// it does not recreate its handle.  If the window object
        /// implements IComponent it will be sited by the 
        /// dialog page so it can get access to global services.
        /// </devdoc>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        protected override IWin32Window Window {
            get {
                return optionsControl;
            }
        }
        /// <summary>
        /// Gets or sets the path to the image file.
        /// </summary>
        /// <remarks>The property that needs to be persisted.</remarks>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string CustomBitmap {
            get {
                return selectedImagePath;
            }
            set {
                selectedImagePath = value;
            }
        }

        /// <summary>
        /// Handles "Activate" messages from the Visual Studio environment.
        /// </summary>
        /// <devdoc>
        /// This method is called when Visual Studio wants to activate this page.  
        /// </devdoc>
        /// <remarks>If the Cancel property of the event is set to true, the page is not activated.</remarks>
        protected override void OnActivate(CancelEventArgs e) {
            controller.UpdateData();
            base.OnActivate(e);
        }

        /// <summary>
        /// Handles "Close" messages from the Visual Studio environment.
        /// </summary>
        /// <devdoc>
        /// This event is raised when the page is closed.
        /// </devdoc>
        protected override void OnApply(PageApplyEventArgs e) {
            controller.HandleSaveCommand();
        }
    }
}