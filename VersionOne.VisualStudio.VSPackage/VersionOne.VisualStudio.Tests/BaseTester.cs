using NUnit.Framework;
using Ninject;
using Rhino.Mocks;
using VersionOne.VisualStudio.DataLayer;

namespace VersionOne.VisualStudio.Tests {
    public class BaseTester {
        protected readonly IKernel Container = new StandardKernel();
        protected readonly MockRepository MockRepository = new MockRepository();

        [TestFixtureSetUp]
        public void TestFixtureSetUp() {
            ServiceLocator.Instance.SetContainer(Container);
            Container.Bind<IDataLayerInternal>().ToConstant(MockRepository.Stub<IDataLayerInternal>());
        }
    }
}