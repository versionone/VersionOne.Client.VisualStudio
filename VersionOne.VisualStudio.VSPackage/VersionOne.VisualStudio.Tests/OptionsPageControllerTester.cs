using NUnit.Framework;
using Rhino.Mocks;
using VersionOne.VisualStudio.DataLayer;
using VersionOne.VisualStudio.VSPackage.Controllers;
using VersionOne.VisualStudio.VSPackage.Controls;
using VersionOne.VisualStudio.VSPackage.Events;
using VersionOne.VisualStudio.VSPackage.Settings;

namespace VersionOne.VisualStudio.Tests {
    [TestFixture]
    public class OptionsPageControllerTester {
        private readonly MockRepository mockRepository = new MockRepository();

        private OptionsPageController controller;
        private IOptionsPageView viewMock;
        private IDataLayer dataLayerMock;
        private ISettings settingsStub;
        private IEventDispatcher eventDispatcherMock;

        [SetUp]
        public void SetUp() {
            dataLayerMock = mockRepository.StrictMock<IDataLayer>();
            settingsStub = mockRepository.Stub<ISettings>();
            eventDispatcherMock = mockRepository.StrictMock<IEventDispatcher>();
            viewMock = mockRepository.StrictMock<IOptionsPageView>();
            controller = new OptionsPageController(dataLayerMock, settingsStub, eventDispatcherMock);
        }

        [Test]
        public void HandleModelChanged() {
            Expect.Call(viewMock.Controller).PropertyBehavior().IgnoreArguments();
            Expect.Call(viewMock.Model).PropertyBehavior().IgnoreArguments();
            Expect.Call(() => eventDispatcherMock.ModelChanged += null).IgnoreArguments();
            var eventRaiser = LastCall.GetEventRaiser();
            Expect.Call(dataLayerMock.Connect(null)).Return(true).IgnoreArguments();
            Expect.Call(() => eventDispatcherMock.Notify(null, new ModelChangedArgs(EventReceiver.ProjectView, EventContext.ProjectsRequested))).IgnoreArguments();

            mockRepository.ReplayAll();
            
            controller.RegisterView(viewMock);
            controller.PrepareView();
            controller.Prepare();
            eventRaiser.Raise(null, new ModelChangedArgs(EventReceiver.OptionsView, EventContext.V1SettingsChanged));

            mockRepository.VerifyAll();
        }

        [Test]
        public void Save() {
            Expect.Call(viewMock.Controller).PropertyBehavior().IgnoreArguments();
            Expect.Call(viewMock.Model).PropertyBehavior().IgnoreArguments();
            Expect.Call(() => eventDispatcherMock.ModelChanged += null).IgnoreArguments();
            Expect.Call(viewMock.UpdateModel);
            Expect.Call(() => eventDispatcherMock.Notify(this, new ModelChangedArgs(EventReceiver.OptionsView, EventContext.V1SettingsChanged))).IgnoreArguments();

            mockRepository.ReplayAll();
            
            controller.RegisterView(viewMock);
            controller.PrepareView();
            controller.Prepare();
            controller.HandleSaveCommand();

            mockRepository.VerifyAll();
        }

        [Test]
        public void SaveFailure() {
            Expect.Call(viewMock.Controller).PropertyBehavior().IgnoreArguments();
            Expect.Call(viewMock.Model).PropertyBehavior().IgnoreArguments();
            Expect.Call(() => eventDispatcherMock.ModelChanged += null).IgnoreArguments();
            Expect.Call(viewMock.UpdateModel);
            Expect.Call(() => eventDispatcherMock.Notify(this, null)).IgnoreArguments().Throw(new DataLayerException(null));
            Expect.Call(() => viewMock.ShowErrorMessage(null, null)).IgnoreArguments();

            mockRepository.ReplayAll();
            
            controller.RegisterView(viewMock);
            controller.PrepareView();
            controller.Prepare();
            controller.HandleSaveCommand();

            mockRepository.VerifyAll();
        }

        [Test]
        public void VerifyConnection() {
            Expect.Call(viewMock.Controller).PropertyBehavior().IgnoreArguments();
            Expect.Call(viewMock.Model).PropertyBehavior().IgnoreArguments();
            Expect.Call(() => eventDispatcherMock.ModelChanged += null).IgnoreArguments();
            Expect.Call(() => dataLayerMock.CheckConnection(null)).IgnoreArguments();
            Expect.Call(() => viewMock.ShowMessage(null, null)).IgnoreArguments();

            mockRepository.ReplayAll();
            
            controller.RegisterView(viewMock);
            controller.PrepareView();
            controller.Prepare();
            controller.HandleVerifyConnectionCommand(settingsStub);

            mockRepository.VerifyAll();
        }

        [Test]
        public void VerifyConnectionFailure() {
            Expect.Call(viewMock.Controller).PropertyBehavior().IgnoreArguments();
            Expect.Call(viewMock.Model).PropertyBehavior().IgnoreArguments();
            Expect.Call(() => eventDispatcherMock.ModelChanged += null).IgnoreArguments();
            Expect.Call(() => dataLayerMock.CheckConnection(null)).IgnoreArguments().Throw(new DataLayerException(null));
            Expect.Call(() => viewMock.ShowErrorMessage(null, null)).IgnoreArguments();

            mockRepository.ReplayAll();
            
            controller.RegisterView(viewMock);
            controller.PrepareView();
            controller.Prepare();
            controller.HandleVerifyConnectionCommand(settingsStub);

            mockRepository.VerifyAll();
        }
    }
}
