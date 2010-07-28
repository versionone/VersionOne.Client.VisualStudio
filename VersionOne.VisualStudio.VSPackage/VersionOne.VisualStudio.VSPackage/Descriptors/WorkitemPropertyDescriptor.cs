using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Globalization;
using VersionOne.VisualStudio.DataLayer;
using VersionOne.VisualStudio.VSPackage.Events;
using VersionOne.VisualStudio.VSPackage.Settings;

namespace VersionOne.VisualStudio.VSPackage.Descriptors {
    public class WorkitemPropertyDescriptor : PropertyDescriptor {
        protected readonly string attribute;
        protected readonly PropertyUpdateSource updateSource;

        protected readonly IEventDispatcher eventDispatcher = EventDispatcher.Instance;

        protected bool readOnly;
        protected readonly Entity entity;

        public Workitem Workitem {
            get { return entity as Workitem; }
        }

        public Entity Entity {
            get { return entity; }
        }

        public string Attribute {
            get { return attribute; }
        }

        public WorkitemPropertyDescriptor(Entity entity, string name, string attribute, Attribute[] attrs, PropertyUpdateSource updateSource)
            : base(name, attrs) {
            this.entity = entity;
            this.attribute = attribute;
            this.updateSource = updateSource;
            readOnly = entity.IsPropertyDefinitionReadOnly(Attribute) || entity.IsPropertyReadOnly(Attribute);
        }

        public WorkitemPropertyDescriptor(Entity entity, string name, ColumnSetting settings, Attribute[] attrs, PropertyUpdateSource updateSource)
            : this(entity, name, settings.Attribute, attrs, updateSource) {
                readOnly |= settings.ReadOnly;
        }

        public override bool CanResetValue(object component) {
            return false;
        }

        public override Type ComponentType {
            get { return typeof(Entity); }
        }

        public override bool IsReadOnly {
            get { return readOnly; }
        }

        public override Type PropertyType {
            get { return typeof(string); }
        }

        public override void ResetValue(object component) {
        }

        public override bool ShouldSerializeValue(object component) {
            return false;
        }

        public override void SetValue(object component, object newValue) {
            Entity item = (Entity) component;

            if (newValue != null && newValue.Equals(string.Empty)) {
                newValue = null; 
            }

            item.SetProperty(Attribute, newValue);

            eventDispatcher.InvokeWorkitemPropertiesUpdated(this, new WorkitemPropertiesUpdatedArgs(updateSource));
        }

        public override object GetValue(object component) {
            Entity item = entity;

            try {
                object value = item.GetProperty(Attribute);
                if (value is double) {
                    return ((double)value).ToString("0.00", CultureInfo.CurrentCulture);
                } 
                return value;
            } catch (Exception ex) {
                ApiDataLayer.Warning(string.Format("Cannot get value of {0} of asset {1}.", Attribute, item), ex);
                return string.Empty;
            }
        }

        public override object GetEditor(Type editorBaseType) {
            if (editorBaseType == typeof(UITypeEditor)) {
                EditorAttribute editor = (EditorAttribute)Attributes[typeof(EditorAttribute)];

                if (editor != null) {
                    Type type = Type.GetType(editor.EditorTypeName);
                    return Activator.CreateInstance(type);
                }
            }

            return base.GetEditor(editorBaseType);
        }
    }
}