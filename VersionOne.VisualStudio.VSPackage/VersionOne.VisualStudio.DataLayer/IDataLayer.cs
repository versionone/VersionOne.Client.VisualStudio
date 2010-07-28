using System.Collections.Generic;

namespace VersionOne.VisualStudio.DataLayer {
    public interface IDataLayer {
        /// <summary>
        /// Validate VersionOne connection
        /// </summary>
        /// <param name="path">VersionOne server URL</param>
        /// <param name="userName">User login</param>
        /// <param name="password">User password</param>
        /// <param name="integrated">Use Windows integrated authentication</param>
        void CheckConnection(string path, string userName, string password, bool integrated);

        /// <summary>
        /// Connect to VersionOne server.
        /// </summary>
        bool Connect(string path, string userName, string password, bool integrated);

        /// <summary>
        /// Check if V1 connection is established.
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// Commit all pending changes to V1.
        /// </summary>
        void CommitChanges();

        /// <summary>
        /// Using this property makes query to V1 server. Use CurrentProject if it is posible.
        /// </summary>
        string CurrentProjectId { get; set;}

        /// <summary>
        /// Workitem representing current project selected in Projects view.
        /// </summary>
        Project CurrentProject { get; set;}

        /// <summary>
        /// Effort tracking enabled state.
        /// </summary>
        bool TrackEffort { get; }

        /// <summary>
        /// Current Story effort tracking level.
        /// </summary>
        EffortTrackingLevel StoryTrackingLevel { get; }

        /// <summary>
        /// Current Defect effort tracking level.
        /// </summary>
        EffortTrackingLevel DefectTrackingLevel { get; }

        /// <summary>
        /// Gets tree of projects from V1 server. In most cases there is only one (Scope:0) root project, but may be more.  
        /// </summary>
        /// <returns>
        ///     List of root Projects. Child projects can be accessed through Project.Children property.
        /// </returns>
        IList<Project> GetProjectTree();

        /// <summary>
        /// Gets lists of stories, defects, tasks and tests from V1 server.   
        /// </summary>
        /// <returns>
        ///     List of Stories and Defects. Tasks and tests can be accessed through Children property of stories and defects.
        /// </returns>
        List<Workitem> GetWorkitems();

        /// <summary>
        /// Get collection of available values of list property.
        /// </summary>
        /// <exception cref="KeyNotFoundException">If no property found.</exception>
        PropertyValues GetListPropertyValues(string propertyName);

        /// <summary>
        /// Flag managing filtering tasks by owner, persistance and modified state.
        /// </summary>
        bool ShowAllTasks { get; set;}

        /// <summary>
        /// Sets API version used in V1 server communications. Must be set before Connect() call;
        /// </summary>
        string ApiVersion { get; set; }

        /// <summary>
        /// Resolve key using VersionOne Localizer.
        /// </summary>
        /// <param name="key">Key to resolve, ex. column name</param>
        /// <returns>Localized key value.</returns>
        string LocalizerResolve(string key);

        /// <summary>
        /// Try to resolve key using VersionOne Localizer.
        /// </summary>
        /// <see cref="LocalizerResolve(string)"/>
        bool TryLocalizerResolve(string key, out string result);

        /// <summary>
        /// Recreate connection to VersionOne
        /// </summary>
        void Reconnect();

        /// <summary>
        /// Adds attribute to be queried for workitems of specified type. 
        /// All attributes must be added before Connect() method call.
        /// </summary>
        /// <param name="attr">Name of attribute.</param>
        /// <param name="prefix">Type of Workitem specified by prefix, ex. Workitem.Story.</param>
        /// <param name="isList">Specifies whether property is list property.</param>
        void AddProperty(string attr, string prefix, bool isList);

        /// <summary>
        /// Create new Workitem and store it.
        /// </summary>
        /// <param name="workitemType">Workitem type token, ex. Entity.TaskPrefix</param>
        /// <param name="parent">Parent workitem, if exists</param>
        /// <returns>Newly created workitem</returns>
        Workitem CreateWorkitem(string workitemType, Workitem parent);
    }
}
