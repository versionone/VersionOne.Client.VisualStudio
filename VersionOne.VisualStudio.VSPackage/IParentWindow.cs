using System;

namespace VersionOne.VisualStudio.VSPackage {
    public interface IParentWindow {
        object GetVsService(Type serviceType);
        string Caption { get; set; }
    }
}