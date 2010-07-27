// Guids.cs
// MUST match guids.h
using System;

namespace VersionOne.VisualStudio.VSPackage {
    static class GuidList {
        public const string guidVersionOne_VisualStudio_VSPackagePkgString = "9423c8f0-ca04-432e-af2d-89aa9bc5ebe5";
        public const string guidVersionOne_VisualStudio_VSPackageCmdSetString = "2c4566ea-865a-453f-b55e-32b199bbace4";
        public const string guidToolWindowPersistanceString = "140fa325-23a8-482b-9efd-9e4297081947";

        public static readonly Guid guidVersionOne_VisualStudio_VSPackageCmdSet = new Guid(guidVersionOne_VisualStudio_VSPackageCmdSetString);
    }
}