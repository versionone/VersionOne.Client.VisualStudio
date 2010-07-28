using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using VersionOne.SDK.APIClient;
using System.Net;
using Attribute = VersionOne.SDK.APIClient.Attribute;

namespace VersionOne.VisualStudio.DataLayer {
    public class ApiDataLayer : IDataLayer {
        #region Private fields

        private readonly VersionOneConnector connector = new VersionOneConnector();

        private static IDataLayer dataLayer;
        private readonly static LinkedList<AttributeInfo> attributesToQuery = new LinkedList<AttributeInfo>();

		private RequiredFieldsValidator requiredFieldsValidator;
        internal Dictionary<string, IAssetType> Types;
        internal IAssetType ProjectType;
        internal IAssetType TaskType;
        internal IAssetType TestType;
        internal IAssetType DefectType;
        internal IAssetType StoryType;
        private IAssetType WorkitemType;
        private IAssetType PrimaryWorkitemType;
        private IAssetType EffortType;

        private bool trackEffort;
        private EffortTrackingLevel defectTrackingLevel;
        private EffortTrackingLevel storyTrackingLevel;

        private Dictionary<string, PropertyValues> listPropertyValues;
        private AssetList AllAssets;
		private readonly Dictionary<Asset, double> efforts = new Dictionary<Asset, double>();

        private static readonly string[] effortTrackingRelatedAttributes = new string[] {
            "DetailEstimate",
            "ToDo",
            "Done",
            "Effort",
            "Actuals",
        };
        private readonly IList<string> effortTrackingAttributesList = new List<string>(effortTrackingRelatedAttributes);

        #endregion

        internal Oid MemberOid;

        private ApiDataLayer() {
            string[] prefixes = new string[] {
                Entity.TaskPrefix, 
                Entity.DefectPrefix, 
                Entity.StoryPrefix, 
                Entity.TestPrefix
            };

            foreach (string prefix in prefixes) {
                attributesToQuery.AddLast(new AttributeInfo("CheckQuickClose", prefix, false));
                attributesToQuery.AddLast(new AttributeInfo("CheckQuickSignup", prefix, false));
            }

            attributesToQuery.AddLast(new AttributeInfo("Schedule.EarliestActiveTimebox", Entity.ProjectPrefix, false));
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
				AllAssets = null;
            }
        }

        public Project CurrentProject {
            get {
				if (currentProjectId == null) {
					currentProjectId = "Scope:0";
				}
            	return GetProjectById(currentProjectId);
            }
            set { 
				currentProjectId = value.Id;
				AllAssets = null;
			}
        }

        private bool showAllTasks;
        public bool ShowAllTasks {
            get { return showAllTasks; }
            set { showAllTasks = value; }
        }

        public void CommitChanges() {
			connector.CheckConnection();

            try {
                Dictionary<Asset, List<RequiredFieldsDto>> validationResult = new Dictionary<Asset, List<RequiredFieldsDto>>();

                ICollection<Workitem> workitems = GetWorkitems();
                foreach (Workitem item in workitems) {
                    if (ValidateWorkitemAndCommitOnSuccess(item, validationResult)) {
                        foreach (Workitem child in item.Children) {
                            ValidateWorkitemAndCommitOnSuccess(child, validationResult);
                        }
                    }
                }

                if(validationResult.Count > 0) {
                    throw new ValidatorException(requiredFieldsValidator.CreateErrorMessage(validationResult));
                }

            } catch (APIException ex) {
                throw Warning("Failed to commit changes.", ex);
            }
        }

        private bool ValidateWorkitemAndCommitOnSuccess(Workitem item, IDictionary<Asset, List<RequiredFieldsDto>> validationResults) {
            List<RequiredFieldsDto> itemValidationResult = requiredFieldsValidator.Validate(item.Asset);

            if (itemValidationResult.Count == 0) {
                item.CommitChanges();
                CommitEfforts(item.Asset);
                return true;
            }
            
            validationResults.Add(item.Asset, itemValidationResult);
            return false;
        }

        private IFilterTerm GetScopeFilter(IAssetType assetType) {
            List<FilterTerm> terms = new List<FilterTerm>(4);
            
            FilterTerm term = new FilterTerm(assetType.GetAttributeDefinition("Scope.AssetState"));
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
        
        public bool TrackEffort {
            get {
                return trackEffort;
            }
        }

        private static EffortTrackingLevel TranslateEffortTrackingLevel(TrackingLevel level) {
            switch (level) {
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
            return effortTrackingAttributesList.Contains(attributeName);
        }

        public EffortTrackingLevel DefectTrackingLevel {
            get { return defectTrackingLevel; }
        }

        public EffortTrackingLevel StoryTrackingLevel {
            get { return storyTrackingLevel; }
        }

        #endregion

        private void AddSelection(Query query, string typePrefix) {
            foreach (AttributeInfo attrInfo in attributesToQuery) {
				if (attrInfo.Prefix == typePrefix) {
                    try {
                    	IAttributeDefinition def = Types[attrInfo.Prefix].GetAttributeDefinition(attrInfo.Attr);
						query.Selection.Add(def);
                    } catch (MetaException e) {
                        Warning("Wrong attribute: " + attrInfo, e);
                    }
                }
            }

			if (requiredFieldsValidator.GetFields(typePrefix) == null) {
				return;
			}

			foreach (RequiredFieldsDto field in requiredFieldsValidator.GetFields(typePrefix)) {
				try {
					IAttributeDefinition def = Types[typePrefix].GetAttributeDefinition(field.Name);
    				query.Selection.Add(def);
				} catch (MetaException e) {
					Warning("Wrong attribute: " + field.Name, e);
				}
			}
		}

        private Project GetProjectById(string id) {
			if (!connector.IsConnected) {
				return null;
			}
			if (currentProjectId == null) {
				throw new DataLayerException("Current project is not selected");
			}
            
            Query query = new Query(Oid.FromToken(id, connector.MetaModel));
            AddSelection(query, Entity.ProjectPrefix);
        	QueryResult result;
			try{
				result = connector.Services.Retrieve(query);
			} catch(MetaException ex){
				connector.IsConnected = false;				
				throw new DataLayerException("Unable to get projects", ex);
			} catch(Exception ex) {
				throw new DataLayerException("Unable to get projects", ex);
			}
            
            if (result.TotalAvaliable == 1) {
                return WorkitemFactory.Instance.CreateProject(result.Assets[0], null);
            }
            return null;
        }

        public List<Workitem> GetWorkitems() {
            connector.CheckConnection();
			if (currentProjectId == null) {
				throw new DataLayerException("Current project is not selected");
			}

			if (AllAssets == null) {
				try {
					IAttributeDefinition parentDef = WorkitemType.GetAttributeDefinition("Parent");

					Query query = new Query(WorkitemType, parentDef);
                    AddSelection(query, Entity.TaskPrefix);
                    AddSelection(query, Entity.StoryPrefix);
                    AddSelection(query, Entity.DefectPrefix);
                    AddSelection(query, Entity.TestPrefix);

                    query.Filter = GetScopeFilter(WorkitemType);

					query.OrderBy.MajorSort(PrimaryWorkitemType.DefaultOrderBy, OrderBy.Order.Ascending);
					query.OrderBy.MinorSort(WorkitemType.DefaultOrderBy, OrderBy.Order.Ascending);

					QueryResult assetList = connector.Services.Retrieve(query);
					AllAssets = assetList.Assets;

                    List<string> allowedTypeTokens = new List<string>( new string[] {
                                  StoryType.Token, DefectType.Token, TaskType.Token, TestType.Token,                                                      
				    });

				    AllAssets.RemoveAll(delegate(Asset asset) {
				                            return !allowedTypeTokens.Contains(asset.AssetType.Token);
				                        });

				} catch (MetaException ex) {
                    throw Warning("Unable to get workitems.", ex);
				} catch (WebException ex) {
					connector.IsConnected = false;
                    throw Warning("Unable to get workitems.", ex);
				} catch (Exception ex) {
					throw Warning("Unable to get workitems.", ex);
				}
			}

			List<Workitem> res = new List<Workitem>(AllAssets.Count);
			foreach (Asset asset in AllAssets) {
				if (ShowAllTasks || AssetPassesShowMyTasksFilter(asset)) {
					res.Add(WorkitemFactory.Instance.CreateWorkitem(asset, null));
				}
			}
			return res;
        }

		/// <summary>
		/// Check if asset should be used when Show My Tasks filter is on
		/// </summary>
		/// <param name="asset">Story, Task, Defect or Test</param>
		/// <returns>true if current user is owner of asset, false - otherwise</returns>
		internal bool AssetPassesShowMyTasksFilter(Asset asset){
            if(asset.HasChanged || asset.Oid.IsNull) {
                return true;
            }

            IAttributeDefinition definition = WorkitemType.GetAttributeDefinition(Entity.OwnersProperty);
			Attribute attribute = asset.GetAttribute(definition);

			IEnumerable owners = attribute.Values;
			foreach (Oid oid in owners) {
				if (oid == MemberOid) {
					return true;
				}
			}
			if (asset.Children != null) {
				foreach (Asset child in asset.Children) {
					if (AssetPassesShowMyTasksFilter(child)) {
						return true;
					}
				}
			}

			return false;
		}

        public IList<Project> GetProjectTree() {
            try {
                Query scopeQuery = new Query(ProjectType, ProjectType.GetAttributeDefinition("Parent"));
                FilterTerm stateTerm = new FilterTerm(ProjectType.GetAttributeDefinition("AssetState"));
                stateTerm.NotEqual(AssetState.Closed);
                scopeQuery.Filter = stateTerm;
                AddSelection(scopeQuery, Entity.ProjectPrefix);
                QueryResult result = connector.Services.Retrieve(scopeQuery);
                IList<Project> roots = new List<Project>(result.Assets.Count);
                foreach (Asset asset in result.Assets) {
                    roots.Add(WorkitemFactory.Instance.CreateProject(asset, null));
                }
                return roots;
            } catch (WebException ex) {
            	connector.IsConnected = false;
                throw Warning("Can't get projects list.", ex);
            } catch (Exception ex) {
                throw Warning("Can't get projects list.", ex);
            }
        }

        public void CheckConnection(string path, string userName, string password, bool integrated) {
            connector.CheckConnection(path, userName, password, integrated);
        }

        public bool Connect(string path, string userName, string password, bool integrated) {
            connector.IsConnected = false;
        	AllAssets = null;
            try {
                connector.Connect(path, userName, password, integrated);

                Types = new Dictionary<string, IAssetType>(5);
                ProjectType = GetAssetType(Entity.ProjectPrefix);
                TaskType = GetAssetType(Entity.TaskPrefix);
                TestType = GetAssetType(Entity.TestPrefix);
                DefectType = GetAssetType(Entity.DefectPrefix);
                StoryType = GetAssetType(Entity.StoryPrefix);
                WorkitemType = connector.MetaModel.GetAssetType("Workitem");
                PrimaryWorkitemType = connector.MetaModel.GetAssetType("PrimaryWorkitem");

                trackEffort = connector.V1Configuration.EffortTracking;
				efforts.Clear();
                if (trackEffort) {
                    EffortType = connector.MetaModel.GetAssetType("Actual");                    
                }

                storyTrackingLevel = TranslateEffortTrackingLevel(connector.V1Configuration.StoryTrackingLevel);
                defectTrackingLevel = TranslateEffortTrackingLevel(connector.V1Configuration.DefectTrackingLevel);

                MemberOid = connector.Services.LoggedIn;
                listPropertyValues =  GetListPropertyValues();
				requiredFieldsValidator = new RequiredFieldsValidator(connector.MetaModel, connector.Services, Instance);
                connector.IsConnected = true;
                return true;
            } catch (MetaException ex) {
                throw Warning("Cannot connect to V1 server.", ex);
            } catch (WebException ex) {
            	connector.IsConnected = false;
                throw Warning("Cannot connect to V1 server.", ex);
            } catch (Exception ex) {
                throw Warning("Cannot connect to V1 server.", ex);
            }
        }

        // TODO try to find out why SecurityException might occur here
        private IAssetType GetAssetType(string token) {
            IAssetType type = connector.MetaModel.GetAssetType(token);
            Types.Add(token, type);
            return type;
        }

        private static string ResolvePropertyKey(string propertyAlias) {
            switch (propertyAlias) {
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
            Dictionary<string, PropertyValues> res = new Dictionary<string, PropertyValues>(attributesToQuery.Count);
            foreach (AttributeInfo attrInfo in attributesToQuery) {
                if (!attrInfo.IsList) {
                    continue;
                }

                string propertyAlias = attrInfo.Prefix + attrInfo.Attr;
                if (!res.ContainsKey(propertyAlias)) {
                    string propertyName = ResolvePropertyKey(propertyAlias);
                    
                    PropertyValues values;
                    if (res.ContainsKey(propertyName)) {
                        values = res[propertyName];
                    } else {
                        values = QueryPropertyValues(propertyName);
                        res.Add(propertyName, values);
                    }
                    
                    if (!res.ContainsKey(propertyAlias)) {
                        res.Add(propertyAlias, values);
                    }
                }
            }
            return res;
        }

        private PropertyValues QueryPropertyValues(string propertyName) {
        	PropertyValues res = new PropertyValues();
        	IAssetType assetType = connector.MetaModel.GetAssetType(propertyName);
            IAttributeDefinition nameDef = assetType.GetAttributeDefinition(Entity.NameProperty);
        	IAttributeDefinition inactiveDef;

            Query query = new Query(assetType);
        	query.Selection.Add(nameDef);
        	if (assetType.TryGetAttributeDefinition("Inactive", out inactiveDef)) {
        		FilterTerm filter = new FilterTerm(inactiveDef);
        		filter.Equal("False");
        		query.Filter = filter;
			}

    		query.OrderBy.MajorSort(assetType.DefaultOrderBy, OrderBy.Order.Ascending);

			res.Add(new ValueId());
            foreach (Asset asset in connector.Services.Retrieve(query).Assets) {
                string name = asset.GetAttribute(nameDef).Value as string;
                res.Add(new ValueId(asset.Oid, name));
            }
            return res;
        }


        internal static DataLayerException Warning(string msg) {
            Debug.WriteLine(msg);
            return new DataLayerException(msg);
        }

        internal static DataLayerException Warning(string msg, Exception cause) {
            Debug.WriteLine(String.Format("{0}.\n\tException:{1}\n\t Stacktrace:{2}", msg, cause.Message, cause.StackTrace));
            return new DataLayerException(msg, cause);
        }

        #region Localizer

        public string LocalizerResolve(string key) {
            try {
                return connector.Localizer.Resolve(key);
            } catch (Exception ex) {
                throw new DataLayerException("Failed to resolve key.", ex);
            }
        }

        public bool TryLocalizerResolve(string key, out string result) {
            result = null;

            try {
                if (connector.Localizer != null) {
                    result = connector.Localizer.Resolve(key);
                    return true;
                }
            } catch (V1Exception) {}

            return false;
        }

        #endregion

        public static IDataLayer Instance {
            get {
                if (dataLayer == null) {
                    dataLayer = new ApiDataLayer();
                }
                return dataLayer;
            }
        }

        public bool IsConnected {
            get {
                if (!connector.IsConnected) {
                    try {
                        Reconnect();
                    } catch (DataLayerException) {
                        //Do nothing
                    }
                }
                return connector.IsConnected;
            }
        }

        /// <exception cref="KeyNotFoundException">If there are no values for this property.</exception>
        public PropertyValues GetListPropertyValues(string propertyName) {
            string propertyKey = ResolvePropertyKey(propertyName);
            return listPropertyValues.ContainsKey(propertyName) ? listPropertyValues[propertyKey] : null;
        }

        internal void CommitAsset(Asset asset) {
			try {
				List<RequiredFieldsDto> requiredData = requiredFieldsValidator.Validate(asset);

				if (requiredData.Count > 0) {
                    string message = requiredFieldsValidator.GetMessageOfUnfilledFieldsList(requiredData, Environment.NewLine, ", ");
					throw new ValidatorException(message);
				}
			} catch (APIException ex) {
				throw Warning("Cannot validate required fields.", ex);
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
            Connect(connector.Path, connector.Username, connector.Password, connector.Integrated);
		}

        internal double? GetEffort(Asset asset) {
            double res;
            if (efforts.TryGetValue(asset, out res))
                return res;
            return null;
        }

        internal void AddEffort(Asset asset, double newValue) {
			if (efforts.ContainsKey(asset)){
                if (newValue == 0) {
                    efforts.Remove(asset);
                } else {
                    efforts[asset] = newValue;
                }
			}
			else {
                if (newValue != 0) {
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
            IList<Asset> toRemove = new List<Asset>();

            foreach (KeyValuePair<Asset, double> pair in efforts) {
                if(exactAsset != null && !exactAsset.Equals(pair.Key)) {
                    continue;
                }
                
                Asset effort = connector.Services.New(EffortType, pair.Key.Oid);
                effort.SetAttributeValue(EffortType.GetAttributeDefinition("Value"), pair.Value);
                effort.SetAttributeValue(EffortType.GetAttributeDefinition("Date"), DateTime.Now);
                connector.Services.Save(effort);

                toRemove.Add(pair.Key);
            }

            foreach (Asset asset in toRemove) {
                efforts.Remove(asset);
            }
        }

        public void AddProperty(string attr, string prefix, bool isList) {
            attributesToQuery.AddLast(new AttributeInfo(attr, prefix, isList));
        }

        internal void ExecuteOperation(Asset asset, IOperation operation) {
            connector.Services.ExecuteOperation(operation, asset.Oid);
        }

        internal void CleanupWorkitem(Workitem item) {
            if (item.Parent != null && AllAssets.Contains(item.Parent.Asset)) {
                item.Parent.Asset.Children.Remove(item.Asset);
            }
            AllAssets.Remove(item.Asset);
            
            efforts.Remove(item.Asset);
            foreach (Asset child in item.Asset.Children) {
                efforts.Remove(child);
            }
        }

        /// <summary>
        /// Refreshes data for Asset wrapped by specified Workitem.
        /// </summary>
        internal void RefreshAsset(Workitem workitem) {
            try {
                IAttributeDefinition stateDef = workitem.Asset.AssetType.GetAttributeDefinition("AssetState");
                Query query = new Query(workitem.Asset.Oid.Momentless, false);
                AddSelection(query, workitem.TypePrefix);
                query.Selection.Add(stateDef);
                QueryResult newAssets = connector.Services.Retrieve(query);

                AssetList containedIn = workitem.Parent == null ? AllAssets : workitem.Parent.Asset.Children;

                if (newAssets.TotalAvaliable != 1) {
                    containedIn.Remove(workitem.Asset);
                    return;
                }

                Asset newAsset = newAssets.Assets[0];
                AssetState newAssetState = (AssetState)newAsset.GetAttribute(stateDef).Value;
                if (newAssetState == AssetState.Closed) {
                    containedIn.Remove(workitem.Asset);
                    return;
                }

                containedIn[containedIn.IndexOf(workitem.Asset)] = newAsset;
                newAsset.Children.AddRange(workitem.Asset.Children);
            }
            catch (MetaException ex) {
                throw Warning("Unable to get workitems.", ex);
            } catch (WebException ex) {
                connector.IsConnected = false;
                throw Warning("Unable to get workitems.", ex);
            } catch (Exception ex) {
                throw Warning("Unable to get workitems.", ex);
            }
        }

        public Workitem CreateWorkitem(string assetType, Workitem parent) {
            AssetFactory assetFactory = new AssetFactory(this, CurrentProject, attributesToQuery);
            Workitem item = WorkitemFactory.Instance.CreateWorkitem(assetFactory, assetType, parent);
            
            if(item.IsPrimary) {
                AllAssets.Add(item.Asset);
            }
            
            return item;
        }
    }
}