using System.Collections.Generic;
using VersionOne.VisualStudio.DataLayer.Entities;
using VersionOne.VisualStudio.VSPackage.Controllers;

namespace VersionOne.VisualStudio.VSPackage.Controls {
    public interface IProjectTreeView : IWaitCursorProvider {
        void RefreshProperties();
        void ResetPropertyView();
        void UpdateData();

        IEnumerable<Project> Projects { get; set; }
        void CompleteProjectsRequest();
        
        ProjectTreeController Controller { get; set; }
    }
}