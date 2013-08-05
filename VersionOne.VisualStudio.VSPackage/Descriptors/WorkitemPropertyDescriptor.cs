using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Design;
using System.Globalization;
using VersionOne.VisualStudio.DataLayer;
using VersionOne.VisualStudio.DataLayer.Entities;
using VersionOne.VisualStudio.VSPackage.Events;
using VersionOne.VisualStudio.VSPackage.Settings;

namespace VersionOne.VisualStudio.VSPackage.Descriptors {
    public class WorkitemPropertyDescriptor : PropertyDescriptor {
        private readonly string attribute;
        private readonly PropertyUpdateSource updateSource;

        private static readonly IDictionary<PropertyUpdateSource, EventContext> ContextMappings = new Dictionary<PropertyUpdateSource, EventContext> {
                                          { PropertyUpdateSource.ProjectPropertyView, EventContext.ProjectPropertiesUpdated },
                                          { PropertyUpdateSource.ProjectView, EventContext.ProjectPropertiesUpdated },
                                          { PropertyUpdateSource.WorkitemPropertyView, EventContext.WorkitemPropertiesUpdatedFromPropertyView },
                                          { PropertyUpdateSource.WorkitemView, EventContext.WorkitemPropertiesUpdatedFromView },
                                      };

        private readonly IEventDispatcher eventDispatcher = ServiceLocator.Instance.Get<IEventDispatcher>();

        private readonly bool readOnly;
        private readonly Entity entity;

        public Workitem Workitem {
            get { return entity as Workitem; }
        }

        public Entity Entity {
            get { return entity; }
        }

        public string Attribute {
            get { return attribute; }
        }

        private WorkitemPropertyDescriptor(Entity entity, string name, string attribute, Attribute[] attrs, PropertyUpdateSource updateSource)
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
            var item = (Entity) component;

            if (newValue != null && newValue.Equals(string.Empty)) {
                newValue = null; 
            }

            item.SetProperty(Attribute, newValue);
            eventDispatcher.Notify(this, ResolveChangeArgs());
        }

        private ModelChangedArgs ResolveChangeArgs() {
            var receiver =
                new[] {PropertyUpdateSource.ProjectPropertyView, PropertyUpdateSource.ProjectView}.Contains(updateSource)
                    ? EventReceiver.ProjectView
                    : EventReceiver.WorkitemView;

            var context = ContextMappings[updateSource];

            return new ModelChangedArgs(receiver, context);
        }

        public override object GetValue(object component) {
            var item = entity;

            try {
                var value = item.GetProperty(Attribute);
                
                if(value is double) {
                    return ((double)value).ToString("0.00", CultureInfo.CurrentCulture);
                } 

                return value;
            } catch (Exception ex) {
                // TODO possibly log this, but this would the only Logger usage among descriptors
                Debug.WriteLine(string.Format("Cannot get value of {0} of asset {1}.", Attribute, item), ex);
                return string.Empty;
            }
        }

        public override object GetEditor(Type editorBaseType) {
            if (editorBaseType == typeof(UITypeEditor)) {
                var editor = (EditorAttribute) Attributes[typeof(EditorAttribute)];

                if (editor != null) {
                    var type = Type.GetType(editor.EditorTypeName);
                    return Activator.CreateInstance(type);
                }
            }

            return base.GetEditor(editorBaseType);
        }
    }
}