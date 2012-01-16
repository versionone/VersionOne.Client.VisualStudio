using NUnit.Framework;
using Rhino.Mocks;
using VersionOne.VisualStudio.DataLayer;
using VersionOne.VisualStudio.DataLayer.Logging;
using VersionOne.VisualStudio.VSPackage.Controllers;
using VersionOne.VisualStudio.VSPackage.Controls;
using VersionOne.VisualStudio.VSPackage.Events;
using VersionOne.VisualStudio.VSPackage.Settings;

namespace VersionOne.VisualStudio.Tests {
    [TestFixture]
    public class OptionsPageControllerTester : BaseTester {
        private OptionsPageController controller;
        private IOptionsPageView viewMock;
        private IDataLayer dataLayerMock;
        private ISettings settingsStub;
        private IEventDispatcher eventDispatcherMock;
        private ILoggerFactory loggerFactoryMock;

        [SetUp]
        public void SetUp() {
            dataLayerMock = MockRepository.StrictMock<IDataLayer>();
            settingsStub = MockRepository.Stub<ISettings>();
            eventDispatcherMock = MockRepository.StrictMock<IEventDispatcher>();
            viewMock = MockRepository.StrictMock<IOptionsPageView>();
            loggerFactoryMock = MockRepository.DynamicMock<ILoggerFactory>();
            loggerFactoryMock.Stub(x => x.GetLogger(null)).IgnoreArguments().Return(MockRepository.Stub<ILogger>());
            controller = new OptionsPageController(loggerFactoryMock, dataLayerMock, settingsStub, eventDispatcherMock);
        }

        [Test]
        public void HandleModelChanged() {
            Expect.Call(viewMock.Controller).PropertyBehavior().IgnoreArguments();
            Expect.Call(viewMock.Model).PropertyBehavior().IgnoreArguments();
            Expect.Call(() => eventDispatcherMock.ModelChanged += null).IgnoreArguments();
            var eventRaiser = LastCall.GetEventRaiser();
            Expect.Call(dataLayerMock.Connect(null)).Return(true).IgnoreArguments();
            Expect.Call(() => eventDispatcherMock.Notify(controller, new ModelChangedArgs(EventReceiver.WorkitemView, EventContext.WorkitemCacheInvalidated)));
            Expect.Call(() => eventDispatcherMock.Notify(controller, new ModelChangedArgs(EventReceiver.ProjectView, EventContext.ProjectsRequested))).IgnoreArguments();

            MockRepository.ReplayAll();
            
            controller.RegisterView(viewMock);
            controller.PrepareView();
            controller.Prepare();
            eventRaiser.Raise(controller, new ModelChangedArgs(EventReceiver.OptionsView, EventContext.V1SettingsChanged));

            MockRepository.VerifyAll();
        }

        [Test]
        public void Save() {
            Expect.Call(viewMock.Controller).PropertyBehavior().IgnoreArguments();
            Expect.Call(viewMock.Model).PropertyBehavior().IgnoreArguments();
            Expect.Call(() => eventDispatcherMock.ModelChanged += null).IgnoreArguments();
            Expect.Call(viewMock.UpdateModel);
            Expect.Call(() => eventDispatcherMock.Notify(this, new ModelChangedArgs(EventReceiver.OptionsView, EventContext.V1SettingsChanged))).IgnoreArguments();

            MockRepository.ReplayAll();
            
            controller.RegisterView(viewMock);
            controller.PrepareView();
            controller.Prepare();
            controller.HandleSaveCommand();

            MockRepository.VerifyAll();
        }

        [Test]
        public void SaveFailure() {
            Expect.Call(viewMock.Controller).PropertyBehavior().IgnoreArguments();
            Expect.Call(viewMock.Model).PropertyBehavior().IgnoreArguments();
            Expect.Call(() => eventDispatcherMock.ModelChanged += null).IgnoreArguments();
            Expect.Call(viewMock.UpdateModel);
            Expect.Call(() => eventDispatcherMock.Notify(this, null)).IgnoreArguments().Throw(new DataLayerException(null));
            Expect.Call(() => viewMock.ShowErrorMessage(null, null)).IgnoreArguments();

            MockRepository.ReplayAll();
            
            controller.RegisterView(viewMock);
            controller.PrepareView();
            controller.Prepare();
            controller.HandleSaveCommand();

            MockRepository.VerifyAll();
        }

        [Test]
        public void VerifyConnection() {
            Expect.Call(viewMock.Controller).PropertyBehavior().IgnoreArguments();
            Expect.Call(viewMock.Model).PropertyBehavior().IgnoreArguments();
            Expect.Call(() => eventDispatcherMock.ModelChanged += null).IgnoreArguments();
            Expect.Call(() => dataLayerMock.CheckConnection(null)).IgnoreArguments();
            Expect.Call(() => viewMock.ShowMessage(null, null)).IgnoreArguments();

            MockRepository.ReplayAll();
            
            controller.RegisterView(viewMock);
            controller.PrepareView();
            controller.Prepare();
            controller.HandleVerifyConnectionCommand(settingsStub);

            MockRepository.VerifyAll();
        }

        [Test]
        public void VerifyConnectionFailure() {
            Expect.Call(viewMock.Controller).PropertyBehavior().IgnoreArguments();
            Expect.Call(viewMock.Model).PropertyBehavior().IgnoreArguments();
            Expect.Call(() => eventDispatcherMock.ModelChanged += null).IgnoreArguments();
            Expect.Call(() => dataLayerMock.CheckConnection(null)).IgnoreArguments().Throw(new DataLayerException(null));
            Expect.Call(() => viewMock.ShowErrorMessage(null, null)).IgnoreArguments();

            MockRepository.ReplayAll();
            
            controller.RegisterView(viewMock);
            controller.PrepareView();
            controller.Prepare();
            controller.HandleVerifyConnectionCommand(settingsStub);

            MockRepository.VerifyAll();
        }
    }
}