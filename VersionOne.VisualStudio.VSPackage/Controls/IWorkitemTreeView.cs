using System.Windows.Forms;
using VersionOne.VisualStudio.DataLayer.Entities;
using VersionOne.VisualStudio.VSPackage.Controllers;
using VersionOne.VisualStudio.VSPackage.Descriptors;

namespace VersionOne.VisualStudio.VSPackage.Controls {
    public interface IWorkitemTreeView : IWaitCursorProvider {
        WorkitemTreeController Controller { get; set; }
        
        string Title { get; set; }
        WorkitemDescriptor CurrentWorkitemDescriptor { get; }
        StoryTreeModel Model { get; set; }

        void Refresh();
        void RefreshProperties();
        void ResetPropertyView();
        void SetSelection();
        void ExpandCurrentNode();
        void ReconfigureTreeColumns();
        bool CheckSettingsAreValid();
        void SelectWorkitem(Workitem item);

        void ShowValidationInformationDialog(string validationResult);

        bool AddTaskCommandEnabled { get; set; }
        bool AddDefectCommandEnabled { get; set; }
        bool AddTestCommandEnabled { get; set; }

        void ShowErrorMessage(string message);
    }
}