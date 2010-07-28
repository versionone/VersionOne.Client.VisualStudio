using System;
using System.Collections.Generic;
using System.Diagnostics;
using VersionOne.SDK.APIClient;

namespace VersionOne.VisualStudio.DataLayer {
    internal class AssetFactory {
        private readonly Project currentProject;
        private readonly ApiDataLayer dataLayer;
        private readonly IEnumerable<AttributeInfo> attributesToQuery;

        private readonly IDictionary<string, IAssetType> TypeMappings = new Dictionary<string, IAssetType>();

        internal AssetFactory(ApiDataLayer dataLayer, Project currentProject, IEnumerable<AttributeInfo> attributesToQuery) {
            this.dataLayer = dataLayer;
            this.currentProject = currentProject;
            this.attributesToQuery = attributesToQuery;

            TypeMappings.Add(Entity.DefectPrefix, dataLayer.DefectType);
            TypeMappings.Add(Entity.TaskPrefix, dataLayer.TaskType);
            TypeMappings.Add(Entity.TestPrefix, dataLayer.TestType);
        }

        internal Asset CreateAssetForPrimaryWorkitem(string typeToken) {
            IAssetType type = ResolveAssetTypeFor(typeToken);

            try {
                Asset asset = new Asset(type);

                SetupAssetAttributes(asset, typeToken);
                LoadAssetAttribute(asset, "Scope.Name", currentProject.GetProperty(Entity.NameProperty));
                LoadAssetAttribute(asset, "Timebox.Name", currentProject.GetProperty("Schedule.EarliestActiveTimebox.Name"));
                SetAssetAttribute(asset, "Timebox", currentProject.GetProperty("Schedule.EarliestActiveTimebox"));
                SetAssetAttribute(asset, "Scope", currentProject.Asset.Oid);

                return asset;
            }
            catch (MetaException ex) {
                throw new DataLayerException("Cannot create new " + typeToken, ex);
            }
            catch (APIException ex) {
                throw new DataLayerException("Cannot create new " + typeToken, ex);
            }
        }

        internal Asset CreateAssetForSecondaryWorkitem(string typeToken, Workitem parent) {
            IAssetType type = ResolveAssetTypeFor(typeToken);

            try {
                Asset asset = new Asset(type);

                SetupAssetAttributes(asset, typeToken);
                SetAssetAttribute(asset, "Parent", parent.Asset.Oid);
                LoadAssetAttribute(asset, "Scope.Name", currentProject.GetProperty(Entity.NameProperty));
                LoadAssetAttribute(asset, "Parent.Name", parent.GetProperty(Entity.NameProperty));
                LoadAssetAttribute(asset, "Timebox.Name", parent.GetProperty("Timebox.Name"));

                parent.Asset.Children.Add(asset);

                return asset;
            }
            catch (MetaException ex) {
                throw new DataLayerException("Cannot create new " + typeToken, ex);
            }
            catch (APIException ex) {
                throw new DataLayerException("Cannot create new " + typeToken, ex);
            }
        }

        private IAssetType ResolveAssetTypeFor(string typeToken) {
            if(TypeMappings.ContainsKey(typeToken)) {
                return TypeMappings[typeToken];
            }

            throw new NotSupportedException("Cannot create asset of type " + typeToken);
        }

        private static void SetAssetAttribute(Asset asset, string attrName, object value) {
            IAssetType type = asset.AssetType;
            IAttributeDefinition def = type.GetAttributeDefinition(attrName);
            if (value == null || (value is Oid && value.Equals(Oid.Null))) {
                asset.EnsureAttribute(def);
            } else {
                asset.SetAttributeValue(def, value);
            }
        }

        private static void LoadAssetAttribute(Asset asset, string attribute, object property) {
            IAttributeDefinition def = asset.AssetType.GetAttributeDefinition(attribute);
            asset.LoadAttributeValue(def, property);
        }

        private void SetupAssetAttributes(Asset asset, string typeToken) {
            foreach (AttributeInfo attrInfo in attributesToQuery) {
                if (attrInfo.Prefix != typeToken) {
                    continue;
                }

                try {
                    IAttributeDefinition def = dataLayer.Types[attrInfo.Prefix].GetAttributeDefinition(attrInfo.Attr);
                    asset.EnsureAttribute(def);
                }
                catch (MetaException ex) {
                    Debug.WriteLine("Problem setting up attribute: " + ex.Message);
                }
            }
        }
    }
}