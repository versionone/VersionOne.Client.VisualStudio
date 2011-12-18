using NUnit.Framework;

using Rhino.Mocks;

using VersionOne.SDK.APIClient;
using VersionOne.VisualStudio.DataLayer;
using VersionOne.VisualStudio.DataLayer.Entities;

namespace VersionOne.VisualStudio.Tests {
    [TestFixture]
    public class EffortTrackingTester {
        private readonly MockRepository mockRepository = new MockRepository();
        private IVersionOneConnector connectorMock;
        private IV1Configuration configuration;

        [SetUp]
        public void SetUp() {
            connectorMock = mockRepository.StrictMock<IVersionOneConnector>();
            configuration = mockRepository.StrictMock<IV1Configuration>();
        }

        [Test]
        public void EffortTrackingInitTest() {
            InitEffortExpectations();

            mockRepository.ReplayAll();
            var effortTracking = new EffortTracking(connectorMock);
            effortTracking.Init();
            mockRepository.VerifyAll();

            Assert.IsTrue(effortTracking.TrackEffort);
            Assert.AreEqual(EffortTrackingLevel.PrimaryWorkitem, effortTracking.DefectTrackingLevel);
            Assert.AreEqual(EffortTrackingLevel.SecondaryWorkitem, effortTracking.StoryTrackingLevel);
        }

        private void InitEffortExpectations() {
            Expect.Call(connectorMock.V1Configuration).Return(configuration);
            Expect.Call(configuration.EffortTracking).Return(true);
            Expect.Call(configuration.StoryTrackingLevel).Return(TrackingLevel.Off);
            Expect.Call(configuration.DefectTrackingLevel).Return(TrackingLevel.On);
        }

        [Test]
        public void RefreshConfigTest() {
            using (mockRepository.Ordered()) {
                InitEffortExpectations();

                Expect.Call(connectorMock.LoadV1Configuration()).Return(configuration);
                Expect.Call(configuration.EffortTracking).Return(false);
                Expect.Call(configuration.StoryTrackingLevel).Return(TrackingLevel.On);
                Expect.Call(configuration.DefectTrackingLevel).Return(TrackingLevel.Off);
            }

            mockRepository.ReplayAll();
            var effortTracking = new EffortTracking(connectorMock);
            effortTracking.Init();
            effortTracking.Refresh();
            mockRepository.VerifyAll();

            Assert.IsFalse(effortTracking.TrackEffort);
            Assert.AreEqual(EffortTrackingLevel.SecondaryWorkitem, effortTracking.DefectTrackingLevel);
            Assert.AreEqual(EffortTrackingLevel.PrimaryWorkitem, effortTracking.StoryTrackingLevel);
        }

        [Test]
        public void ValidateDefectEffortTrackingPropertyTest() {
            var workitem = mockRepository.StrictMock<TestWorkitem>(null, null, null);

            InitEffortExpectations();
            SetupResult.For(workitem.TypePrefix).Return(Entity.DefectType);

            mockRepository.ReplayAll();

            var effortTracking = new EffortTracking(connectorMock);
            effortTracking.Init();
            Assert.IsFalse(effortTracking.AreEffortTrackingPropertiesReadOnly(workitem));

            mockRepository.VerifyAll();
        }

        [Test]
        public void ValidateDefectEffortTrackingPropertyReadOnlyTest() {
            var workitem = mockRepository.StrictMock<TestWorkitem>(null, null, null);

            Expect.Call(connectorMock.V1Configuration).Return(configuration);
            Expect.Call(configuration.EffortTracking).Return(true);
            Expect.Call(configuration.StoryTrackingLevel).Return(TrackingLevel.On);
            Expect.Call(configuration.DefectTrackingLevel).Return(TrackingLevel.Off);
            SetupResult.For(workitem.TypePrefix).Return(Entity.DefectType);

            mockRepository.ReplayAll();

            var effortTracking = new EffortTracking(connectorMock);
            effortTracking.Init();
            Assert.IsTrue(effortTracking.AreEffortTrackingPropertiesReadOnly(workitem));

            mockRepository.VerifyAll();
        }

        [Test]
        public void ValidateStoryEffortTrackingPropertyTest() {
            var workitem = mockRepository.StrictMock<TestWorkitem>(null, null, null);

            Expect.Call(connectorMock.V1Configuration).Return(configuration);
            Expect.Call(configuration.EffortTracking).Return(true);
            Expect.Call(configuration.StoryTrackingLevel).Return(TrackingLevel.On);
            Expect.Call(configuration.DefectTrackingLevel).Return(TrackingLevel.Off);
            SetupResult.For(workitem.TypePrefix).Return(Entity.StoryType);

            mockRepository.ReplayAll();

            var effortTracking = new EffortTracking(connectorMock);
            effortTracking.Init();
            Assert.IsFalse(effortTracking.AreEffortTrackingPropertiesReadOnly(workitem));

            mockRepository.VerifyAll();
        }

        [Test]
        public void ValidateStoryEffortTrackingPropertyReadOnlyTest() {
            var workitem = mockRepository.StrictMock<TestWorkitem>(null, null, null);

            InitEffortExpectations();
            SetupResult.For(workitem.TypePrefix).Return(Entity.StoryType);

            mockRepository.ReplayAll();

            var effortTracking = new EffortTracking(connectorMock);
            effortTracking.Init();
            Assert.IsTrue(effortTracking.AreEffortTrackingPropertiesReadOnly(workitem));

            mockRepository.VerifyAll();
        }

        [Test]
        public void ValidateTaskEffortTrackingPropertyTest() {
            var parent = mockRepository.StrictMock<TestWorkitem>(null, null, null);
            var workitem = mockRepository.StrictMock<TestWorkitem>(null, null, null);

            InitEffortExpectations();
            SetupResult.For(workitem.TypePrefix).Return(Entity.TaskType);
            SetupResult.For(workitem.Parent).Return(parent);
            SetupResult.For(parent.TypePrefix).Return(Entity.StoryType);            

            mockRepository.ReplayAll();

            var effortTracking = new EffortTracking(connectorMock);
            effortTracking.Init();
            Assert.IsFalse(effortTracking.AreEffortTrackingPropertiesReadOnly(workitem));

            mockRepository.VerifyAll();
        }
    }
}