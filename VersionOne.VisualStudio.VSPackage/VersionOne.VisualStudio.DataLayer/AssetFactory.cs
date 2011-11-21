using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using VersionOne.SDK.APIClient;
using VersionOne.VisualStudio.DataLayer.Entities;
using VersionOne.VisualStudio.DataLayer.Logging;

namespace VersionOne.VisualStudio.DataLayer {
    internal class AssetFactory {
        private readonly Project currentProject;
        private readonly ApiDataLayer dataLayer;
        private readonly IEnumerable<AttributeInfo> attributesToQuery;

        private readonly IDictionary<string, IAssetType> typeMappings = new Dictionary<string, IAssetType>();

        private readonly ILogger logger;

        internal AssetFactory(ApiDataLayer dataLayer, Project currentProject, ILoggerFactory loggerFactory, IEnumerable<AttributeInfo> attributesToQuery) {
            this.dataLayer = dataLayer;
            this.currentProject = currentProject;
            this.attributesToQuery = attributesToQuery;

            logger = loggerFactory.GetLogger("AssetFactory");

            typeMappings.Add(Entity.DefectPrefix, dataLayer.DefectType);
            typeMappings.Add(Entity.TaskPrefix, dataLayer.TaskType);
            typeMappings.Add(Entity.TestPrefix, dataLayer.TestType);
        }

        internal Asset CreateAssetForPrimaryWorkitem(string typeToken) {
            var type = ResolveAssetTypeFor(typeToken);

            try {
                var asset = new Asset(type);

                SetupAssetAttributes(asset, typeToken);
                LoadAssetAttribute(asset, "Scope.Name", currentProject.GetProperty(Entity.NameProperty));
                LoadAssetAttribute(asset, "Timebox.Name", currentProject.GetProperty("Schedule.EarliestActiveTimebox.Name"));
                SetAssetAttribute(asset, "Timebox", currentProject.GetProperty("Schedule.EarliestActiveTimebox"));
                SetAssetAttribute(asset, "Scope", currentProject.Asset.Oid);

                return asset;
            } catch(MetaException ex) {
                throw new DataLayerException("Cannot create new " + typeToken, ex);
            } catch(APIException ex) {
                throw new DataLayerException("Cannot create new " + typeToken, ex);
            }
        }

        internal Asset CreateAssetForSecondaryWorkitem(string typeToken, Workitem parent) {
            var type = ResolveAssetTypeFor(typeToken);

            try {
                var asset = new Asset(type);

                SetupAssetAttributes(asset, typeToken);
                SetAssetAttribute(asset, "Parent", parent.Asset.Oid);
                LoadAssetAttribute(asset, "Scope.Name", currentProject.GetProperty(Entity.NameProperty));
                LoadAssetAttribute(asset, "Parent.Name", parent.GetProperty(Entity.NameProperty));
                LoadAssetAttribute(asset, "Timebox.Name", parent.GetProperty("Timebox.Name"));

                parent.Asset.Children.Add(asset);

                return asset;
            } catch(MetaException ex) {
                throw new DataLayerException("Cannot create new " + typeToken, ex);
            } catch(APIException ex) {
                throw new DataLayerException("Cannot create new " + typeToken, ex);
            }
        }

        private IAssetType ResolveAssetTypeFor(string typeToken) {
            if(typeMappings.ContainsKey(typeToken)) {
                return typeMappings[typeToken];
            }

            throw new NotSupportedException("Cannot create asset of type " + typeToken);
        }

        private static void SetAssetAttribute(Asset asset, string attrName, object value) {
            var type = asset.AssetType;
            var def = type.GetAttributeDefinition(attrName);
            
            if (value == null || (value is Oid && value.Equals(Oid.Null))) {
                asset.EnsureAttribute(def);
            } else {
                asset.SetAttributeValue(def, value);
            }
        }

        private static void LoadAssetAttribute(Asset asset, string attribute, object property) {
            var def = asset.AssetType.GetAttributeDefinition(attribute);
            asset.LoadAttributeValue(def, property);
        }

        private void SetupAssetAttributes(Asset asset, string typeToken) {
            foreach (var attrInfo in attributesToQuery.Where(attrInfo => attrInfo.Prefix == typeToken)) {
                try {
                    var def = dataLayer.Types[attrInfo.Prefix].GetAttributeDefinition(attrInfo.Attr);
                    asset.EnsureAttribute(def);
                } catch(MetaException ex) {
                    logger.Warn("Problem setting up attribute: " + ex.Message);
                }
            }
        }
    }
}