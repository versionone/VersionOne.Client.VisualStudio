using System;
using System.Windows.Forms;
using Rhino.Mocks;
using NUnit.Framework;
using VersionOne.VisualStudio.DataLayer;
using VersionOne.VisualStudio.DataLayer.Entities;
using VersionOne.VisualStudio.VSPackage;
using VersionOne.VisualStudio.VSPackage.Controllers;
using VersionOne.VisualStudio.VSPackage.Controls;
using VersionOne.VisualStudio.VSPackage.Descriptors;
using VersionOne.VisualStudio.VSPackage.Events;
using VersionOne.VisualStudio.VSPackage.Settings;

namespace VersionOne.VisualStudio.Tests {
    [TestFixture]
    public class WorkitemTreeControllerTester {
        private WorkitemTreeController controller;
        private IDataLayer dataLayerMock;
        private ISettings settingsMock;
        private IEventDispatcher eventDispatcherMock;
        private IWorkitemTreeView viewMock;
        private IWaitCursor waitCursorStub;

        private readonly MockRepository mockRepository = new MockRepository();

        [SetUp]
        public void SetUp() {
            dataLayerMock = mockRepository.StrictMock<IDataLayer>();
            settingsMock = mockRepository.StrictMock<ISettings>();
            viewMock = mockRepository.StrictMock<IWorkitemTreeView>();
            waitCursorStub = mockRepository.Stub<IWaitCursor>();
            eventDispatcherMock = mockRepository.Stub<IEventDispatcher>();
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
        public void RegisterAndPrepareView() {
            ExpectRegisterAndPrepareView();

            mockRepository.ReplayAll();

            controller = new WorkitemTreeController(dataLayerMock, settingsMock, eventDispatcherMock);
            controller.Register(viewMock);
            Assert.AreEqual(controller, viewMock.Controller);
            controller.PrepareView();
            
            mockRepository.VerifyAll();
        }

        [Test]
        public void HandleRefresh() {
            ExpectRegisterAndPrepareView();
            Expect.Call(viewMock.GetWaitCursor()).Return(waitCursorStub);
            Expect.Call(dataLayerMock.Reconnect);
            Expect.Call(() => eventDispatcherMock.InvokeModelChanged(null, ModelChangedArgs.SettingsChanged));

            mockRepository.ReplayAll();

            controller = new TestWorkitemTreeController(dataLayerMock, settingsMock, eventDispatcherMock);
            controller.Register(viewMock);
            controller.PrepareView();
            controller.HandleRefreshCommand();

            mockRepository.VerifyAll();
        }

        [Test]
        public void HandleRefreshWithException() {
            ExpectRegisterAndPrepareView();
            Expect.Call(viewMock.GetWaitCursor()).Return(waitCursorStub);
            Expect.Call(dataLayerMock.Reconnect).Throw(new DataLayerException(null));
            Expect.Call(viewMock.ResetPropertyView);
            Expect.Call(() => eventDispatcherMock.InvokeModelChanged(null, ModelChangedArgs.SettingsChanged));

            mockRepository.ReplayAll();

            controller = new TestWorkitemTreeController(dataLayerMock, settingsMock, eventDispatcherMock);
            controller.Register(viewMock);
            controller.PrepareView();
            controller.HandleRefreshCommand();

            mockRepository.VerifyAll();
        }

        [Test]
        public void CommitItem() {
            var workitemMock = mockRepository.PartialMock<TestWorkitem>(Guid.NewGuid().ToString(), true);
            var descriptor = new WorkitemDescriptor(workitemMock, new ColumnSetting[0], PropertyUpdateSource.WorkitemView, true);

            ExpectRegisterAndPrepareView();
            Expect.Call(viewMock.CurrentWorkitemDescriptor).Return(descriptor);
            Expect.Call(viewMock.GetWaitCursor()).Return(waitCursorStub);
            Expect.Call(workitemMock.CommitChanges);
            Expect.Call(() => eventDispatcherMock.InvokeModelChanged(null, ModelChangedArgs.WorkitemChanged));

            mockRepository.ReplayAll();

            controller = new TestWorkitemTreeController(dataLayerMock, settingsMock, eventDispatcherMock);
            controller.Register(viewMock);
            controller.PrepareView();
            controller.CommitItem();

            mockRepository.VerifyAll();
        }

        [Test]
        public void CommitItemValidationFailure() {
            var workitemMock = mockRepository.PartialMock<TestWorkitem>(Guid.NewGuid().ToString(), true);
            var descriptor = new WorkitemDescriptor(workitemMock, new ColumnSetting[0], PropertyUpdateSource.WorkitemView, true);

            ExpectRegisterAndPrepareView();
            Expect.Call(viewMock.CurrentWorkitemDescriptor).Return(descriptor);
            Expect.Call(viewMock.GetWaitCursor()).Return(waitCursorStub);
            Expect.Call(workitemMock.CommitChanges).Throw(new ValidatorException(null));
            Expect.Call(() => viewMock.ShowErrorMessage(null)).IgnoreArguments();
            Expect.Call(() => eventDispatcherMock.InvokeModelChanged(null, ModelChangedArgs.WorkitemChanged));

            mockRepository.ReplayAll();

            controller = new TestWorkitemTreeController(dataLayerMock, settingsMock, eventDispatcherMock);
            controller.Register(viewMock);
            controller.PrepareView();
            controller.CommitItem();

            mockRepository.VerifyAll();
        }

        [Test]
        public void RevertNonVirtualItem() {
            var workitemMock = mockRepository.PartialMock<TestWorkitem>(Guid.NewGuid().ToString(), false);
            var descriptor = new WorkitemDescriptor(workitemMock, new ColumnSetting[0], PropertyUpdateSource.WorkitemView, true);

            ExpectRegisterAndPrepareView();
            Expect.Call(viewMock.CurrentWorkitemDescriptor).Return(descriptor);
            Expect.Call(workitemMock.RevertChanges);
            Expect.Call(viewMock.RefreshProperties);
            Expect.Call(viewMock.Refresh);

            mockRepository.ReplayAll();

            controller = new WorkitemTreeController(dataLayerMock, settingsMock, eventDispatcherMock);
            controller.Register(viewMock);
            controller.PrepareView();
            controller.RevertItem();

            mockRepository.VerifyAll();
        }

        [Test]
        public void SignupItem() {
            var workitemMock = mockRepository.PartialMock<TestWorkitem>(Guid.NewGuid().ToString(), false);
            var descriptor = new WorkitemDescriptor(workitemMock, new ColumnSetting[0], PropertyUpdateSource.WorkitemView, true);

            ExpectRegisterAndPrepareView();
            Expect.Call(viewMock.CurrentWorkitemDescriptor).Return(descriptor);
            Expect.Call(viewMock.GetWaitCursor()).Return(waitCursorStub);
            Expect.Call(workitemMock.Signup);
            Expect.Call(() => eventDispatcherMock.InvokeModelChanged(null, ModelChangedArgs.WorkitemChanged));

            mockRepository.ReplayAll();

            controller = new TestWorkitemTreeController(dataLayerMock, settingsMock, eventDispatcherMock);
            controller.Register(viewMock);
            controller.PrepareView();
            controller.SignupItem();

            mockRepository.VerifyAll();
        }

        [Test]
        public void QuickCloseItem() {
            var workitemMock = mockRepository.PartialMock<TestWorkitem>(Guid.NewGuid().ToString(), false);
            var descriptor = new WorkitemDescriptor(workitemMock, new ColumnSetting[0], PropertyUpdateSource.WorkitemView, true);

            ExpectRegisterAndPrepareView();
            Expect.Call(viewMock.CurrentWorkitemDescriptor).Return(descriptor);
            Expect.Call(viewMock.GetWaitCursor()).Return(waitCursorStub);
            Expect.Call(workitemMock.QuickClose);
            Expect.Call(() => eventDispatcherMock.InvokeModelChanged(null, ModelChangedArgs.WorkitemChanged));

            mockRepository.ReplayAll();

            controller = new TestWorkitemTreeController(dataLayerMock, settingsMock, eventDispatcherMock);
            controller.Register(viewMock);
            controller.PrepareView();
            controller.QuickCloseItem();

            mockRepository.VerifyAll();
        }

        [Test]
        public void CloseItem() {
            var workitemMock = mockRepository.PartialMock<TestWorkitem>(Guid.NewGuid().ToString(), true);
            var descriptor = new WorkitemDescriptor(workitemMock, new ColumnSetting[0], PropertyUpdateSource.WorkitemView, true);

            ExpectRegisterAndPrepareView();
            Expect.Call(viewMock.CurrentWorkitemDescriptor).Return(descriptor);
            // TODO refactor view to be able not to reference System.Windows.Forms here and in controller
            Expect.Call(viewMock.ShowCloseWorkitemDialog(workitemMock)).Return(DialogResult.OK);
            Expect.Call(() => eventDispatcherMock.InvokeModelChanged(null, ModelChangedArgs.WorkitemChanged));

            mockRepository.ReplayAll();

            controller = new WorkitemTreeController(dataLayerMock, settingsMock, eventDispatcherMock);
            controller.Register(viewMock);
            controller.PrepareView();
            controller.CloseItem();

            mockRepository.VerifyAll();
        }

        [Test]
        public void ModelChangedEvent() {
            Expect.Call(() => eventDispatcherMock.ModelChanged += null).IgnoreArguments();
            var raiser = LastCall.GetEventRaiser();
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

            Expect.Call(dataLayerMock.CurrentProject).Return(null);
            Expect.Call(viewMock.CheckSettingsAreValid()).Return(false);
            Expect.Call(viewMock.ResetPropertyView);
            Expect.Call(viewMock.Refresh);
            Expect.Call(viewMock.CurrentWorkitemDescriptor).Return(null);

            mockRepository.ReplayAll();

            controller = new WorkitemTreeController(dataLayerMock, settingsMock, eventDispatcherMock);
            controller.Register(viewMock);
            controller.PrepareView();
            raiser.Raise(null, ModelChangedArgs.SettingsChanged);

            mockRepository.VerifyAll();
        }

        [Test]
        public void AddDefect() {
            var workitemMock = mockRepository.PartialMock<TestWorkitem>(Guid.NewGuid().ToString(), true);

            ExpectRegisterAndPrepareView();
            Expect.Call(dataLayerMock.CreateWorkitem(Entity.DefectPrefix, null)).Return(workitemMock);
            Expect.Call(() => eventDispatcherMock.InvokeModelChanged(null, ModelChangedArgs.ProjectChanged));
            Expect.Call(() => viewMock.SelectWorkitem(workitemMock));
            Expect.Call(viewMock.Refresh);
            Expect.Call(viewMock.RefreshProperties);

            mockRepository.ReplayAll();
            controller = new WorkitemTreeController(dataLayerMock, settingsMock, eventDispatcherMock);
            controller.Register(viewMock);
            controller.PrepareView();
            controller.AddDefect();
            mockRepository.VerifyAll();
        }

        [Test]
        public void AddTask() {
            var parentWorkitemMock = mockRepository.PartialMock<TestWorkitem>(Guid.NewGuid().ToString(), true);
            var workitemMock = mockRepository.PartialMock<TestWorkitem>(Guid.NewGuid().ToString(), false);
            var descriptor = new WorkitemDescriptor(parentWorkitemMock, new ColumnSetting[0], PropertyUpdateSource.WorkitemView, true);

            ExpectRegisterAndPrepareView();
            Expect.Call(viewMock.CurrentWorkitemDescriptor).Return(descriptor);
            Expect.Call(dataLayerMock.CreateWorkitem(Entity.TaskPrefix, parentWorkitemMock)).Return(workitemMock);
            Expect.Call(() => eventDispatcherMock.InvokeModelChanged(null, ModelChangedArgs.WorkitemChanged));
            Expect.Call(viewMock.ExpandCurrentNode);
            Expect.Call(() => viewMock.SelectWorkitem(workitemMock));
            Expect.Call(viewMock.Refresh);
            Expect.Call(viewMock.RefreshProperties);

            mockRepository.ReplayAll();
            controller = new WorkitemTreeController(dataLayerMock, settingsMock, eventDispatcherMock);
            controller.Register(viewMock);
            controller.PrepareView();
            controller.AddTask();
            mockRepository.VerifyAll();
        }

        [Test]
        public void HandleFilteringByOwner() {
            const bool onlyMyTasks = true;

            ExpectRegisterAndPrepareView();
            Expect.Call(settingsMock.ShowMyTasks).PropertyBehavior();
            Expect.Call(settingsMock.StoreSettings);
            Expect.Call(dataLayerMock.ShowAllTasks).PropertyBehavior();
            Expect.Call(() => eventDispatcherMock.InvokeModelChanged(null, ModelChangedArgs.WorkitemChanged));

            mockRepository.ReplayAll();
            controller = new WorkitemTreeController(dataLayerMock, settingsMock, eventDispatcherMock);
            controller.Register(viewMock);
            controller.PrepareView();
            controller.HandleFilteringByOwner(onlyMyTasks);
            mockRepository.VerifyAll();
        }

        [Test]
        public void HandleSaveCommand() {
            ExpectRegisterAndPrepareView();
            Expect.Call(viewMock.GetWaitCursor()).Return(waitCursorStub);
            Expect.Call(dataLayerMock.CommitChanges);
            Expect.Call(dataLayerMock.Reconnect);
            Expect.Call(() => eventDispatcherMock.InvokeModelChanged(null, ModelChangedArgs.SettingsChanged));

            mockRepository.ReplayAll();
            controller = new TestWorkitemTreeController(dataLayerMock, settingsMock, eventDispatcherMock);
            controller.Register(viewMock);
            controller.PrepareView();
            controller.HandleSaveCommand();

            mockRepository.VerifyAll();
        }

        [Test]
        public void HandleSaveCommandValidationException() {
            ExpectRegisterAndPrepareView();
            Expect.Call(viewMock.GetWaitCursor()).Return(waitCursorStub);
            Expect.Call(dataLayerMock.CommitChanges).Throw(new ValidatorException(null));
            Expect.Call(dataLayerMock.Reconnect).Repeat.Never();
            Expect.Call(() => viewMock.ShowValidationInformationDialog(null)).IgnoreArguments();
            Expect.Call(() => eventDispatcherMock.InvokeModelChanged(null, ModelChangedArgs.SettingsChanged));

            mockRepository.ReplayAll();
            controller = new TestWorkitemTreeController(dataLayerMock, settingsMock, eventDispatcherMock);
            controller.Register(viewMock);
            controller.PrepareView();
            controller.HandleSaveCommand();

            mockRepository.VerifyAll();
        }

        [Test]
        public void HandleSaveCommandGenericDataLayerException() {
            ExpectRegisterAndPrepareView();
            Expect.Call(viewMock.GetWaitCursor()).Return(waitCursorStub);
            Expect.Call(dataLayerMock.CommitChanges).Throw(new DataLayerException(null));
            Expect.Call(dataLayerMock.Reconnect).Repeat.Never();
            Expect.Call(() => viewMock.ShowErrorMessage(null)).IgnoreArguments();
            Expect.Call(viewMock.ResetPropertyView);
            Expect.Call(() => eventDispatcherMock.InvokeModelChanged(null, ModelChangedArgs.SettingsChanged));

            mockRepository.ReplayAll();
            controller = new TestWorkitemTreeController(dataLayerMock, settingsMock, eventDispatcherMock);
            controller.Register(viewMock);
            controller.PrepareView();
            controller.HandleSaveCommand();

            mockRepository.VerifyAll();
        }

        [Test]
        public void HandleSaveCommandNullReferenceException() {
            ExpectRegisterAndPrepareView();
            Expect.Call(viewMock.GetWaitCursor()).Return(waitCursorStub);
            Expect.Call(dataLayerMock.CommitChanges);
            Expect.Call(dataLayerMock.Reconnect).Throw(new NullReferenceException());
            Expect.Call(() => viewMock.ShowErrorMessage(null)).IgnoreArguments();
            Expect.Call(viewMock.ResetPropertyView);
            Expect.Call(() => eventDispatcherMock.InvokeModelChanged(null, ModelChangedArgs.SettingsChanged));

            mockRepository.ReplayAll();
            controller = new TestWorkitemTreeController(dataLayerMock, settingsMock, eventDispatcherMock);
            controller.Register(viewMock);
            controller.PrepareView();
            controller.HandleSaveCommand();

            mockRepository.VerifyAll();
        }

        /// <summary>
        /// Use it instead of original controller anytime you need to test async methods.
        /// </summary>
        private class TestWorkitemTreeController : WorkitemTreeController {
            internal TestWorkitemTreeController(IDataLayer dataLayer, ISettings settings, IEventDispatcher eventDispatcher) : base(dataLayer, settings, eventDispatcher) { }

            protected override ITaskRunner GetTaskRunner(IWaitCursor waitCursor) {
                return new SynchronousTaskRunner(waitCursor);
            }
        }
    }
}