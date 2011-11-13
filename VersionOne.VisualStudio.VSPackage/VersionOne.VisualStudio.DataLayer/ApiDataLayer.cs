using System;
using System.Collections.Generic;
using System.Linq;
using VersionOne.SDK.APIClient;
using System.Net;
using VersionOne.VisualStudio.DataLayer.Entities;
using VersionOne.VisualStudio.DataLayer.Settings;

namespace VersionOne.VisualStudio.DataLayer {
    public class ApiDataLayer : IDataLayer {
        #region Private fields

        private readonly VersionOneConnector connector = new VersionOneConnector();

        private static IDataLayer dataLayer;
        private readonly static LinkedList<AttributeInfo> AttributesToQuery = new LinkedList<AttributeInfo>();

        private RequiredFieldsValidator requiredFieldsValidator;
        internal Dictionary<string, IAssetType> Types;
        internal IAssetType ProjectType;
        internal IAssetType TaskType;
        internal IAssetType TestType;
        internal IAssetType DefectType;
        internal IAssetType StoryType;
        private IAssetType workitemType;
        private IAssetType primaryWorkitemType;
        private IAssetType effortType;

        private Dictionary<string, PropertyValues> listPropertyValues;
        private AssetList allAssets = new AssetList();
        private readonly Dictionary<Asset, double> efforts = new Dictionary<Asset, double>();

        private readonly IList<string> effortTrackingAttributes = new List<string> {
            "DetailEstimate",
            "ToDo",
            "Done",
            "Effort",
            "Actuals",
        };

        #endregion

        internal Oid MemberOid;

        private ApiDataLayer() {
            var prefixes = new[] {
                Entity.TaskPrefix, 
                Entity.DefectPrefix, 
                Entity.StoryPrefix, 
                Entity.TestPrefix
            };

            foreach(var prefix in prefixes) {
                AttributesToQuery.AddLast(new AttributeInfo("CheckQuickClose", prefix, false));
                AttributesToQuery.AddLast(new AttributeInfo("CheckQuickSignup", prefix, false));
            }

            AttributesToQuery.AddLast(new AttributeInfo("Schedule.EarliestActiveTimebox", Entity.ProjectPrefix, false));
        }

        public string ApiVersion {
            get { return connector.ApiVersion; }
            set { connector.ApiVersion = value; }
        }

        public string NullProjectToken {
            get { return Oid.Null.Token; }
        }

        private string currentProjectId;
        public string CurrentProjectId {
            get { return currentProjectId; }
            set {
                currentProjectId = value;
                DropWorkitemCache();
            }
        }

        public Project CurrentProject {
            get {
                if(currentProjectId == null) {
                    currentProjectId = "Scope:0";
                }

                return GetProjectById(currentProjectId);
            }
            set {
                currentProjectId = value.Id;
                DropWorkitemCache();
            }
        }

        public bool ShowAllTasks { get; set; }

        public void CommitChanges() {
            connector.CheckConnection();

            try {
                var validationResult = new Dictionary<Asset, List<RequiredFieldsDto>>();

                var workitems = GetWorkitems();

                foreach(var item in workitems) {
                    if(!ValidateWorkitemAndCommitOnSuccess(item, validationResult)) {
                        continue;
                    }

                    foreach(var child in item.Children) {
                        ValidateWorkitemAndCommitOnSuccess(child, validationResult);
                    }
                }

                if(validationResult.Count > 0) {
                    throw new ValidatorException(requiredFieldsValidator.CreateErrorMessage(validationResult));
                }

            } catch(APIException ex) {
                Logger.Error("Failed to commit changes.", ex);
            }
        }

        private bool ValidateWorkitemAndCommitOnSuccess(Workitem item, IDictionary<Asset, List<RequiredFieldsDto>> validationResults) {
            var itemValidationResult = requiredFieldsValidator.Validate(item.Asset);

            if(itemValidationResult.Count == 0) {
                item.CommitChanges();
                CommitEfforts(item.Asset);
                return true;
            }

            validationResults.Add(item.Asset, itemValidationResult);
            return false;
        }

        private IFilterTerm GetScopeFilter(IAssetType assetType) {
            var terms = new List<FilterTerm>(4);

            var term = new FilterTerm(assetType.GetAttributeDefinition("Scope.AssetState"));
            term.NotEqual(AssetState.Closed);
            terms.Add(term);

            term = new FilterTerm(assetType.GetAttributeDefinition("Scope.ParentMeAndUp"));
            term.Equal(currentProjectId);
            terms.Add(term);

            term = new FilterTerm(assetType.GetAttributeDefinition("Timebox.State.Code"));
            term.Equal("ACTV");
            terms.Add(term);

            term = new FilterTerm(assetType.GetAttributeDefinition("AssetState"));
            term.NotEqual(AssetState.Closed);
            terms.Add(term);

            return new AndFilterTerm(terms.ToArray());
        }

        #region Effort tracking

        public EffortTrackingLevel DefectTrackingLevel { get; private set; }
        public EffortTrackingLevel StoryTrackingLevel { get; private set; }
        public bool TrackEffort { get; private set; }

        private static EffortTrackingLevel TranslateEffortTrackingLevel(TrackingLevel level) {
            switch(level) {
                case TrackingLevel.On:
                    return EffortTrackingLevel.PrimaryWorkitem;
                case TrackingLevel.Off:
                    return EffortTrackingLevel.SecondaryWorkitem;
                case TrackingLevel.Mix:
                    return EffortTrackingLevel.Both;
                default:
                    throw new NotSupportedException("Unknown tracking level");
            }
        }

        internal bool IsEffortTrackingRelated(string attributeName) {
            return effortTrackingAttributes.Contains(attributeName);
        }

        #endregion

        private void AddSelection(Query query, string typePrefix) {
            foreach(var attrInfo in AttributesToQuery) {
                if(attrInfo.Prefix == typePrefix) {
                    try {
                        var def = Types[attrInfo.Prefix].GetAttributeDefinition(attrInfo.Attr);
                        query.Selection.Add(def);
                    } catch(MetaException e) {
                        Logger.Warning("Wrong attribute: " + attrInfo, e);
                    }
                }
            }

            if(requiredFieldsValidator.GetFields(typePrefix) == null) {
                return;
            }

            foreach(var field in requiredFieldsValidator.GetFields(typePrefix)) {
                try {
                    var def = Types[typePrefix].GetAttributeDefinition(field.Name);
                    query.Selection.Add(def);
                } catch(MetaException e) {
                    Logger.Warning("Wrong attribute: " + field.Name, e);
                }
            }
        }

        private Project GetProjectById(string id) {
            if(!connector.IsConnected) {
                return null;
            }

            if(currentProjectId == null) {
                throw new DataLayerException("Current project is not selected");
            }

            var query = new Query(Oid.FromToken(id, connector.MetaModel));
            AddSelection(query, Entity.ProjectPrefix);
            QueryResult result;
            
            try {
                result = connector.Services.Retrieve(query);
            } catch(MetaException ex) {
                connector.IsConnected = false;
                throw new DataLayerException("Unable to get projects", ex);
            } catch(Exception ex) {
                throw new DataLayerException("Unable to get projects", ex);
            }

            return result.TotalAvaliable == 1 ? WorkitemFactory.CreateProject(result.Assets[0], null) : null;
        }

        public void DropWorkitemCache() {
            allAssets = null;
        }

        public List<Workitem> GetWorkitems() {
            connector.CheckConnection();
            
            if(currentProjectId == null) {
                throw new DataLayerException("Current project is not selected");
            }

            if(allAssets == null) {
                try {
                    var parentDef = workitemType.GetAttributeDefinition("Parent");

                    var query = new Query(workitemType, parentDef);
                    AddSelection(query, Entity.TaskPrefix);
                    AddSelection(query, Entity.StoryPrefix);
                    AddSelection(query, Entity.DefectPrefix);
                    AddSelection(query, Entity.TestPrefix);

                    query.Filter = GetScopeFilter(workitemType);

                    query.OrderBy.MajorSort(primaryWorkitemType.DefaultOrderBy, OrderBy.Order.Ascending);
                    query.OrderBy.MinorSort(workitemType.DefaultOrderBy, OrderBy.Order.Ascending);

                    var assetList = connector.Services.Retrieve(query);
                    allAssets = assetList.Assets;

                    var allowedTypeTokens = new List<string> {
                        StoryType.Token, DefectType.Token, TaskType.Token, TestType.Token                                                      
				    };

                    allAssets.RemoveAll(asset => !allowedTypeTokens.Contains(asset.AssetType.Token));

                } catch(MetaException ex) {
                    Logger.Error("Unable to get workitems.", ex);
                } catch(WebException ex) {
                    connector.IsConnected = false;
                    Logger.Error("Unable to get workitems.", ex);
                } catch(Exception ex) {
                    Logger.Error("Unable to get workitems.", ex);
                }
            }

            return allAssets.Where(asset => ShowAllTasks || AssetPassesShowMyTasksFilter(asset))
                            .Select(asset => WorkitemFactory.CreateWorkitem(asset, null)).ToList();
        }

        /// <summary>
        /// Check if asset should be used when Show My Tasks filter is on
        /// </summary>
        /// <param name="asset">Story, Task, Defect or Test</param>
        /// <returns>true if current user is owner of asset, false - otherwise</returns>
        internal bool AssetPassesShowMyTasksFilter(Asset asset) {
            if(asset.HasChanged || asset.Oid.IsNull) {
                return true;
            }

            var definition = workitemType.GetAttributeDefinition(Entity.OwnersProperty);
            var attribute = asset.GetAttribute(definition);
            var owners = attribute.Values;
            
            if(owners.Cast<Oid>().Any(oid => oid == MemberOid)) {
                return true;
            }

            if(asset.Children != null) {
                return asset.Children.Any(AssetPassesShowMyTasksFilter);
            }

            return false;
        }

        public IList<Project> GetProjectTree() {
            try {
                var scopeQuery = new Query(ProjectType, ProjectType.GetAttributeDefinition("Parent"));
                var stateTerm = new FilterTerm(ProjectType.GetAttributeDefinition("AssetState"));
                stateTerm.NotEqual(AssetState.Closed);
                scopeQuery.Filter = stateTerm;
                AddSelection(scopeQuery, Entity.ProjectPrefix);
                var result = connector.Services.Retrieve(scopeQuery);
                
                var roots = result.Assets.Select(asset => WorkitemFactory.CreateProject(asset, null)).ToList();
                return roots;
            } catch(WebException ex) {
                connector.IsConnected = false;
                Logger.Error("Can't get projects list.", ex);
                return null;
            } catch(Exception ex) {
                Logger.Error("Can't get projects list.", ex);
                return null;
            }
        }

        public void CheckConnection(VersionOneSettings settings) {
            connector.CheckConnection(settings);
        }

        public bool Connect(VersionOneSettings settings) {
            connector.IsConnected = false;
            DropWorkitemCache();

            try {
                connector.Connect(settings);

                Types = new Dictionary<string, IAssetType>(5);
                ProjectType = GetAssetType(Entity.ProjectPrefix);
                TaskType = GetAssetType(Entity.TaskPrefix);
                TestType = GetAssetType(Entity.TestPrefix);
                DefectType = GetAssetType(Entity.DefectPrefix);
                StoryType = GetAssetType(Entity.StoryPrefix);
                workitemType = connector.MetaModel.GetAssetType("Workitem");
                primaryWorkitemType = connector.MetaModel.GetAssetType("PrimaryWorkitem");

                TrackEffort = connector.V1Configuration.EffortTracking;
                efforts.Clear();

                if(TrackEffort) {
                    effortType = connector.MetaModel.GetAssetType("Actual");
                }

                StoryTrackingLevel = TranslateEffortTrackingLevel(connector.V1Configuration.StoryTrackingLevel);
                DefectTrackingLevel = TranslateEffortTrackingLevel(connector.V1Configuration.DefectTrackingLevel);

                MemberOid = connector.Services.LoggedIn;
                listPropertyValues = GetListPropertyValues();
                requiredFieldsValidator = new RequiredFieldsValidator(connector.MetaModel, connector.Services, Instance);
                connector.IsConnected = true;

                return true;
            } catch(MetaException ex) {
                Logger.Error("Cannot connect to V1 server.", ex);
                return false;
            } catch(WebException ex) {
                connector.IsConnected = false;
                Logger.Error("Cannot connect to V1 server.", ex);
                return false;
            } catch(Exception ex) {
                Logger.Error("Cannot connect to V1 server.", ex);
                return false;
            }
        }

        // TODO try to find out why SecurityException might occur here
        private IAssetType GetAssetType(string token) {
            var type = connector.MetaModel.GetAssetType(token);
            Types.Add(token, type);
            return type;
        }

        private static string ResolvePropertyKey(string propertyAlias) {
            switch(propertyAlias) {
                case "DefectStatus":
                    return "StoryStatus";
                case "DefectSource":
                    return "StorySource";
                case "ScopeBuildProjects":
                    return "BuildProject";
                case "TaskOwners":
                case "StoryOwners":
                case "DefectOwners":
                case "TestOwners":
                    return "Member";
            }

            return propertyAlias;
        }

        private Dictionary<string, PropertyValues> GetListPropertyValues() {
            var res = new Dictionary<string, PropertyValues>(AttributesToQuery.Count);
            
            foreach(var attrInfo in AttributesToQuery) {
                if(!attrInfo.IsList) {
                    continue;
                }

                var propertyAlias = attrInfo.Prefix + attrInfo.Attr;

                if(res.ContainsKey(propertyAlias)) {
                    continue;
                }

                var propertyName = ResolvePropertyKey(propertyAlias);

                PropertyValues values;
                if(res.ContainsKey(propertyName)) {
                    values = res[propertyName];
                } else {
                    values = QueryPropertyValues(propertyName);
                    res.Add(propertyName, values);
                }

                if(!res.ContainsKey(propertyAlias)) {
                    res.Add(propertyAlias, values);
                }
            }

            return res;
        }

        private PropertyValues QueryPropertyValues(string propertyName) {
            var res = new PropertyValues();
            var assetType = connector.MetaModel.GetAssetType(propertyName);
            var nameDef = assetType.GetAttributeDefinition(Entity.NameProperty);
            IAttributeDefinition inactiveDef;

            var query = new Query(assetType);
            query.Selection.Add(nameDef);
            
            if(assetType.TryGetAttributeDefinition("Inactive", out inactiveDef)) {
                var filter = new FilterTerm(inactiveDef);
                filter.Equal("False");
                query.Filter = filter;
            }

            query.OrderBy.MajorSort(assetType.DefaultOrderBy, OrderBy.Order.Ascending);

            res.Add(new ValueId());
            
            foreach(var asset in connector.Services.Retrieve(query).Assets) {
                var name = asset.GetAttribute(nameDef).Value as string;
                res.Add(new ValueId(asset.Oid, name));
            }

            return res;
        }

        #region Localizer

        public string LocalizerResolve(string key) {
            try {
                return connector.Localizer.Resolve(key);
            } catch(Exception ex) {
                throw new DataLayerException("Failed to resolve key.", ex);
            }
        }

        public bool TryLocalizerResolve(string key, out string result) {
            result = null;

            try {
                if(connector.Localizer != null) {
                    result = connector.Localizer.Resolve(key);
                    return true;
                }
            } catch(V1Exception) { }

            return false;
        }

        #endregion

        public static IDataLayer Instance {
            get { return dataLayer ?? (dataLayer = new ApiDataLayer()); }
        }

        public bool IsConnected {
            get {
                if(!connector.IsConnected) {
                    try {
                        Reconnect();
                    } catch(DataLayerException) {
                        //Do nothing
                    }
                }

                return connector.IsConnected;
            }
        }

        /// <exception cref="KeyNotFoundException">If there are no values for this property.</exception>
        public PropertyValues GetListPropertyValues(string propertyName) {
            var propertyKey = ResolvePropertyKey(propertyName);
            return listPropertyValues.ContainsKey(propertyName) ? listPropertyValues[propertyKey] : null;
        }

        internal void CommitAsset(Asset asset) {
            try {
                var requiredData = requiredFieldsValidator.Validate(asset);

                if(requiredData.Count > 0) {
                    var message = requiredFieldsValidator.GetMessageOfUnfilledFieldsList(requiredData, Environment.NewLine, ", ");
                    throw new ValidatorException(message);
                }
            } catch(APIException ex) {
                Logger.Error("Cannot validate required fields.", ex);
            }

            connector.Services.Save(asset);
            CommitEfforts(asset);
        }

        internal void RevertAsset(Asset asset) {
            asset.RejectChanges();
            efforts.Remove(asset);
        }

        /// <summary>
        /// Reconnect with settings, used in last Connect() call.
        /// </summary>
        public void Reconnect() {
            Connect(connector.VersionOneSettings);
            // TODO figure out why using the next line instead of Connect() call breaks. @see VersionOne.SDK.APIClient.Services.TreeAssetListByAttribute(..) - NullReferenceEx.
            // connector.Reconnect();
        }

        internal double? GetEffort(Asset asset) {
            double res;
            if(efforts.TryGetValue(asset, out res))
                return res;
            return null;
        }

        internal void AddEffort(Asset asset, double newValue) {
            if(efforts.ContainsKey(asset)) {
                if(newValue == 0) {
                    efforts.Remove(asset);
                } else {
                    efforts[asset] = newValue;
                }
            } else {
                if(newValue != 0) {
                    efforts.Add(asset, newValue);
                }
            }
        }

        /// <summary>
        /// Commit efforts.
        /// </summary>
        /// <param name="exactAsset">Specific asset to commit related efforts. If all efforts are to be committed, pass null.</param>
        // TODO refactor this
        private void CommitEfforts(Asset exactAsset) {
            var toRemove = new List<Asset>();

            foreach(var pair in efforts) {
                if(exactAsset != null && !exactAsset.Equals(pair.Key)) {
                    continue;
                }

                var effort = connector.Services.New(effortType, pair.Key.Oid);
                effort.SetAttributeValue(effortType.GetAttributeDefinition("Value"), pair.Value);
                effort.SetAttributeValue(effortType.GetAttributeDefinition("Date"), DateTime.Now);
                connector.Services.Save(effort);

                toRemove.Add(pair.Key);
            }

            foreach(var asset in toRemove) {
                efforts.Remove(asset);
            }
        }

        public void AddProperty(string attr, string prefix, bool isList) {
            AttributesToQuery.AddLast(new AttributeInfo(attr, prefix, isList));
        }

        internal void ExecuteOperation(Asset asset, IOperation operation) {
            connector.Services.ExecuteOperation(operation, asset.Oid);
        }

        internal void CleanupWorkitem(Workitem item) {
            if(item.Parent != null && allAssets.Contains(item.Parent.Asset)) {
                item.Parent.Asset.Children.Remove(item.Asset);
            }

            allAssets.Remove(item.Asset);

            efforts.Remove(item.Asset);
            foreach(var child in item.Asset.Children) {
                efforts.Remove(child);
            }
        }

        /// <summary>
        /// Refreshes data for Asset wrapped by specified Workitem.
        /// </summary>
        // TODO refactor
        internal void RefreshAsset(Workitem workitem) {
            try {
                var stateDef = workitem.Asset.AssetType.GetAttributeDefinition("AssetState");
                
                var query = new Query(workitem.Asset.Oid.Momentless, false);
                AddSelection(query, workitem.TypePrefix);
                query.Selection.Add(stateDef);
                
                var newAssets = connector.Services.Retrieve(query);

                var containedIn = workitem.Parent == null ? allAssets : workitem.Parent.Asset.Children;

                if(newAssets.TotalAvaliable != 1) {
                    containedIn.Remove(workitem.Asset);
                    return;
                }

                var newAsset = newAssets.Assets[0];
                var newAssetState = (AssetState)newAsset.GetAttribute(stateDef).Value;
                
                if(newAssetState == AssetState.Closed) {
                    containedIn.Remove(workitem.Asset);
                    return;
                }

                containedIn[containedIn.IndexOf(workitem.Asset)] = newAsset;
                newAsset.Children.AddRange(workitem.Asset.Children);
            } catch(MetaException ex) {
                Logger.Error("Unable to get workitems.", ex);
            } catch(WebException ex) {
                connector.IsConnected = false;
                Logger.Error("Unable to get workitems.", ex);
            } catch(Exception ex) {
                Logger.Error("Unable to get workitems.", ex);
            }
        }

        public Workitem CreateWorkitem(string assetType, Workitem parent) {
            var assetFactory = new AssetFactory(this, CurrentProject, AttributesToQuery);
            var item = WorkitemFactory.CreateWorkitem(assetFactory, assetType, parent);

            if(item.IsPrimary) {
                allAssets.Add(item.Asset);
            }

            return item;
        }
    }
}