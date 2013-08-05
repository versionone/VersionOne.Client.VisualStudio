using NUnit.Framework;
using Rhino.Mocks;
using VersionOne.SDK.APIClient;
using VersionOne.VisualStudio.DataLayer;
using VersionOne.VisualStudio.DataLayer.Entities;

namespace VersionOne.VisualStudio.Tests {
    [TestFixture]
    public class EffortTrackingTester : BaseTester {
        private IVersionOneConnector connectorMock;
        private IV1Configuration configuration;

        [SetUp]
        public void SetUp() {
            connectorMock = MockRepository.StrictMock<IVersionOneConnector>();
            configuration = MockRepository.StrictMock<IV1Configuration>();
        }

        [Test]
        public void EffortTrackingInit() {
            InitEffortExpectations(TrackingLevel.Off, TrackingLevel.On);

            MockRepository.ReplayAll();
            var effortTracking = new EffortTracking(connectorMock);
            effortTracking.Init();
            MockRepository.VerifyAll();

            Assert.IsTrue(effortTracking.TrackEffort);
            Assert.AreEqual(EffortTrackingLevel.PrimaryWorkitem, effortTracking.DefectTrackingLevel);
            Assert.AreEqual(EffortTrackingLevel.SecondaryWorkitem, effortTracking.StoryTrackingLevel);
        }

        [Test]
        public void RefreshConfig() {
            using (MockRepository.Ordered()) {
                InitEffortExpectations(TrackingLevel.Off, TrackingLevel.On);

                Expect.Call(connectorMock.LoadV1Configuration()).Return(configuration);
                Expect.Call(configuration.EffortTracking).Return(false);
                Expect.Call(configuration.StoryTrackingLevel).Return(TrackingLevel.On);
                Expect.Call(configuration.DefectTrackingLevel).Return(TrackingLevel.Off);
            }

            MockRepository.ReplayAll();
            var effortTracking = new EffortTracking(connectorMock);
            effortTracking.Init();
            effortTracking.Refresh();
            MockRepository.VerifyAll();

            Assert.IsFalse(effortTracking.TrackEffort);
            Assert.AreEqual(EffortTrackingLevel.SecondaryWorkitem, effortTracking.DefectTrackingLevel);
            Assert.AreEqual(EffortTrackingLevel.PrimaryWorkitem, effortTracking.StoryTrackingLevel);
        }

        [Test]
        public void ValidateDefectEffortTrackingProperty() {
            var workitem = MockRepository.StrictMock<TestWorkitem>(null, null, null);

            InitEffortExpectations(TrackingLevel.Off, TrackingLevel.On);
            SetupResult.For(workitem.TypePrefix).Return(Entity.DefectType);

            MockRepository.ReplayAll();

            var effortTracking = new EffortTracking(connectorMock);
            effortTracking.Init();
            Assert.IsFalse(effortTracking.AreEffortTrackingPropertiesReadOnly(workitem));

            MockRepository.VerifyAll();
        }

        [Test]
        public void ValidateDefectEffortTrackingPropertyReadOnly() {
            var workitem = MockRepository.StrictMock<TestWorkitem>(null, null, null);

            InitEffortExpectations(TrackingLevel.On, TrackingLevel.Off);
            SetupResult.For(workitem.TypePrefix).Return(Entity.DefectType);

            MockRepository.ReplayAll();

            var effortTracking = new EffortTracking(connectorMock);
            effortTracking.Init();
            Assert.IsTrue(effortTracking.AreEffortTrackingPropertiesReadOnly(workitem));

            MockRepository.VerifyAll();
        }

        [Test]
        public void ValidateStoryEffortTrackingProperty() {
            var workitem = MockRepository.StrictMock<TestWorkitem>(null, null, null);

            InitEffortExpectations(TrackingLevel.On, TrackingLevel.Off);
            SetupResult.For(workitem.TypePrefix).Return(Entity.StoryType);

            MockRepository.ReplayAll();

            var effortTracking = new EffortTracking(connectorMock);
            effortTracking.Init();
            Assert.IsFalse(effortTracking.AreEffortTrackingPropertiesReadOnly(workitem));

            MockRepository.VerifyAll();
        }

        [Test]
        public void ValidateStoryEffortTrackingPropertyReadOnly() {
            var workitem = MockRepository.StrictMock<TestWorkitem>(null, null, null);

            InitEffortExpectations(TrackingLevel.Off, TrackingLevel.On);
            SetupResult.For(workitem.TypePrefix).Return(Entity.StoryType);

            MockRepository.ReplayAll();

            var effortTracking = new EffortTracking(connectorMock);
            effortTracking.Init();
            Assert.IsTrue(effortTracking.AreEffortTrackingPropertiesReadOnly(workitem));

            MockRepository.VerifyAll();
        }

        [Test]
        public void ValidateTaskEffortTrackingProperty() {
            var parent = MockRepository.StrictMock<TestWorkitem>(null, null, null);
            var workitem = MockRepository.StrictMock<TestWorkitem>(null, null, null);

            InitEffortExpectations(TrackingLevel.Off, TrackingLevel.On);
            SetupResult.For(workitem.TypePrefix).Return(Entity.TaskType);
            SetupResult.For(workitem.Parent).Return(parent);
            SetupResult.For(parent.TypePrefix).Return(Entity.StoryType);            

            MockRepository.ReplayAll();

            var effortTracking = new EffortTracking(connectorMock);
            effortTracking.Init();
            Assert.IsFalse(effortTracking.AreEffortTrackingPropertiesReadOnly(workitem));
            Assert.IsTrue(effortTracking.AreEffortTrackingPropertiesReadOnly(parent));

            MockRepository.VerifyAll();
        }

        [Test]
        public void ValidateTestEffortTrackingProperty() {
            var parent = MockRepository.StrictMock<TestWorkitem>(null, null, null);
            var workitem = MockRepository.StrictMock<TestWorkitem>(null, null, null);

            InitEffortExpectations(TrackingLevel.Off, TrackingLevel.Mix);
            SetupResult.For(workitem.TypePrefix).Return(Entity.TestType);
            SetupResult.For(workitem.Parent).Return(parent);
            SetupResult.For(parent.TypePrefix).Return(Entity.DefectType);

            MockRepository.ReplayAll();

            var effortTracking = new EffortTracking(connectorMock);
            effortTracking.Init();
            Assert.IsFalse(effortTracking.AreEffortTrackingPropertiesReadOnly(workitem));
            Assert.IsFalse(effortTracking.AreEffortTrackingPropertiesReadOnly(parent));

            MockRepository.VerifyAll();
        }

        private void InitEffortExpectations(TrackingLevel storyTrackingLevel, TrackingLevel defectTrackingLevel) {
            Expect.Call(connectorMock.V1Configuration).Return(configuration);
            Expect.Call(configuration.EffortTracking).Return(true);
            Expect.Call(configuration.StoryTrackingLevel).Return(storyTrackingLevel);
            Expect.Call(configuration.DefectTrackingLevel).Return(defectTrackingLevel);
        }
    }
}