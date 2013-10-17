using System.Collections.Generic;
using VersionOne.VisualStudio.DataLayer.Entities;
using VersionOne.VisualStudio.DataLayer.Logging;
using VersionOne.VisualStudio.DataLayer.Settings;

namespace VersionOne.VisualStudio.DataLayer {
    public interface IDataLayer {
        /// <summary>
        /// Logger factory used to resolve Data Layer logger
        /// </summary>
        ILoggerFactory LoggerFactory { get; set; }

        /// <summary>
        /// Validate VersionOne connection
        /// </summary>
        /// <param name="settings">VersionOne connection settings: server URL, login, password, etc.</param>
        void CheckConnection(VersionOneSettings settings);

        /// <summary>
        /// Connect to VersionOne server.
        /// </summary>
        bool Connect(VersionOneSettings settings);

        /// <summary>
        /// Check if V1 connection is established.
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// Commit all pending changes to V1.
        /// </summary>
        void CommitChanges(IAssetCache assetCache);

        /// <summary>
        /// Using this property makes query to V1 server. Use CurrentProject if it is posible.
        /// </summary>
        string CurrentProjectId { get; set;}

        /// <summary>
        /// Workitem representing current project selected in Projects view.
        /// </summary>
        Project CurrentProject { get; set;}

        /// <summary>
        /// Gets tree of projects from V1 server. In most cases there is only one (Scope:0) root project, but may be more.  
        /// </summary>
        /// <returns>
        ///     List of root Projects. Child projects can be accessed through Project.Children property.
        /// </returns>
        IList<Project> GetProjectTree();

        /// <summary>
        /// Create empty asset cache that stores workitems between data requests.
        /// </summary>
        IAssetCache CreateAssetCache();

        /// <summary>
        /// Gets lists of stories, defects, tasks and tests from V1 server.   
        /// </summary>
        /// <returns>
        ///     List of Stories and Defects. Tasks and tests can be accessed through Children property of stories and defects.
        /// </returns>
        void GetWorkitems(IAssetCache assetCache);

        /// <summary>
        /// Get collection of available values of list property.
        /// </summary>
        /// <exception cref="KeyNotFoundException">If no property found.</exception>
        PropertyValues GetListPropertyValues(string propertyName);

        /// <summary>
        /// Update collection of available values of list property.
        /// </summary>
        void UpdateListPropertyValues();

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
        /// <param name="entityContainer">Entity container</param>
        /// <returns>Newly created workitem</returns>
        Workitem CreateWorkitem(string workitemType, Workitem parent, IEntityContainer entityContainer);

        /// <summary>
        /// Non-existing project token.
        /// </summary>
        string NullProjectToken { get; }

        /// <summary>
        /// Effort tracking information.
        /// </summary>
        IEffortTracking EffortTracking { get; }
    }
}