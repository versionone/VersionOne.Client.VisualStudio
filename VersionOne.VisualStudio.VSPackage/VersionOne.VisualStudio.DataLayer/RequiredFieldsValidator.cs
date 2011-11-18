using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using VersionOne.SDK.APIClient;
using VersionOne.VisualStudio.DataLayer.Entities;
using VersionOne.VisualStudio.DataLayer.Logging;
using Attribute = VersionOne.SDK.APIClient.Attribute;

namespace VersionOne.VisualStudio.DataLayer {
    internal class RequiredFieldsValidator {
        private readonly IMetaModel metaModel;
        private readonly IServices services;
        private readonly IDataLayerInternal dataLayer;
        private readonly ILogger logger;
        private readonly Dictionary<string, List<RequiredFieldsDto>> requiredFieldsList = new Dictionary<string, List<RequiredFieldsDto>>();

        internal RequiredFieldsValidator(IMetaModel metaModel, IServices services, IDataLayerInternal dataLayer) {
            this.metaModel = metaModel;
            this.services = services;
            this.dataLayer = dataLayer;
            
            logger = dataLayer.Logger;

            requiredFieldsList.Add(Entity.TaskPrefix, GetRequiredFields(Entity.TaskPrefix));
            requiredFieldsList.Add(Entity.DefectPrefix, GetRequiredFields(Entity.DefectPrefix));
            requiredFieldsList.Add(Entity.StoryPrefix, GetRequiredFields(Entity.StoryPrefix));
            requiredFieldsList.Add(Entity.TestPrefix, GetRequiredFields(Entity.TestPrefix));
        }

        private List<RequiredFieldsDto> GetRequiredFields(string assetType) {
            var fields = new List<RequiredFieldsDto>();
            var attributeDefinitionAssetType = metaModel.GetAssetType("AttributeDefinition");
            var nameAttributeDef = attributeDefinitionAssetType.GetAttributeDefinition("Name");
            var assetNameAttributeDef = attributeDefinitionAssetType.GetAttributeDefinition("Asset.AssetTypesMeAndDown.Name");
            var taskType = metaModel.GetAssetType(assetType);

            var query = new Query(attributeDefinitionAssetType);
            query.Selection.Add(nameAttributeDef);
            var assetTypeTerm = new FilterTerm(assetNameAttributeDef);
            assetTypeTerm.Equal(assetType);
            query.Filter = new AndFilterTerm(new IFilterTerm[] { assetTypeTerm });

            QueryResult result;
            try {
                result = services.Retrieve(query);
            } catch (Exception ex) {
                logger.Error("Cannot get meta data for " + assetType, ex);
                return null;
            }

            foreach (var asset in result.Assets) {
                try {
                    var name = asset.GetAttribute(nameAttributeDef).Value.ToString();
                    
                    if(IsRequiredField(taskType, name)) {
                        var reqFieldData = new RequiredFieldsDto(name, taskType.GetAttributeDefinition(name).DisplayName);
                        fields.Add(reqFieldData);
                    }
                } catch(Exception ex) {
                    logger.Error("Cannot get meta data for " + assetType, ex);
                }
            }

            return fields;
        }

        private static bool IsRequiredField(IAssetType taskType, string name) {
            var def = taskType.GetAttributeDefinition(name);
            return def.IsRequired && !def.IsReadOnly;
        }

        private void ValidateAsset(Asset asset, IDictionary<Asset, List<RequiredFieldsDto>> validationResults) {
            var fields = Validate(asset);
            
            if (fields.Count > 0) {
                validationResults.Add(asset, fields);
            }
        }

        internal Dictionary<Asset, List<RequiredFieldsDto>> Validate(IEnumerable<Asset> assets) {
            var requiredData = new Dictionary<Asset, List<RequiredFieldsDto>>();
            
            foreach (var asset in assets) {
                ValidateAsset(asset, requiredData);
                
                foreach(var child in asset.Children) {
                    ValidateAsset(child, requiredData);
                }
            }

            return requiredData;
        }

        internal List<RequiredFieldsDto> Validate(Asset asset) {
            var unfilledFields = new List<RequiredFieldsDto>();
            var type = asset.AssetType.Token;
            
            if(!requiredFieldsList.ContainsKey(type)) {
                return unfilledFields;
            }

            foreach(var field in requiredFieldsList[type]) {
                var fullName = type + "." + field.Name;
                var attribute = asset.Attributes[fullName];

                if(attribute == null) {
                    logger.Error("Incorrect attribute: " + fullName);
                }

                if (IsMultiValueAndUnfilled(attribute) || IsSingleValueAndUnfilled(attribute)) {
                    unfilledFields.Add(field);
                }
            }

            return unfilledFields;
        }

        private static bool IsSingleValueAndUnfilled(Attribute attribute) {
            return !attribute.Definition.IsMultiValue &&
                    ((attribute.Value is Oid && ((Oid)attribute.Value).IsNull) || attribute.Value == null);
        }

        private bool IsMultiValueAndUnfilled(Attribute attribute) {
            return (attribute.Definition.IsMultiValue && attribute.ValuesList.Count < 1);
        }


        internal string CreateErrorMessage(Dictionary<Asset, List<RequiredFieldsDto>> requiredData) {
            var message = new StringBuilder();

            foreach(var asset in requiredData.Keys) {
                var type = asset.AssetType.Token;
                var assetDisplayName = dataLayer.LocalizerResolve(asset.AssetType.DisplayName);
                var idAttribute = asset.Attributes[type + ".Number"];
                var id = idAttribute != null && idAttribute.Value != null ? idAttribute.Value.ToString() : "<New>";

                message.Append("The following fields are not filled for ").Append(id).Append(" ").Append(assetDisplayName).Append(":");
                message.Append(GetMessageOfUnfilledFieldsList(requiredData[asset], Environment.NewLine + "   ", Environment.NewLine + "   ")).Append(Environment.NewLine);
            }

            return message.ToString();
        }

        internal string GetMessageOfUnfilledFieldsList(IEnumerable<RequiredFieldsDto> unfilledFields, string startWith, string delimiter) {
            var message = new StringBuilder(startWith);

            foreach(var fieldDisplayName in unfilledFields.Select(field => dataLayer.LocalizerResolve(field.DisplayName))) {
                message.Append(fieldDisplayName).Append(delimiter);
            }

            message.Remove(message.Length - delimiter.Length, delimiter.Length);
            return message.ToString();
        }

        public List<RequiredFieldsDto> GetFields(string typePrefix) {
            return requiredFieldsList.ContainsKey(typePrefix) ? requiredFieldsList[typePrefix] : null;
        }
    }
}