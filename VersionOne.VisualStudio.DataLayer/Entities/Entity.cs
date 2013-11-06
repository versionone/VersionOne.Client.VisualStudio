using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using VersionOne.SDK.APIClient;
using VersionOne.VisualStudio.DataLayer.Logging;

namespace VersionOne.VisualStudio.DataLayer.Entities {
    public abstract class Entity {
        #region Constants

        public const string TaskType = "Task";
        public const string StoryType = "Story";
        public const string DefectType = "Defect";
        public const string TestType = "Test";
        public const string ProjectType = "Scope";

        public const string NameProperty = "Name";
        public const string StatusProperty = "Status";
        public const string EffortProperty = "Actuals";
        public const string DoneProperty = "Actuals.Value.@Sum";
        public const string ScheduleNameProperty = "Schedule.Name";
        public const string OwnersProperty = "Owners";
        public const string ToDoProperty = "ToDo";
        public const string OrderProperty = "Order";

        #endregion

        protected readonly IDataLayerInternal DataLayer;
        protected readonly IEntityContainer EntityContainer;
        protected readonly ILogger Logger;

        protected internal Asset Asset;

        public abstract string TypePrefix { get; }

        public virtual string Id {
            get { return Asset.Oid.Momentless.Token; }
        }

        private PropertyValues GetPropertyValues(string propertyName) {
            return DataLayer.GetListPropertyValues(TypePrefix + propertyName);
        }

        protected Entity(Asset asset, IEntityContainer entityContainer) {
            DataLayer = ServiceLocator.Instance.Get<IDataLayerInternal>();
            EntityContainer = entityContainer;
            Logger = DataLayer.Logger;
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
                return EntityContainer.GetEffort(this);
            }

            var attribute = Asset.Attributes[TypePrefix + '.' + propertyName];
            var allValues = GetPropertyValues(propertyName);

            if (attribute.Definition.IsMultiValue) {
                var currentValues = attribute.Values;
                return allValues == null ? currentValues : allValues.Subset(attribute.Values);
            }

            var value = attribute.Value;
            
            if (value is Oid) {
                return allValues == null ? value : allValues.Find((Oid)value);
            }

            return value;
        }

        /// <summary>
        /// Sets property value.
        /// </summary>
        /// <param name="propertyName">Short name of the property to set. Eg. "Name"</param>
        /// <param name="newValue">String, Double, null, ValueId, PropertyValues accepted.</param>
        public virtual void SetProperty(string propertyName, object newValue) {
            try {
                var attrDef = Asset.AssetType.GetAttributeDefinition(propertyName);

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
                Logger.Warn("Cannot set property: " + propertyName, ex);
            }
        }

        private void SetNumericProperty(string propertyName, object newValue) {
            var doubleValue = Convert.ToDouble(newValue, CultureInfo.CurrentCulture);

            if (propertyName == EffortProperty) {
                EntityContainer.AddEffort(this, doubleValue);
            } else if (newValue != null || doubleValue >= 0) {
                SetPropertyInternal(propertyName, doubleValue);
            }
        }

        private void SetPropertyInternal(string propertyName, object newValue) {
            var attribute = Asset.Attributes[TypePrefix + '.' + propertyName];
            var oldValue = attribute.Value;

            if (oldValue == newValue || (oldValue != null && oldValue.Equals(newValue))) {
                return;
            }

            Asset.SetAttributeValue(Asset.AssetType.GetAttributeDefinition(propertyName), newValue);
        }

        private void SetMultiValueProperty(string propertyName, object newValue) {
            var attrDef = Asset.AssetType.GetAttributeDefinition(propertyName);
            var attribute = Asset.Attributes[TypePrefix + '.' + propertyName];

            var oldValues = attribute.ValuesList;
            var newValues = (PropertyValues)newValue;

            foreach(var oldValue in oldValues.Cast<Oid>().Where(oldValue => !newValues.ContainsOid(oldValue))) {
                Asset.RemoveAttributeValue(attrDef, oldValue);
            }

            foreach(var value in newValues.Where(value => !oldValues.Contains(value.Oid))) {
                Asset.AddAttributeValue(attrDef, value.Oid);
            }
        }

        #region Equals, GetHashCode, ==, !=, ToString

        public override bool Equals(object obj) {
            if (obj == null || obj.GetType() != GetType()) {
                return false;
            }

            var other = (Entity) obj;
            
            return other.Asset.Oid == Asset.Oid;
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
            var fullName = TypePrefix + '.' + propertyName;
            
            try {
                return propertyName != EffortProperty && Asset.Attributes[fullName].Definition.IsReadOnly;
            } catch (Exception ex) {
                Logger.Warn("Cannot get property: " + fullName, ex);
                return true;
            }
        }

        public abstract bool IsPropertyReadOnly(string propertyName);
    }
}