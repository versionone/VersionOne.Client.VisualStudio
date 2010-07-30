using System;
using System.Windows.Forms;

using Rhino.Mocks;
using NUnit.Framework;
using Rhino.Mocks.Interfaces;

using VersionOne.VisualStudio.VSPackage;
using VersionOne.VisualStudio.VSPackage.Controllers;
using VersionOne.VisualStudio.VSPackage.Controls;
using VersionOne.VisualStudio.VSPackage.DataLayer;
using VersionOne.VisualStudio.VSPackage.Descriptors;
using VersionOne.VisualStudio.VSPackage.Events;
using VersionOne.VisualStudio.VSPackage.Settings;

namespace VersionOne.VSPackage.Tests {
    [TestFixture]
    public class WorkitemTreeControllerTester {
        private WorkitemTreeController controller;
        private IDataLayer dataLayerMock;
        private ISettings settingsMock;
        private IEventDispatcher eventDispatcherMock;
        private IWorkitemTreeView viewMock;

        private readonly MockRepository mockRepository = new MockRepository();

        [SetUp]
        public void SetUp() {
            dataLayerMock = mockRepository.CreateMock<IDataLayer>();
            settingsMock = mockRepository.CreateMock<ISettings>();
            viewMock = mockRepository.CreateMock<IWorkitemTreeView>();
            eventDispatcherMock = mockRepository.CreateMock<IEventDispatcher>();
        }

        private void ExpectRegisterAndPrepareView() {
            eventDispatcherMock.ModelChanged += null;
            LastCall.IgnoreArguments();
            eventDispatcherMock.WorkitemPropertiesUpdated += null;
            LastCall.IgnoreArguments();
            Expect.Call(viewMock.Controller).PropertyBehavior();
            Expect.Call(dataLayerMock.CurrentProject).Return(null);
            Expect.Call(viewMock.Title).IgnoreArguments().PropertyBehavior();
            Expect.Call(viewMock.Model).IgnoreArguments().PropertyBehavior();
            viewMock.ReconfigureTreeColumns();
            Expect.Call(viewMock.CheckSettingsAreValid()).Return(true);
            viewMock.ReconfigureTreeColumns();
            viewMock.SetSelection();
            viewMock.Refresh();
            Expect.Call(viewMock.CurrentWorkitemDescriptor).Return(null);
            Expect.Call(viewMock.AddTaskCommandEnabled).IgnoreArguments().PropertyBehavior();
            Expect.Call(viewMock.AddTestCommandEnabled).IgnoreArguments().PropertyBehavior();
            Expect.Call(viewMock.AddDefectCommandEnabled).IgnoreArguments().PropertyBehavior();
        }

        [Test]
        public void RegisterAndPrepareViewTest() {
            ExpectRegisterAndPrepareView();

            mockRepository.ReplayAll();

            controller = new WorkitemTreeController(dataLayerMock, settingsMock, eventDispatcherMock);
            controller.Register(viewMock);
            Assert.AreEqual(controller, viewMock.Controller);
            controller.PrepareView();
            
            mockRepository.VerifyAll();
        }

        [Test]
        public void HandleRefreshTest() {
            ExpectRegisterAndPrepareView();
            dataLayerMock.Reconnect();
            eventDispatcherMock.InvokeModelChanged(null, ModelChangedArgs.SettingsChanged);

            mockRepository.ReplayAll();

            controller = new WorkitemTreeController(dataLayerMock, settingsMock, eventDispatcherMock);
            controller.Register(viewMock);
            controller.PrepareView();
            controller.HandleRefreshCommand();

            mockRepository.VerifyAll();
        }

        [Test]
        public void HandleRefreshWithExceptionTest() {
            ExpectRegisterAndPrepareView();
            dataLayerMock.Reconnect();
            LastCall.Throw(new DataLayerException(null));
            viewMock.ResetPropertyView();
            eventDispatcherMock.InvokeModelChanged(null, ModelChangedArgs.SettingsChanged);

            mockRepository.ReplayAll();

            controller = new WorkitemTreeController(dataLayerMock, settingsMock, eventDispatcherMock);
            controller.Register(viewMock);
            controller.PrepareView();
            controller.HandleRefreshCommand();

            mockRepository.VerifyAll();
        }

        [Test]
        public void CommitItemTest() {
            TestWorkitem workitemMock = mockRepository.PartialMock<TestWorkitem>(Guid.NewGuid().ToString(), true);
            WorkitemDescriptor descriptor = new WorkitemDescriptor(workitemMock, new ColumnSetting[0], PropertyUpdateSource.WorkitemView, true);

            ExpectRegisterAndPrepareView();
            Expect.Call(viewMock.CurrentWorkitemDescriptor).Return(descriptor);
            workitemMock.CommitChanges();
            eventDispatcherMock.InvokeModelChanged(null, ModelChangedArgs.WorkitemChanged);

            mockRepository.ReplayAll();

            controller = new WorkitemTreeController(dataLayerMock, settingsMock, eventDispatcherMock);
            controller.Register(viewMock);
            controller.PrepareView();
            controller.CommitItem();

            mockRepository.VerifyAll();
        }

        [Test]
        public void RevertNonVirtualItemTest() {
            TestWorkitem workitemMock = mockRepository.PartialMock<TestWorkitem>(Guid.NewGuid().ToString(), false);
            WorkitemDescriptor descriptor = new WorkitemDescriptor(workitemMock, new ColumnSetting[0], PropertyUpdateSource.WorkitemView, true);

            ExpectRegisterAndPrepareView();
            Expect.Call(viewMock.CurrentWorkitemDescriptor).Return(descriptor);
            workitemMock.RevertChanges();
            viewMock.RefreshProperties();
            viewMock.Refresh();

            mockRepository.ReplayAll();

            controller = new WorkitemTreeController(dataLayerMock, settingsMock, eventDispatcherMock);
            controller.Register(viewMock);
            controller.PrepareView();
            controller.RevertItem();

            mockRepository.VerifyAll();
        }

        [Test]
        public void SignupItemTest() {
            TestWorkitem workitemMock = mockRepository.PartialMock<TestWorkitem>(Guid.NewGuid().ToString(), false);
            WorkitemDescriptor descriptor = new WorkitemDescriptor(workitemMock, new ColumnSetting[0], PropertyUpdateSource.WorkitemView, true);

            ExpectRegisterAndPrepareView();
            Expect.Call(viewMock.CurrentWorkitemDescriptor).Return(descriptor);
            workitemMock.Signup();
            eventDispatcherMock.InvokeModelChanged(null, ModelChangedArgs.WorkitemChanged);

            mockRepository.ReplayAll();

            controller = new WorkitemTreeController(dataLayerMock, settingsMock, eventDispatcherMock);
            controller.Register(viewMock);
            controller.PrepareView();
            controller.SignupItem();

            mockRepository.VerifyAll();
        }

        [Test]
        public void QuickCloseItemTest() {
            TestWorkitem workitemMock = mockRepository.PartialMock<TestWorkitem>(Guid.NewGuid().ToString(), false);
            WorkitemDescriptor descriptor = new WorkitemDescriptor(workitemMock, new ColumnSetting[0], PropertyUpdateSource.WorkitemView, true);

            ExpectRegisterAndPrepareView();
            Expect.Call(viewMock.CurrentWorkitemDescriptor).Return(descriptor);
            workitemMock.QuickClose();
            eventDispatcherMock.InvokeModelChanged(null, ModelChangedArgs.WorkitemChanged);

            mockRepository.ReplayAll();

            controller = new WorkitemTreeController(dataLayerMock, settingsMock, eventDispatcherMock);
            controller.Register(viewMock);
            controller.PrepareView();
            controller.QuickCloseItem();

            mockRepository.VerifyAll();
        }

        [Test]
        public void CloseItemTest() {
            TestWorkitem workitemMock = mockRepository.PartialMock<TestWorkitem>(Guid.NewGuid().ToString(), true);
            WorkitemDescriptor descriptor = new WorkitemDescriptor(workitemMock, new ColumnSetting[0], PropertyUpdateSource.WorkitemView, true);

            ExpectRegisterAndPrepareView();
            Expect.Call(viewMock.CurrentWorkitemDescriptor).Return(descriptor);
            // TODO refactor view to be able not to reference System.Windows.Forms here and in controller
            Expect.Call(viewMock.ShowCloseWorkitemDialog(workitemMock)).Return(DialogResult.OK);
            eventDispatcherMock.InvokeModelChanged(null, ModelChangedArgs.WorkitemChanged);

            mockRepository.ReplayAll();

            controller = new WorkitemTreeController(dataLayerMock, settingsMock, eventDispatcherMock);
            controller.Register(viewMock);
            controller.PrepareView();
            controller.CloseItem();

            mockRepository.VerifyAll();
        }

        [Test]
        public void ModelChangedEventTest() {
            eventDispatcherMock.ModelChanged += null;
            LastCall.IgnoreArguments();
            IEventRaiser raiser = LastCall.GetEventRaiser();
            eventDispatcherMock.WorkitemPropertiesUpdated += null;
            LastCall.IgnoreArguments();
            Expect.Call(viewMock.Controller).PropertyBehavior();
            Expect.Call(dataLayerMock.CurrentProject).Return(null);
            Expect.Call(viewMock.Title).IgnoreArguments().PropertyBehavior();
            Expect.Call(viewMock.Model).IgnoreArguments().PropertyBehavior();
            viewMock.ReconfigureTreeColumns();
            Expect.Call(viewMock.CheckSettingsAreValid()).Return(true);
            viewMock.ReconfigureTreeColumns();
            viewMock.SetSelection();
            viewMock.Refresh();
            Expect.Call(viewMock.CurrentWorkitemDescriptor).Return(null);
            Expect.Call(viewMock.AddTaskCommandEnabled).IgnoreArguments().PropertyBehavior();
            Expect.Call(viewMock.AddTestCommandEnabled).IgnoreArguments().PropertyBehavior();
            Expect.Call(viewMock.AddDefectCommandEnabled).IgnoreArguments().PropertyBehavior();

            Expect.Call(dataLayerMock.CurrentProject).Return(null);
            Expect.Call(viewMock.CheckSettingsAreValid()).Return(false);
            viewMock.ResetPropertyView();
            viewMock.Refresh();
            Expect.Call(viewMock.CurrentWorkitemDescriptor).Return(null);

            mockRepository.ReplayAll();

            controller = new WorkitemTreeController(dataLayerMock, settingsMock, eventDispatcherMock);
            controller.Register(viewMock);
            controller.PrepareView();
            raiser.Raise(null, ModelChangedArgs.SettingsChanged);

            mockRepository.VerifyAll();
        }

        [Test]
        public void AddDefectTest() {
            TestWorkitem workitemMock = mockRepository.PartialMock<TestWorkitem>(Guid.NewGuid().ToString(), true);

            ExpectRegisterAndPrepareView();
            Expect.Call(dataLayerMock.CreateWorkitem(Entity.DefectPrefix, null)).Return(workitemMock);
            eventDispatcherMock.InvokeModelChanged(null, ModelChangedArgs.ProjectChanged);
            viewMock.SelectWorkitem(workitemMock);
            viewMock.Refresh();
            viewMock.RefreshProperties();

            mockRepository.ReplayAll();
            controller = new WorkitemTreeController(dataLayerMock, settingsMock, eventDispatcherMock);
            controller.Register(viewMock);
            controller.PrepareView();
            controller.AddDefect();
            mockRepository.VerifyAll();
        }

        [Test]
        public void AddTaskTest() {
            TestWorkitem parentWorkitemMock = mockRepository.PartialMock<TestWorkitem>(Guid.NewGuid().ToString(), true);
            TestWorkitem workitemMock = mockRepository.PartialMock<TestWorkitem>(Guid.NewGuid().ToString(), false);
            WorkitemDescriptor descriptor = new WorkitemDescriptor(parentWorkitemMock, new ColumnSetting[0], PropertyUpdateSource.WorkitemView, true);

            ExpectRegisterAndPrepareView();
            Expect.Call(viewMock.CurrentWorkitemDescriptor).Return(descriptor);
            Expect.Call(dataLayerMock.CreateWorkitem(Entity.TaskPrefix, parentWorkitemMock)).Return(workitemMock);
            eventDispatcherMock.InvokeModelChanged(null, ModelChangedArgs.WorkitemChanged);
            viewMock.ExpandCurrentNode();
            viewMock.SelectWorkitem(workitemMock);
            viewMock.Refresh();
            viewMock.RefreshProperties();

            mockRepository.ReplayAll();
            controller = new WorkitemTreeController(dataLayerMock, settingsMock, eventDispatcherMock);
            controller.Register(viewMock);
            controller.PrepareView();
            controller.AddTask();
            mockRepository.VerifyAll();
        }

        [Test]
        public void HandleFilteringByOwnerTest() {
            const bool onlyMyTasks = true;

            ExpectRegisterAndPrepareView();
            Expect.Call(settingsMock.ShowMyTasks).PropertyBehavior();
            settingsMock.StoreSettings();
            Expect.Call(dataLayerMock.ShowAllTasks).PropertyBehavior();
            eventDispatcherMock.InvokeModelChanged(null, ModelChangedArgs.WorkitemChanged);

            mockRepository.ReplayAll();
            controller = new WorkitemTreeController(dataLayerMock, settingsMock, eventDispatcherMock);
            controller.Register(viewMock);
            controller.PrepareView();
            controller.HandleFilteringByOwner(onlyMyTasks);
            mockRepository.VerifyAll();
        }
    }
}
