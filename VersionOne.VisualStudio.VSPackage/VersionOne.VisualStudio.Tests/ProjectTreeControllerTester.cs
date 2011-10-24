using System.Threading;
using NUnit.Framework;
using Rhino.Mocks;
using VersionOne.VisualStudio.DataLayer;
using VersionOne.VisualStudio.DataLayer.Entities;
using VersionOne.VisualStudio.VSPackage;
using VersionOne.VisualStudio.VSPackage.Controllers;
using VersionOne.VisualStudio.VSPackage.Controls;
using VersionOne.VisualStudio.VSPackage.Events;
using VersionOne.VisualStudio.VSPackage.Settings;

using Is = Rhino.Mocks.Constraints.Is;

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
        }

        [Test]
        public void RegisterAndPrepareView() {
            Expect.Call(() => eventDispatcherMock.ModelChanged += null).IgnoreArguments();
            Expect.Call(viewMock.Controller).PropertyBehavior();
            Expect.Call(viewMock.UpdateData);

            mockRepository.ReplayAll();

            controller = new ProjectTreeController(dataLayerMock, settingsMock, eventDispatcherMock);
            controller.RegisterView(viewMock);
            controller.PrepareView();
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

            controller = new ProjectTreeController(dataLayerMock, settingsMock, eventDispatcherMock);
            controller.RegisterView(viewMock);
            controller.HandleRefreshAction();

            mockRepository.VerifyAll();
        }

        [Test]
        public void ModelChangedNonRelevantChanges() {
            Expect.Call(() => eventDispatcherMock.ModelChanged += null).IgnoreArguments();
            var modelChangedRaiser = LastCall.GetEventRaiser();
            Expect.Call(viewMock.Controller).PropertyBehavior();

            mockRepository.ReplayAll();

            controller = new ProjectTreeController(dataLayerMock, settingsMock, eventDispatcherMock);
            controller.RegisterView(viewMock);
            // this call should not affect Projects view
            modelChangedRaiser.Raise(null, ModelChangedArgs.WorkitemChanged);

            mockRepository.VerifyAll();
        }

        [Test]
        public void ModelChangedImportantChangesTest() {
            Expect.Call(() => eventDispatcherMock.ModelChanged += null).IgnoreArguments();
            var modelChangedRaiser = LastCall.GetEventRaiser();
            Expect.Call(viewMock.Controller).PropertyBehavior();
            Expect.Call(viewMock.UpdateData);

            mockRepository.ReplayAll();

            controller = new ProjectTreeController(dataLayerMock, settingsMock, eventDispatcherMock);
            controller.RegisterView(viewMock);
            // this call should cause controller to update view
            modelChangedRaiser.Raise(null, ModelChangedArgs.SettingsChanged);

            mockRepository.VerifyAll();
        }

        [Test]
        public void ProjectSelectedTest() {
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
            Expect.Call(() => eventDispatcherMock.InvokeModelChanged(null, null)).IgnoreArguments().Constraints(Is.Anything(), Is.Equal(ModelChangedArgs.ProjectChanged));

            mockRepository.ReplayAll();

            controller = new ProjectTreeController(dataLayerMock, settingsMock, eventDispatcherMock);
            controller.RegisterView(viewMock);
            controller.HandleProjectSelected(project);

            mockRepository.VerifyAll();
        }

        [Test]
        [Ignore("This test is currently designed for synchronous execution and would not wait for background routine to complete")]
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
            controller = new ProjectTreeController(dataLayerMock, settingsMock, eventDispatcherMock);
            controller.RegisterView(viewMock);
            controller.PrepareView();
            controller.HandleProjectsRequest();
            
            // Actually, we're able to cheat and run successfully, but it is not a decent solution
            // Thread.Sleep(200);
            
            mockRepository.VerifyAll();
        }
    }
}