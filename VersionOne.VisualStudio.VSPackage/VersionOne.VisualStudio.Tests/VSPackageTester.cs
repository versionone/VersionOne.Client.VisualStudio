using System;
using System.Collections;
using System.Text;
using System.Reflection;
using Microsoft.VsSDK.UnitTestLibrary;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VersionOne.VisualStudio.VSPackage;

namespace VersionOne.VisualStudio.Tests {
    // TODO port this class to NUnit and remove all MSTest references
    [TestClass]
    public class VSPackageTester {
        [TestMethod]
        public void CreateInstance() {
            V1Tracker package = new V1Tracker();
        }

        [TestMethod]
        public void IsIVsPackage() {
            V1Tracker package = new V1Tracker();
            Assert.IsNotNull(package as IVsPackage, "The object does not implement IVsPackage");
        }

        [TestMethod]
        public void SetSite() {
            // Create the package
            IVsPackage package = new V1Tracker() as IVsPackage;
            Assert.IsNotNull(package, "The object does not implement IVsPackage");

            // Create a basic service provider
            OleServiceProvider serviceProvider = OleServiceProvider.CreateOleServiceProviderWithBasicServices();

            // Site the package
            Assert.AreEqual(0, package.SetSite(serviceProvider), "SetSite did not return S_OK");

            // Unsite the package
            Assert.AreEqual(0, package.SetSite(null), "SetSite(null) did not return S_OK");
        }
    }
}