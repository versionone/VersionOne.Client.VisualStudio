// Guids.cs
// MUST match guids.h
using System;

namespace VersionOne.VisualStudio.VSPackage {
    static class GuidList {
        public const string guidVersionOnTrackerPkgString = "9423c8f0-ca04-432e-af2d-89aa9bc5ebe5";
        public const string guidVersionOneTrackerCmdSetString = "2c4566ea-865a-453f-b55e-32b199bbace4";
        public const string guidToolWindowPersistanceString = "140fa325-23a8-482b-9efd-9e4297081947";
        public const string guidTaskWindowPersistanceString = "6b9496c7-4fbe-4807-bd6b-ccfe96d183c3";
        public const string guidProjectWindowPersistanceString = "DE3F3B5C-86CD-4f59-A3F1-9E8BBD3F0D2C";

        public static readonly Guid guidVersionOneTrackerCmdSet = new Guid(guidVersionOneTrackerCmdSetString);
    }
}