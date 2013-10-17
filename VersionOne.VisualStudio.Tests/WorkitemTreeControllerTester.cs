using System;
using System.Collections.Generic;
using Rhino.Mocks;
using NUnit.Framework;
using VersionOne.VisualStudio.DataLayer;
using VersionOne.VisualStudio.DataLayer.Entities;
using VersionOne.VisualStudio.DataLayer.Logging;
using VersionOne.VisualStudio.VSPackage;
using VersionOne.VisualStudio.VSPackage.Controllers;
using VersionOne.VisualStudio.VSPackage.Controls;
using VersionOne.VisualStudio.VSPackage.Descriptors;
using VersionOne.VisualStudio.VSPackage.Events;
using VersionOne.VisualStudio.VSPackage.Settings;

namespace VersionOne.VisualStudio.Tests {
    [TestFixture]
    public class WorkitemTreeControllerTester : BaseTester {
        private WorkitemTreeController controller;
        private IDataLayer dataLayerMock;
        private IAssetCache assetCacheMock;
        private ISettings settingsMock;
        private IEventDispatcher eventDispatcherMock;
        private IWorkitemTreeView viewMock;
        private IWaitCursor waitCursorStub;
        private ILoggerFactory loggerFactoryMock;
        private IEffortTracking effortTrackingMock;
        private Configuration configuration;

        private readonly MockRepository mockRepository = new MockRepository();

        [SetUp]
        public void SetUp() {
            dataLayerMock = mockRepository.StrictMock<IDataLayer>();
            assetCacheMock = mockRepository.StrictMock<IAssetCache>();
            settingsMock = mockRepository.StrictMock<ISettings>();
            viewMock = mockRepository.StrictMock<IWorkitemTreeView>();
            waitCursorStub = mockRepository.Stub<IWaitCursor>();
            eventDispatcherMock = mockRepository.Stub<IEventDispatcher>();
            loggerFactoryMock = mockRepository.DynamicMock<ILoggerFactory>();
            effortTrackingMock = mockRepository.StrictMock<IEffortTracking>();
            loggerFactoryMock.Stub(x => x.GetLogger(null)).IgnoreArguments().Return(mockRepository.Stub<ILogger>());

            configuration = new Configuration();

            Container.Rebind<IDataLayer>().ToConstant(dataLayerMock);
            controller = new WorkitemTreeController(loggerFactoryMock, dataLayerMock, configuration, settingsMock, eventDispatcherMock);
        }

        private void ExpectRegisterAndPrepareView() {
            Expect.Call(() => eventDispatcherMock.ModelChanged += null).IgnoreArguments();
            Expect.Call(viewMock.Controller).PropertyBehavior();
            Expect.Call(dataLayerMock.CreateAssetCache()).Return(assetCacheMock);
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

            controller.Register(viewMock);
            Assert.AreEqual(controller, viewMock.Controller);
            controller.PrepareView();
            
            mockRepository.VerifyAll();
        }

        [Test]
        public void HandleRefresh() {
            ExpectRegisterAndPrepareView();
            Expect.Call(assetCacheMock.Drop);
            Expect.Call(dataLayerMock.EffortTracking).Return(effortTrackingMock);
            Expect.Call(effortTrackingMock.Refresh);
            Expect.Call(dataLayerMock.UpdateListPropertyValues);
            Expect.Call(() => eventDispatcherMock.Notify(null, new ModelChangedArgs(EventReceiver.WorkitemView, EventContext.WorkitemsRequested)));

            mockRepository.ReplayAll();

            controller.Register(viewMock);
            controller.PrepareView();
            controller.HandleRefreshCommand();

            mockRepository.VerifyAll();
        }

        [Test]
        public void CommitItem() {
            var workitemMock = mockRepository.PartialMock<TestWorkitem>(Guid.NewGuid().ToString(), true, assetCacheMock);
            var descriptor = new WorkitemDescriptor(workitemMock, new ColumnSetting[0], PropertyUpdateSource.WorkitemView, true);

            ExpectRegisterAndPrepareView();
            Expect.Call(viewMock.CurrentWorkitemDescriptor).Return(descriptor);
            Expect.Call(viewMock.GetWaitCursor()).Return(waitCursorStub);
            Expect.Call(workitemMock.CommitChanges);
            Expect.Call(() => eventDispatcherMock.Notify(null, new ModelChangedArgs(EventReceiver.ProjectView, EventContext.V1SettingsChanged)));

            mockRepository.ReplayAll();

            controller = new TestWorkitemTreeController(loggerFactoryMock, dataLayerMock, configuration, settingsMock, eventDispatcherMock);
            controller.Register(viewMock);
            controller.PrepareView();
            controller.CommitItem();

            mockRepository.VerifyAll();
        }

        [Test]
        public void CommitItemValidationFailure() {
            var workitemMock = mockRepository.PartialMock<TestWorkitem>(Guid.NewGuid().ToString(), true, assetCacheMock);
            var descriptor = new WorkitemDescriptor(workitemMock, new ColumnSetting[0], PropertyUpdateSource.WorkitemView, true);

            ExpectRegisterAndPrepareView();
            Expect.Call(viewMock.CurrentWorkitemDescriptor).Return(descriptor);
            Expect.Call(viewMock.GetWaitCursor()).Return(waitCursorStub);
            Expect.Call(workitemMock.CommitChanges).Throw(new ValidatorException(null));
            Expect.Call(() => viewMock.ShowErrorMessage(null)).IgnoreArguments();
            Expect.Call(() => eventDispatcherMock.Notify(null, new ModelChangedArgs(EventReceiver.WorkitemView, EventContext.WorkitemsChanged)));

            mockRepository.ReplayAll();

            controller = new TestWorkitemTreeController(loggerFactoryMock, dataLayerMock, configuration, settingsMock, eventDispatcherMock);
            controller.Register(viewMock);
            controller.PrepareView();
            controller.CommitItem();

            mockRepository.VerifyAll();
        }

        [Test]
        public void RevertNonVirtualItem() {
            var workitemMock = mockRepository.PartialMock<TestWorkitem>(Guid.NewGuid().ToString(), false, assetCacheMock);
            var descriptor = new WorkitemDescriptor(workitemMock, new ColumnSetting[0], PropertyUpdateSource.WorkitemView, true);

            ExpectRegisterAndPrepareView();
            Expect.Call(viewMock.CurrentWorkitemDescriptor).Return(descriptor);
            Expect.Call(workitemMock.RevertChanges);
            Expect.Call(viewMock.RefreshProperties);
            Expect.Call(viewMock.Refresh);

            mockRepository.ReplayAll();

            controller.Register(viewMock);
            controller.PrepareView();
            controller.RevertItem();

            mockRepository.VerifyAll();
        }

        [Test]
        public void SignupItem() {
            var workitemMock = mockRepository.PartialMock<TestWorkitem>(Guid.NewGuid().ToString(), false, assetCacheMock);
            var descriptor = new WorkitemDescriptor(workitemMock, new ColumnSetting[0], PropertyUpdateSource.WorkitemView, true);

            ExpectRegisterAndPrepareView();
            Expect.Call(viewMock.CurrentWorkitemDescriptor).Return(descriptor);
            Expect.Call(viewMock.GetWaitCursor()).Return(waitCursorStub);
            Expect.Call(workitemMock.Signup);
            Expect.Call(() => eventDispatcherMock.Notify(null, new ModelChangedArgs(EventReceiver.WorkitemView, EventContext.WorkitemsChanged)));

            mockRepository.ReplayAll();

            controller = new TestWorkitemTreeController(loggerFactoryMock, dataLayerMock, configuration, settingsMock, eventDispatcherMock);
            controller.Register(viewMock);
            controller.PrepareView();
            controller.SignupItem();

            mockRepository.VerifyAll();
        }

        [Test]
        public void QuickCloseItem() {
            var workitemMock = mockRepository.PartialMock<TestWorkitem>(Guid.NewGuid().ToString(), false, assetCacheMock);
            var descriptor = new WorkitemDescriptor(workitemMock, new ColumnSetting[0], PropertyUpdateSource.WorkitemView, true);

            ExpectRegisterAndPrepareView();
            Expect.Call(viewMock.CurrentWorkitemDescriptor).Return(descriptor);
            Expect.Call(viewMock.GetWaitCursor()).Return(waitCursorStub);
            Expect.Call(workitemMock.QuickClose);
            Expect.Call(() => eventDispatcherMock.Notify(null, new ModelChangedArgs(EventReceiver.WorkitemView, EventContext.WorkitemsChanged)));

            mockRepository.ReplayAll();

            controller = new TestWorkitemTreeController(loggerFactoryMock, dataLayerMock, configuration, settingsMock, eventDispatcherMock);
            controller.Register(viewMock);
            controller.PrepareView();
            controller.QuickCloseItem();

            mockRepository.VerifyAll();
        }

        [Test]
        public void CloseItem() {
            var workitemMock = mockRepository.PartialMock<TestWorkitem>(Guid.NewGuid().ToString(), true, assetCacheMock);

            ExpectRegisterAndPrepareView();
            Expect.Call(viewMock.GetWaitCursor()).Return(waitCursorStub);
            Expect.Call(workitemMock.Close);
            Expect.Call(() => eventDispatcherMock.Notify(null, new ModelChangedArgs(EventReceiver.WorkitemView, EventContext.WorkitemsChanged)));

            mockRepository.ReplayAll();

            controller = new TestWorkitemTreeController(loggerFactoryMock, dataLayerMock, configuration, settingsMock, eventDispatcherMock);
            controller.Register(viewMock);
            controller.PrepareView();
            controller.CloseItem(workitemMock);

            mockRepository.VerifyAll();
        }

        [Test]
        public void CloseItemValidationFailure() {
            var workitemMock = mockRepository.PartialMock<TestWorkitem>(Guid.NewGuid().ToString(), true, assetCacheMock);

            ExpectRegisterAndPrepareView();
            Expect.Call(viewMock.GetWaitCursor()).Return(waitCursorStub);
            Expect.Call(workitemMock.Close).Throw(new ValidatorException(null));
            Expect.Call(() => viewMock.ShowValidationInformationDialog(null)).IgnoreArguments();
            Expect.Call(() => eventDispatcherMock.Notify(null, new ModelChangedArgs(EventReceiver.WorkitemView, EventContext.WorkitemsChanged)));

            mockRepository.ReplayAll();

            controller = new TestWorkitemTreeController(loggerFactoryMock, dataLayerMock, configuration, settingsMock, eventDispatcherMock);
            controller.Register(viewMock);
            controller.PrepareView();
            controller.CloseItem(workitemMock);

            mockRepository.VerifyAll();
        }

        [Test]
        public void CloseItemWithGenericDataLayerException() {
            var workitemMock = mockRepository.PartialMock<TestWorkitem>(Guid.NewGuid().ToString(), true, assetCacheMock);

            ExpectRegisterAndPrepareView();
            Expect.Call(viewMock.GetWaitCursor()).Return(waitCursorStub);
            Expect.Call(workitemMock.Close).Throw(new DataLayerException(null));
            Expect.Call(() => viewMock.ShowErrorMessage(null)).IgnoreArguments();
            Expect.Call(() => eventDispatcherMock.Notify(null, new ModelChangedArgs(EventReceiver.WorkitemView, EventContext.WorkitemsChanged)));

            mockRepository.ReplayAll();

            controller = new TestWorkitemTreeController(loggerFactoryMock, dataLayerMock, configuration, settingsMock, eventDispatcherMock);
            controller.Register(viewMock);
            controller.PrepareView();
            controller.CloseItem(workitemMock);

            mockRepository.VerifyAll();
        }

        [Test]
        public void ModelChangedEvent() {
            ExpectRegisterAndPrepareView();

            Expect.Call(() => eventDispatcherMock.ModelChanged += null).IgnoreArguments();
            var raiser = LastCall.GetEventRaiser();
            
            mockRepository.ReplayAll();
            
            controller.Register(viewMock);
            controller.PrepareView();

            raiser.Raise(null, new ModelChangedArgs(EventReceiver.WorkitemView, EventContext.ProjectSelected));

            mockRepository.VerifyAll();
        }

        [Test]
        public void AddDefect() {
            var workitemMock = mockRepository.PartialMock<TestWorkitem>(Guid.NewGuid().ToString(), true, assetCacheMock);

            ExpectRegisterAndPrepareView();
            Expect.Call(dataLayerMock.CreateWorkitem(Entity.DefectType, null, assetCacheMock)).Return(workitemMock);
            Expect.Call(() => assetCacheMock.Add(workitemMock));
            Expect.Call(() => eventDispatcherMock.Notify(null, new ModelChangedArgs(EventReceiver.ProjectView, EventContext.ProjectSelected)));
            Expect.Call(() => viewMock.SelectWorkitem(workitemMock));
            Expect.Call(viewMock.Refresh);
            Expect.Call(viewMock.RefreshProperties);

            mockRepository.ReplayAll();
            controller.Register(viewMock);
            controller.PrepareView();
            controller.AddDefect();
            mockRepository.VerifyAll();
        }

        [Test]
        public void AddTask() {
            var parentWorkitemMock = mockRepository.PartialMock<TestWorkitem>(Guid.NewGuid().ToString(), true, assetCacheMock);
            var workitemMock = mockRepository.PartialMock<TestWorkitem>(Guid.NewGuid().ToString(), false, assetCacheMock);
            var descriptor = new WorkitemDescriptor(parentWorkitemMock, new ColumnSetting[0], PropertyUpdateSource.WorkitemView, true);

            ExpectRegisterAndPrepareView();
            Expect.Call(viewMock.CurrentWorkitemDescriptor).Return(descriptor);
            Expect.Call(dataLayerMock.CreateWorkitem(Entity.TaskType, parentWorkitemMock, assetCacheMock)).Return(workitemMock);
            Expect.Call(() => eventDispatcherMock.Notify(null, new ModelChangedArgs(EventReceiver.WorkitemView, EventContext.WorkitemsChanged)));
            Expect.Call(viewMock.ExpandCurrentNode);
            Expect.Call(() => viewMock.SelectWorkitem(workitemMock));
            Expect.Call(viewMock.Refresh);
            Expect.Call(viewMock.RefreshProperties);

            mockRepository.ReplayAll();
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
            Expect.Call(() => eventDispatcherMock.Notify(null, new ModelChangedArgs(EventReceiver.WorkitemView, EventContext.WorkitemsChanged)));

            mockRepository.ReplayAll();
            controller.Register(viewMock);
            controller.PrepareView();
            controller.HandleFilteringByOwner(onlyMyTasks);
            mockRepository.VerifyAll();
        }

        [Test]
        public void HandleSaveCommand() {
            ExpectRegisterAndPrepareView();
            Expect.Call(viewMock.GetWaitCursor()).Return(waitCursorStub);
            Expect.Call(() => dataLayerMock.CommitChanges(assetCacheMock));
            Expect.Call(assetCacheMock.Drop);
            Expect.Call(() => eventDispatcherMock.Notify(null, new ModelChangedArgs(EventReceiver.WorkitemView, EventContext.WorkitemsRequested)));

            mockRepository.ReplayAll();
            controller = new TestWorkitemTreeController(loggerFactoryMock, dataLayerMock, configuration, settingsMock, eventDispatcherMock);
            controller.Register(viewMock);
            controller.PrepareView();
            controller.HandleSaveCommand();

            mockRepository.VerifyAll();
        }

        [Test]
        public void HandleSaveCommandValidationException() {
            ExpectRegisterAndPrepareView();
            Expect.Call(viewMock.GetWaitCursor()).Return(waitCursorStub);
            Expect.Call(() => dataLayerMock.CommitChanges(assetCacheMock)).Throw(new ValidatorException(null));
            Expect.Call(assetCacheMock.Drop).Repeat.Never();
            Expect.Call(() => viewMock.ShowValidationInformationDialog(null)).IgnoreArguments();

            mockRepository.ReplayAll();
            controller = new TestWorkitemTreeController(loggerFactoryMock, dataLayerMock, configuration, settingsMock, eventDispatcherMock);
            controller.Register(viewMock);
            controller.PrepareView();
            controller.HandleSaveCommand();

            mockRepository.VerifyAll();
        }

        [Test]
        public void HandleSaveCommandGenericDataLayerException() {
            ExpectRegisterAndPrepareView();
            Expect.Call(viewMock.GetWaitCursor()).Return(waitCursorStub);
            Expect.Call(() => dataLayerMock.CommitChanges(assetCacheMock)).Throw(new DataLayerException(null));
            Expect.Call(assetCacheMock.Drop).Repeat.Never();
            Expect.Call(() => viewMock.ShowErrorMessage(null)).IgnoreArguments();
            Expect.Call(viewMock.ResetPropertyView);

            mockRepository.ReplayAll();
            controller = new TestWorkitemTreeController(loggerFactoryMock, dataLayerMock, configuration, settingsMock, eventDispatcherMock);
            controller.Register(viewMock);
            controller.PrepareView();
            controller.HandleSaveCommand();

            mockRepository.VerifyAll();
        }

        [Test]
        public void GetWorkitems() {
            var workitems = new List<Workitem>();

            ExpectRegisterAndPrepareView();
            Expect.Call(() => dataLayerMock.GetWorkitems(assetCacheMock));
            Expect.Call(settingsMock.ShowMyTasks).Return(false);
            Expect.Call(assetCacheMock.GetWorkitems(true)).Return(workitems);

            mockRepository.ReplayAll();
            controller = new TestWorkitemTreeController(loggerFactoryMock, dataLayerMock, configuration, settingsMock, eventDispatcherMock);
            controller.Register(viewMock);
            controller.PrepareView();
            var returnedWorkitems = controller.GetWorkitems();
            Assert.AreEqual(workitems, returnedWorkitems);

            mockRepository.VerifyAll();
        }

        [Test]
        public void GetWorkitemsFailure() {
            ExpectRegisterAndPrepareView();
            Expect.Call(() => dataLayerMock.GetWorkitems(assetCacheMock)).Throw(new DataLayerException(null));
            Expect.Call(() => viewMock.ShowErrorMessage(null)).IgnoreArguments();

            mockRepository.ReplayAll();
            controller = new TestWorkitemTreeController(loggerFactoryMock, dataLayerMock, configuration, settingsMock, eventDispatcherMock);
            controller.Register(viewMock);
            controller.PrepareView();
            var returnedWorkitems = controller.GetWorkitems();
            Assert.IsNull(returnedWorkitems);

            mockRepository.VerifyAll();
        }

        /// <summary>
        /// Use it instead of original controller anytime you need to test async methods.
        /// </summary>
        private class TestWorkitemTreeController : WorkitemTreeController {
            internal TestWorkitemTreeController(ILoggerFactory loggerFactory, IDataLayer dataLayer, Configuration configuration, ISettings settings, IEventDispatcher eventDispatcher)
                : base(loggerFactory, dataLayer, configuration, settings, eventDispatcher) { }

            protected override ITaskRunner GetTaskRunner(IWaitCursor waitCursor) {
                return new SynchronousTaskRunner(waitCursor);
            }
        }
    }
}