using System;
using System.Windows.Forms;
using Rhino.Mocks;
using NUnit.Framework;
using Rhino.Mocks.Interfaces;
using VersionOne.VisualStudio.DataLayer;
using VersionOne.VisualStudio.DataLayer.Entities;
using VersionOne.VisualStudio.VSPackage.Controllers;
using VersionOne.VisualStudio.VSPackage.Controls;
using VersionOne.VisualStudio.VSPackage.Descriptors;
using VersionOne.VisualStudio.VSPackage.Events;
using VersionOne.VisualStudio.VSPackage.Settings;

namespace VersionOne.VisualStudio.Tests {
    [TestFixture]
    // TODO upgrade to .NET 4.0, fix ignored stuff
    public class WorkitemTreeControllerTester {
        private WorkitemTreeController controller;
        private IDataLayer dataLayerMock;
        private ISettings settingsMock;
        private IEventDispatcher eventDispatcherMock;
        private IWorkitemTreeView viewMock;

        private readonly MockRepository mockRepository = new MockRepository();

        [SetUp]
        public void SetUp() {
            dataLayerMock = mockRepository.StrictMock<IDataLayer>();
            settingsMock = mockRepository.StrictMock<ISettings>();
            viewMock = mockRepository.StrictMock<IWorkitemTreeView>();
            eventDispatcherMock = mockRepository.StrictMock<IEventDispatcher>();
        }

        private void ExpectRegisterAndPrepareView() {
            Expect.Call(() => eventDispatcherMock.ModelChanged += null).IgnoreArguments();
            Expect.Call(() => eventDispatcherMock.WorkitemPropertiesUpdated += null).IgnoreArguments();
            Expect.Call(viewMock.Controller).PropertyBehavior();
            Expect.Call(dataLayerMock.CurrentProject).Return(null);
            Expect.Call(viewMock.Title).IgnoreArguments().PropertyBehavior();
            Expect.Call(viewMock.Model).IgnoreArguments().PropertyBehavior();
            Expect.Call(viewMock.ReconfigureTreeColumns);
            Expect.Call(viewMock.CheckSettingsAreValid()).Return(true);
            Expect.Call(viewMock.ReconfigureTreeColumns);
            Expect.Call(viewMock.SetSelection);
            Expect.Call(viewMock.Refresh);
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
        [Ignore("TODO fix")]
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
        [Ignore("TODO fix")]
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
            var workitemMock = mockRepository.PartialMock<TestWorkitem>(Guid.NewGuid().ToString(), true);
            var descriptor = new WorkitemDescriptor(workitemMock, new ColumnSetting[0], PropertyUpdateSource.WorkitemView, true);

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
            var workitemMock = mockRepository.PartialMock<TestWorkitem>(Guid.NewGuid().ToString(), false);
            var descriptor = new WorkitemDescriptor(workitemMock, new ColumnSetting[0], PropertyUpdateSource.WorkitemView, true);

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
            var workitemMock = mockRepository.PartialMock<TestWorkitem>(Guid.NewGuid().ToString(), false);
            var descriptor = new WorkitemDescriptor(workitemMock, new ColumnSetting[0], PropertyUpdateSource.WorkitemView, true);

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
            var workitemMock = mockRepository.PartialMock<TestWorkitem>(Guid.NewGuid().ToString(), false);
            var descriptor = new WorkitemDescriptor(workitemMock, new ColumnSetting[0], PropertyUpdateSource.WorkitemView, true);

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
            var workitemMock = mockRepository.PartialMock<TestWorkitem>(Guid.NewGuid().ToString(), true);
            var descriptor = new WorkitemDescriptor(workitemMock, new ColumnSetting[0], PropertyUpdateSource.WorkitemView, true);

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
            Expect.Call(() => eventDispatcherMock.ModelChanged += null).IgnoreArguments();
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
            var workitemMock = mockRepository.PartialMock<TestWorkitem>(Guid.NewGuid().ToString(), true);

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
            var parentWorkitemMock = mockRepository.PartialMock<TestWorkitem>(Guid.NewGuid().ToString(), true);
            var workitemMock = mockRepository.PartialMock<TestWorkitem>(Guid.NewGuid().ToString(), false);
            var descriptor = new WorkitemDescriptor(parentWorkitemMock, new ColumnSetting[0], PropertyUpdateSource.WorkitemView, true);

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