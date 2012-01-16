using NUnit.Framework;
using Rhino.Mocks;
using VersionOne.VisualStudio.DataLayer;
using VersionOne.VisualStudio.DataLayer.Entities;
using VersionOne.VisualStudio.DataLayer.Logging;
using VersionOne.VisualStudio.VSPackage;
using VersionOne.VisualStudio.VSPackage.Controllers;
using VersionOne.VisualStudio.VSPackage.Controls;
using VersionOne.VisualStudio.VSPackage.Events;
using VersionOne.VisualStudio.VSPackage.Settings;

namespace VersionOne.VisualStudio.Tests {
    [TestFixture]
    public class ProjectTreeControllerTester : BaseTester {
        private ProjectTreeController controller;
        private IProjectTreeView viewMock;
        private IDataLayer dataLayerMock;
        private IEventDispatcher eventDispatcherMock;
        private ISettings settingsMock;
        private ILoggerFactory loggerFactoryMock;
        
        [SetUp]
        public void SetUp() {
            viewMock = MockRepository.StrictMock<IProjectTreeView>();
            dataLayerMock = MockRepository.StrictMock<IDataLayer>();
            eventDispatcherMock = MockRepository.StrictMock<IEventDispatcher>();
            settingsMock = MockRepository.StrictMock<ISettings>();
            loggerFactoryMock = MockRepository.DynamicMock<ILoggerFactory>();
            loggerFactoryMock.Stub(x => x.GetLogger(null)).IgnoreArguments().Return(MockRepository.Stub<ILogger>());

            controller = new ProjectTreeController(loggerFactoryMock, dataLayerMock, settingsMock, eventDispatcherMock);
        }

        [Test]
        public void RegisterAndPrepareView() {
            Expect.Call(() => eventDispatcherMock.ModelChanged += null).IgnoreArguments();
            Expect.Call(viewMock.Controller).PropertyBehavior();
            Expect.Call(viewMock.UpdateData);

            MockRepository.ReplayAll();

            controller.RegisterView(viewMock);
            controller.PrepareView();
            controller.Prepare();
            Assert.AreEqual(viewMock.Controller, controller);
            Assert.AreEqual(controller.View, viewMock);

            MockRepository.VerifyAll();
        }

        [Test]
        public void Refresh() {
            Expect.Call(() => eventDispatcherMock.ModelChanged += null).IgnoreArguments();
            Expect.Call(viewMock.Controller).PropertyBehavior();
            Expect.Call(viewMock.UpdateData);

            MockRepository.ReplayAll();

            controller.RegisterView(viewMock);
            controller.Prepare();
            controller.HandleRefreshAction();

            MockRepository.VerifyAll();
        }

        [Test]
        public void ModelChangedNonRelevantChanges() {
            Expect.Call(() => eventDispatcherMock.ModelChanged += null).IgnoreArguments();
            var modelChangedRaiser = LastCall.GetEventRaiser();
            Expect.Call(viewMock.Controller).PropertyBehavior();

            MockRepository.ReplayAll();

            controller.RegisterView(viewMock);
            controller.Prepare();
            modelChangedRaiser.Raise(null, new ModelChangedArgs(EventReceiver.WorkitemView, EventContext.WorkitemsChanged));

            MockRepository.VerifyAll();
        }

        [Test]
        public void ModelChangedImportantChanges() {
            Expect.Call(() => eventDispatcherMock.ModelChanged += null).IgnoreArguments();
            var modelChangedRaiser = LastCall.GetEventRaiser();
            Expect.Call(viewMock.Controller).PropertyBehavior();
            Expect.Call(viewMock.UpdateData);

            MockRepository.ReplayAll();

            controller = new ProjectTreeController(loggerFactoryMock, dataLayerMock, settingsMock, eventDispatcherMock);
            controller.RegisterView(viewMock);
            controller.Prepare();
            modelChangedRaiser.Raise(null, new ModelChangedArgs(EventReceiver.ProjectView, EventContext.ProjectsRequested));

            MockRepository.VerifyAll();
        }

        [Test]
        public void ProjectSelected() {
            const string oldProject = "old project";
            const string newProject = "new project";
            
            SetupResult.For(settingsMock.SelectedProjectId).Return(oldProject);
            Project project = MockRepository.Stub<TestProject>();
            SetupResult.For(project.Id).Return(newProject);

            Expect.Call(() => eventDispatcherMock.ModelChanged += null).IgnoreArguments();
            Expect.Call(viewMock.Controller).PropertyBehavior();
            Expect.Call(settingsMock.SelectedProjectId).PropertyBehavior().Repeat.Twice();
            Expect.Call(settingsMock.StoreSettings);
            Expect.Call(dataLayerMock.CurrentProject).PropertyBehavior().IgnoreArguments();
            Expect.Call(() => eventDispatcherMock.Notify(controller, new ModelChangedArgs(EventReceiver.WorkitemView, EventContext.WorkitemCacheInvalidated)));
            Expect.Call(viewMock.RefreshProperties);
            Expect.Call(() => eventDispatcherMock.Notify(null, null)).IgnoreArguments();

            MockRepository.ReplayAll();

            controller.RegisterView(viewMock);
            controller.Prepare();
            controller.HandleProjectSelected(project);

            MockRepository.VerifyAll();
        }

        [Test]
        public void GetProjects() {
            var waitCursorStub = MockRepository.Stub<IWaitCursor>();

            Expect.Call(() => eventDispatcherMock.ModelChanged += null).IgnoreArguments();
            Expect.Call(viewMock.Controller).PropertyBehavior();
            Expect.Call(viewMock.UpdateData);
            Expect.Call(viewMock.GetWaitCursor()).Return(waitCursorStub);
            Expect.Call(dataLayerMock.GetProjectTree()).Return(null);
            Expect.Call(viewMock.Projects).PropertyBehavior();
            Expect.Call(viewMock.CompleteProjectsRequest);

            MockRepository.ReplayAll();
            
            controller = new TestProjectTreeController(loggerFactoryMock, dataLayerMock, settingsMock, eventDispatcherMock);
            controller.RegisterView(viewMock);
            controller.PrepareView();
            controller.Prepare();
            controller.HandleProjectsRequest();

            MockRepository.VerifyAll();
        }

        private class TestProjectTreeController : ProjectTreeController {
            internal TestProjectTreeController(ILoggerFactory loggerFactory, IDataLayer dataLayer, ISettings settings, IEventDispatcher eventDispatcher) : base(loggerFactory, dataLayer, settings, eventDispatcher) { }

            protected override ITaskRunner GetTaskRunner(IWaitCursor waitCursor) {
                return new SynchronousTaskRunner(waitCursor);
            }
        }
    }
}