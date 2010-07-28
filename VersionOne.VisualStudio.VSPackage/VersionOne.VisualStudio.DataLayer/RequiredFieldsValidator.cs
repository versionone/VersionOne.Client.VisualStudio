using System;
using System.Collections.Generic;
using System.Text;

using VersionOne.SDK.APIClient;

using Attribute = VersionOne.SDK.APIClient.Attribute;

namespace VersionOne.VisualStudio.DataLayer {
    internal class RequiredFieldsValidator {
        private readonly IMetaModel metaModel;
        private readonly IServices services;
        private readonly IDataLayer dataLayer;
        private readonly Dictionary<string, List<RequiredFieldsDto>> requiredFieldsList = new Dictionary<string, List<RequiredFieldsDto>>();

        internal RequiredFieldsValidator(IMetaModel metaModel, IServices services, IDataLayer dataLayer) {
            this.metaModel = metaModel;
            this.services = services;
            this.dataLayer = dataLayer;

            requiredFieldsList.Add(Entity.TaskPrefix, GetRequiredFields(Entity.TaskPrefix));
            requiredFieldsList.Add(Entity.DefectPrefix, GetRequiredFields(Entity.DefectPrefix));
            requiredFieldsList.Add(Entity.StoryPrefix, GetRequiredFields(Entity.StoryPrefix));
            requiredFieldsList.Add(Entity.TestPrefix, GetRequiredFields(Entity.TestPrefix));
        }

        private List<RequiredFieldsDto> GetRequiredFields(string assetType) {
            List<RequiredFieldsDto> fields = new List<RequiredFieldsDto>();
            IAssetType attributeDefinitionAssetType = metaModel.GetAssetType("AttributeDefinition");
            IAttributeDefinition nameAttributeDef = attributeDefinitionAssetType.GetAttributeDefinition("Name");
            IAttributeDefinition assetNameAttributeDef = attributeDefinitionAssetType.GetAttributeDefinition("Asset.AssetTypesMeAndDown.Name");
            IAssetType taskType = metaModel.GetAssetType(assetType);

            Query query = new Query(attributeDefinitionAssetType);
            query.Selection.Add(nameAttributeDef);
            FilterTerm assetTypeTerm = new FilterTerm(assetNameAttributeDef);
            assetTypeTerm.Equal(assetType);
            query.Filter = new AndFilterTerm(new IFilterTerm[] { assetTypeTerm });

            QueryResult result;
            try {
                result = services.Retrieve(query);
            } catch (Exception ex) {
                throw ApiDataLayer.Warning("Cannot get meta data for " + assetType, ex);
            }

            foreach (Asset asset in result.Assets) {
                try {
                    string name = asset.GetAttribute(nameAttributeDef).Value.ToString();
                    if (IsRequiredField(taskType, name)) {
                        RequiredFieldsDto reqFieldData = new RequiredFieldsDto(name, taskType.GetAttributeDefinition(name).DisplayName);

                        fields.Add(reqFieldData);
                    }
                } catch (Exception e) {
                    throw ApiDataLayer.Warning("Cannot get meta data for " + assetType, e);
                }
            }

            return fields;
        }

        private bool IsRequiredField(IAssetType taskType, string name) {
            IAttributeDefinition def = taskType.GetAttributeDefinition(name);
            return def.IsRequired && !def.IsReadOnly;
        }

        private void ValidateAsset(Asset asset, IDictionary<Asset, List<RequiredFieldsDto>> validationResults) {
            List<RequiredFieldsDto> fields = Validate(asset);
            
            if (fields.Count > 0) {
                validationResults.Add(asset, fields);
            }
        }

        internal Dictionary<Asset, List<RequiredFieldsDto>> Validate(ICollection<Asset> assets) {
            Dictionary<Asset, List<RequiredFieldsDto>> requiredData = new Dictionary<Asset, List<RequiredFieldsDto>>();
            foreach (Asset asset in assets) {
                ValidateAsset(asset, requiredData);
                
                foreach(Asset child in asset.Children) {
                    ValidateAsset(child, requiredData);
                }
            }

            return requiredData;
        }

        internal List<RequiredFieldsDto> Validate(Asset asset) {
            List<RequiredFieldsDto> unfilledFields = new List<RequiredFieldsDto>();
            string type = asset.AssetType.Token;
            if (!requiredFieldsList.ContainsKey(type)) {
                return unfilledFields;
            }

            foreach (RequiredFieldsDto field in requiredFieldsList[type]) {
                string fullName = type + "." + field.Name;
                Attribute attribute = asset.Attributes[fullName];

                if (attribute == null) {
                    throw ApiDataLayer.Warning("Incorrect attribute: " + fullName);
                }

                if (IsMultiValueAndUnfilled(attribute) || IsSingleValueAndUnfilled(attribute)) {
                    unfilledFields.Add(field);
                }
            }

            return unfilledFields;
        }

        private bool IsSingleValueAndUnfilled(Attribute attribute) {
            return !attribute.Definition.IsMultiValue &&
                    ((attribute.Value is Oid && ((Oid)attribute.Value).IsNull) || attribute.Value == null);
        }

        private bool IsMultiValueAndUnfilled(Attribute attribute) {
            return (attribute.Definition.IsMultiValue && attribute.ValuesList.Count < 1);
        }


        internal string CreateErrorMessage(Dictionary<Asset, List<RequiredFieldsDto>> requiredData) {
            StringBuilder message = new StringBuilder();

            foreach (Asset asset in requiredData.Keys) {
                string type = asset.AssetType.Token;
                string assetDisplayName = dataLayer.LocalizerResolve(asset.AssetType.DisplayName);
                Attribute idAttribute = asset.Attributes[type + ".Number"];
                string id = idAttribute != null && idAttribute.Value != null ? idAttribute.Value.ToString() : "<New>";

                message.Append("The following fields are not filled for ").Append(id).Append(" ").Append(assetDisplayName).Append(":");
                message.Append(GetMessageOfUnfilledFieldsList(requiredData[asset], Environment.NewLine + "   ", Environment.NewLine + "   ")).Append(Environment.NewLine);
            }

            return message.ToString();
        }

        internal string GetMessageOfUnfilledFieldsList(List<RequiredFieldsDto> unfilledFields, string startWith, string delimiter) {
            StringBuilder message = new StringBuilder(startWith);

            foreach (RequiredFieldsDto field in unfilledFields) {
                string fieldDisplayName = dataLayer.LocalizerResolve(field.DisplayName);
                message.Append(fieldDisplayName).Append(delimiter);
            }
            message.Remove(message.Length - delimiter.Length, delimiter.Length);
            return message.ToString();
        }

        public List<RequiredFieldsDto> GetFields(string typePrefix) {
            if (requiredFieldsList.ContainsKey(typePrefix)) {
                return requiredFieldsList[typePrefix];
            }
            return null;
        }
    }
}
