using Microsoft.VsSDK.UnitTestLibrary;
using Microsoft.VisualStudio.Shell.Interop;
using VersionOne.VisualStudio.VSPackage;
using NUnit.Framework;

namespace VersionOne.VisualStudio.Tests {
    [TestFixture]
    [Ignore("There are issues with these tests, and they are not really so useful")]
    public class VSPackageTester {
        [Test]
        public void CreateInstance() {
            var package = new V1Tracker();
            Assert.IsNotNull(package);
        }

        [Test]
        public void SetSite() {
            // Create the package
            var package = new V1Tracker() as IVsPackage;
            Assert.IsNotNull(package, "The object does not implement IVsPackage");

            // Create a basic service provider
            var serviceProvider = OleServiceProvider.CreateOleServiceProviderWithBasicServices();

            // Site the package
            Assert.AreEqual(0, package.SetSite(serviceProvider), "SetSite did not return S_OK");

            // Unsite the package
            Assert.AreEqual(0, package.SetSite(null), "SetSite(null) did not return S_OK");
        }
    }
}