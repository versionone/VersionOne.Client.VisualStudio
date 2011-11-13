using NUnit.Framework;
using Rhino.Mocks;
using VersionOne.VisualStudio.DataLayer;
using VersionOne.VisualStudio.DataLayer.Entities;
using VersionOne.VisualStudio.VSPackage;
using VersionOne.VisualStudio.VSPackage.Controllers;
using VersionOne.VisualStudio.VSPackage.Controls;
using VersionOne.VisualStudio.VSPackage.Events;
using VersionOne.VisualStudio.VSPackage.Settings;

namespace VersionOne.VisualStudio.Tests {
    [TestFixture]
    public class ProjectTreeControllerTester {
        private ProjectTreeController controller;
        private IProjectTreeView viewMock;
        private IDataLayer dataLayerMock;
        private IEventDispatcher eventDispatcherMock;
        private ISettings settingsMock;
        
        private readonly MockRepository mockRepository = new MockRepository();

        [SetUp]
        public void SetUp() {
            viewMock = mockRepository.StrictMock<IProjectTreeView>();
            dataLayerMock = mockRepository.StrictMock<IDataLayer>();
            eventDispatcherMock = mockRepository.StrictMock<IEventDispatcher>();
            settingsMock = mockRepository.StrictMock<ISettings>();

            controller = new ProjectTreeController(dataLayerMock, settingsMock, eventDispatcherMock);
        }

        [Test]
        public void RegisterAndPrepareView() {
            Expect.Call(() => eventDispatcherMock.ModelChanged += null).IgnoreArguments();
            Expect.Call(viewMock.Controller).PropertyBehavior();
            Expect.Call(viewMock.UpdateData);

            mockRepository.ReplayAll();

            controller.RegisterView(viewMock);
            controller.PrepareView();
            controller.Prepare();
            Assert.AreEqual(viewMock.Controller, controller);
            Assert.AreEqual(controller.View, viewMock);

            mockRepository.VerifyAll();
        }

        [Test]
        public void Refresh() {
            Expect.Call(() => eventDispatcherMock.ModelChanged += null).IgnoreArguments();
            Expect.Call(viewMock.Controller).PropertyBehavior();
            Expect.Call(viewMock.UpdateData);

            mockRepository.ReplayAll();

            controller.RegisterView(viewMock);
            controller.Prepare();
            controller.HandleRefreshAction();

            mockRepository.VerifyAll();
        }

        [Test]
        public void ModelChangedNonRelevantChanges() {
            Expect.Call(() => eventDispatcherMock.ModelChanged += null).IgnoreArguments();
            var modelChangedRaiser = LastCall.GetEventRaiser();
            Expect.Call(viewMock.Controller).PropertyBehavior();

            mockRepository.ReplayAll();

            controller.RegisterView(viewMock);
            controller.Prepare();
            modelChangedRaiser.Raise(null, new ModelChangedArgs(EventReceiver.WorkitemView, EventContext.WorkitemsChanged));

            mockRepository.VerifyAll();
        }

        [Test]
        public void ModelChangedImportantChanges() {
            Expect.Call(() => eventDispatcherMock.ModelChanged += null).IgnoreArguments();
            var modelChangedRaiser = LastCall.GetEventRaiser();
            Expect.Call(viewMock.Controller).PropertyBehavior();
            Expect.Call(viewMock.UpdateData);

            mockRepository.ReplayAll();

            controller = new ProjectTreeController(dataLayerMock, settingsMock, eventDispatcherMock);
            controller.RegisterView(viewMock);
            controller.Prepare();
            modelChangedRaiser.Raise(null, new ModelChangedArgs(EventReceiver.ProjectView, EventContext.ProjectsRequested));

            mockRepository.VerifyAll();
        }

        [Test]
        public void ProjectSelected() {
            const string oldProject = "old project";
            const string newProject = "new project";
            
            SetupResult.For(settingsMock.SelectedProjectId).Return(oldProject);
            Project project = mockRepository.Stub<TestProject>();
            SetupResult.For(project.Id).Return(newProject);

            Expect.Call(() => eventDispatcherMock.ModelChanged += null).IgnoreArguments();
            Expect.Call(viewMock.Controller).PropertyBehavior();
            Expect.Call(settingsMock.SelectedProjectId).PropertyBehavior().Repeat.Twice();
            Expect.Call(settingsMock.StoreSettings);
            Expect.Call(dataLayerMock.CurrentProject).PropertyBehavior().IgnoreArguments();
            Expect.Call(viewMock.RefreshProperties);
            Expect.Call(() => eventDispatcherMock.Notify(null, null)).IgnoreArguments();

            mockRepository.ReplayAll();

            controller.RegisterView(viewMock);
            controller.Prepare();
            controller.HandleProjectSelected(project);

            mockRepository.VerifyAll();
        }

        [Test]
        public void GetProjects() {
            var waitCursorStub = mockRepository.Stub<IWaitCursor>();

            Expect.Call(() => eventDispatcherMock.ModelChanged += null).IgnoreArguments();
            Expect.Call(viewMock.Controller).PropertyBehavior();
            Expect.Call(viewMock.UpdateData);
            Expect.Call(viewMock.GetWaitCursor()).Return(waitCursorStub);
            Expect.Call(dataLayerMock.GetProjectTree()).Return(null);
            Expect.Call(viewMock.Projects).PropertyBehavior();
            Expect.Call(viewMock.CompleteProjectsRequest);

            mockRepository.ReplayAll();
            
            controller = new TestProjectTreeController(dataLayerMock, settingsMock, eventDispatcherMock);
            controller.RegisterView(viewMock);
            controller.PrepareView();
            controller.Prepare();
            controller.HandleProjectsRequest();

            mockRepository.VerifyAll();
        }

        private class TestProjectTreeController : ProjectTreeController {
            internal TestProjectTreeController(IDataLayer dataLayer, ISettings settings, IEventDispatcher eventDispatcher) : base(dataLayer, settings, eventDispatcher) { }

            protected override ITaskRunner GetTaskRunner(IWaitCursor waitCursor) {
                return new SynchronousTaskRunner(waitCursor);
            }
        }
    }
}