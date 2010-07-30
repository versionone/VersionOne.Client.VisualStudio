using NUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;

using VersionOne.VisualStudio.VSPackage;
using VersionOne.VisualStudio.VSPackage.Controllers;
using VersionOne.VisualStudio.VSPackage.Controls;
using VersionOne.VisualStudio.VSPackage.DataLayer;
using VersionOne.VisualStudio.VSPackage.Events;

namespace VersionOne.VSPackage.Tests {
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
            viewMock = mockRepository.CreateMock<IProjectTreeView>();
            dataLayerMock = mockRepository.CreateMock<IDataLayer>();
            eventDispatcherMock = mockRepository.CreateMock<IEventDispatcher>();
            settingsMock = mockRepository.CreateMock<ISettings>();
        }

        [Test]
        public void RegisterAndPrepareViewTest() {
            eventDispatcherMock.ModelChanged += null;
            LastCall.IgnoreArguments();
            Expect.Call(viewMock.Controller).PropertyBehavior();
            viewMock.UpdateData();

            mockRepository.ReplayAll();

            controller = new ProjectTreeController(dataLayerMock, settingsMock, eventDispatcherMock);
            controller.RegisterView(viewMock);
            controller.PrepareView();
            Assert.AreEqual(viewMock.Controller, controller);
            Assert.AreEqual(controller.View, viewMock);

            mockRepository.VerifyAll();
        }

        [Test]
        public void RefreshTest() {
            eventDispatcherMock.ModelChanged += null;
            LastCall.IgnoreArguments();
            Expect.Call(viewMock.Controller).PropertyBehavior();
            viewMock.UpdateData();

            mockRepository.ReplayAll();

            controller = new ProjectTreeController(dataLayerMock, settingsMock, eventDispatcherMock);
            controller.RegisterView(viewMock);
            controller.HandleRefreshAction();

            mockRepository.VerifyAll();
        }

        [Test]
        public void ModelChangedNonRelevantChangesTest() {
            eventDispatcherMock.ModelChanged += null;
            LastCall.IgnoreArguments();
            IEventRaiser modelChangedRaiser = LastCall.GetEventRaiser();
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
            eventDispatcherMock.ModelChanged += null;
            LastCall.IgnoreArguments();
            IEventRaiser modelChangedRaiser = LastCall.GetEventRaiser();
            Expect.Call(viewMock.Controller).PropertyBehavior();
            viewMock.UpdateData();

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

            eventDispatcherMock.ModelChanged += null;
            LastCall.IgnoreArguments();
            Expect.Call(viewMock.Controller).PropertyBehavior();
            Expect.Call(settingsMock.SelectedProjectId).PropertyBehavior().Repeat.Twice();
            settingsMock.StoreSettings();
            Expect.Call(dataLayerMock.CurrentProject).PropertyBehavior().IgnoreArguments();
            viewMock.RefreshProperties();
            eventDispatcherMock.InvokeModelChanged(controller, ModelChangedArgs.ProjectChanged);
            LastCall.IgnoreArguments();

            mockRepository.ReplayAll();

            controller = new ProjectTreeController(dataLayerMock, settingsMock, eventDispatcherMock);
            controller.RegisterView(viewMock);
            controller.HandleProjectSelected(project);

            mockRepository.VerifyAll();
        }
    }
}
