using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using VersionOne.SDK.APIClient;
using Attribute = VersionOne.SDK.APIClient.Attribute;

namespace VersionOne.VisualStudio.DataLayer.Entities {
    public abstract class Entity {
        #region Constants

        public const string TaskPrefix = "Task";
        public const string StoryPrefix = "Story";
        public const string DefectPrefix = "Defect";
        public const string TestPrefix = "Test";
        public const string ProjectPrefix = "Scope";

        public const string NameProperty = "Name";
        public const string StatusProperty = "Status";
        public const string EffortProperty = "Actuals";
        public const string DoneProperty = "Actuals.Value.@Sum";
        public const string ScheduleNameProperty = "Schedule.Name";
        public const string OwnersProperty = "Owners";
        public const string ToDoProperty = "ToDo";
        public const string DetailEstimateProperty = "DetailEstimate";
        public const string OrderProperty = "Order";

        #endregion

        protected ApiDataLayer dataLayer = ApiDataLayer.Instance as ApiDataLayer;
        protected internal Asset Asset;

        public abstract string TypePrefix { get; }

        public virtual string Id {
            get { return Asset.Oid.Momentless.Token; }
        }

        private PropertyValues GetPropertyValues(string propertyName) {
            return dataLayer.GetListPropertyValues(TypePrefix + propertyName);
        }

        protected Entity(Asset asset) {
            Asset = asset;
        }

        /// <summary>
        /// Gets property value.
        /// </summary>
        /// <param name="propertyName">Short name of the property to get. Eg. "Name"</param>
        /// <returns>String, Double, ValueId or IList&lt;ValueId&gt;.</returns>
        /// <exception cref="KeyNotFoundException">If no property found.</exception>
        public virtual object GetProperty(string propertyName) {
            if (propertyName == EffortProperty) {
                return dataLayer.GetEffort(Asset);
            }

            Attribute attribute = Asset.Attributes[TypePrefix + '.' + propertyName];

            PropertyValues allValues = GetPropertyValues(propertyName);

            if (attribute.Definition.IsMultiValue) {
                IEnumerable currentValues = attribute.Values;
                return allValues == null ? currentValues : allValues.Subset(attribute.Values);
            }

            object value = attribute.Value;
            if (value is Oid) {
                return allValues == null ? value : allValues.Find((Oid)value);
            }

            return value;
        }

        /// <summary>
        /// Sets property value.
        /// </summary>
        /// <param name="propertyName">Short name of the property to set. Eg. "Name"</param>
        /// <param name="newValue">String, Double, null, ValueId, PropertyValues accepted.</returns>
        public virtual void SetProperty(string propertyName, object newValue) {
            try {
                IAttributeDefinition attrDef = Asset.AssetType.GetAttributeDefinition(propertyName);

                // NOTE EffortProperty is in fact of type Relation, but should be handled as Numeric
                if (attrDef.AttributeType == AttributeType.Numeric || propertyName == EffortProperty) {
                    SetNumericProperty(propertyName, newValue);
                    return;
                }

                if (attrDef.IsMultiValue) {
                    SetMultiValueProperty(propertyName, newValue);
                    return;
                }

                if (newValue is ValueId) {
                    newValue = ((ValueId)newValue).Oid;
                } else if (string.Empty.Equals(newValue)) {
                    newValue = null;
                }

                SetPropertyInternal(propertyName, newValue);
            } catch (Exception ex) {
                Logger.Warning("Cannot set property: " + propertyName, ex);
            }
        }

        private void SetNumericProperty(string propertyName, object newValue) {
            double doubleValue = Convert.ToDouble(newValue, CultureInfo.CurrentCulture);

            if (propertyName == EffortProperty) {
                dataLayer.AddEffort(Asset, doubleValue);
            } else if (newValue != null || doubleValue >= 0) {
                SetPropertyInternal(propertyName, doubleValue);
            }
        }

        private void SetPropertyInternal(string propertyName, object newValue) {
            Attribute attribute = Asset.Attributes[TypePrefix + '.' + propertyName];

            object oldValue = attribute.Value;
            if (oldValue == newValue || (oldValue != null && oldValue.Equals(newValue))) {
                return;
            }

            Asset.SetAttributeValue(Asset.AssetType.GetAttributeDefinition(propertyName), newValue);
        }

        private void SetMultiValueProperty(string propertyName, object newValue) {
            IAttributeDefinition attrDef = Asset.AssetType.GetAttributeDefinition(propertyName);
            Attribute attribute = Asset.Attributes[TypePrefix + '.' + propertyName];

            IList oldValues = attribute.ValuesList;
            PropertyValues newValues = (PropertyValues)newValue;

            foreach (Oid oldValue in oldValues) {
                if (!newValues.ContainsOid(oldValue)) {
                    Asset.RemoveAttributeValue(attrDef, oldValue);
                }
            }
            foreach (ValueId value in newValues) {
                if (!oldValues.Contains(value.Oid)) {
                    Asset.AddAttributeValue(attrDef, value.Oid);
                }
            }
        }

        #region Equals, GetHashCode, ==, !=, ToString

        public override bool Equals(object obj) {
            if (obj == null || obj.GetType() != GetType()) {
                return false;
            }

            Entity other = (Entity) obj;
            if (other.Asset.Oid != Asset.Oid) {
                return false;
            }
            return true;
        }

        public override int GetHashCode() {
            return Asset.Oid.GetHashCode();
        }

        public static bool operator ==(Entity t1, Entity t2) {
            if (ReferenceEquals(t1, t2)) {
                return true;
            }
            if (ReferenceEquals(t1, null) || ReferenceEquals(t2, null)) {
                return false;
            }
            return t1.Equals(t2);
        }

        public static bool operator !=(Entity t1, Entity t2) {
            return !(t1 == t2);
        }

        public override string ToString() {
            return Id + (Asset.HasChanged ? " (Changed)" : "");
        }

        #endregion

        public bool IsPropertyDefinitionReadOnly(string propertyName) {
            string fullName = TypePrefix + '.' + propertyName;
            try {
                if (propertyName != EffortProperty && Asset.Attributes[fullName].Definition.IsReadOnly) {
                    return true;
                }

                return false;
            } catch (Exception ex) {
                Logger.Warning("Cannot get property: " + fullName, ex);
                return true;
            }
        }

        public abstract bool IsPropertyReadOnly(string propertyName);
    }
}