using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using VersionOne.VisualStudio.DataLayer.Entities;
using VersionOne.VisualStudio.VSPackage.Events;
using VersionOne.VisualStudio.VSPackage.PropertyEditors;
using VersionOne.VisualStudio.DataLayer;
using VersionOne.VisualStudio.VSPackage.Settings;

namespace VersionOne.VisualStudio.VSPackage.Descriptors {
    public class WorkitemDescriptor : ICustomTypeDescriptor {
        private readonly Entity entity;
        private readonly PropertyUpdateSource updateSource;
        private readonly bool iconless;
        private readonly PropertyDescriptorCollection propertyDescriptors = new PropertyDescriptorCollection(new PropertyDescriptor[] {});

        private object title;
        private object owner;
        private object status;
        private object done;
        private object estimate;
        private object detailEstimate;
        private object effort;



        private readonly IDataLayer dataLayer;

        public WorkitemDescriptor(Entity entity, IEnumerable<ColumnSetting> columns, PropertyUpdateSource updateSource, bool iconless) {
            this.entity = entity;
            this.updateSource = updateSource;
            this.iconless = iconless;
            
            dataLayer = ServiceLocator.Instance.Get<IDataLayer>();
            ConfigurePropertyDescriptors(columns);
        }

        public Entity Entity {
            get { return entity; }
        }

        public Workitem Workitem {
            get { return entity as Workitem; }
        }

        public AttributeCollection GetAttributes() {
            return TypeDescriptor.GetAttributes(this, true);
        }

        // Begin Binding properties in order to show values in treenodes. 
        public object Title
        {
            get {
                if (title==null)
                {
                    title = this.GetProperty("Title");
                }
                return title;
            }
            set { 
                title = value; 
            }
        }

        public object ID
        {
          get
          {
            return GetProperty("ID");
          }
        }

        public object Owner
        {
          
            get {
                if (owner == null)
                {
                    owner = GetProperty("Owner");
                }
                return owner; 
            }
            set {
                owner = value;
            }
        }

        public object Icon
        {
            get { return GetProperty("Icon"); }
        }

        public object Status
        {
            get {
                if (status == null)
                {
                    status = GetProperty("Status");
                }
                return status;
            }
            set
            {
                status = value;
            }
        }

        public object Estimate 
        {
          get {
              if (estimate == null)
              {
                  estimate = GetProperty("Estimate");
              }
              return estimate;
          }
            set
            {
                estimate = value;
            }
        }

        public object DetailEstimate
        {
          get {
              if (detailEstimate == null)
              {
                  detailEstimate = GetProperty("DetailEstimate");
              }
              return detailEstimate;
          }
            set
            {
                detailEstimate = value;
            }
        }

        public object Done
        {
            get {
                if (done == null)
                {
                    done = GetProperty("Done");
                }
                return done;
            }
            set
            {
                done = value;
            }
        }

        public object Effort
        {
            get {
                if (effort == null)
                {
                    effort = GetProperty("Effort");
                }
                return effort;
            }
            set
            {
                effort = value;
            }
        }

        // End Binding properties in order to show values in treenodes. 


        public string GetClassName() {
            return "Details";
        }

        public string GetComponentName() {
            return (string) entity.GetProperty("Name");
        }

        public TypeConverter GetConverter() {
            return TypeDescriptor.GetConverter(this, true);
        }

        public EventDescriptor GetDefaultEvent() {
            return TypeDescriptor.GetDefaultEvent(this, false);
        }

        public PropertyDescriptor GetDefaultProperty() {
            return TypeDescriptor.GetDefaultProperty(this, true);
        }

        public object GetEditor(Type editorBaseType) {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        public EventDescriptorCollection GetEvents() {
            return TypeDescriptor.GetEvents(this, true);
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes) {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        public object GetProperty(string propertyName)
        {
          return GetProperties()[propertyName].GetValue(entity);
        }

        public PropertyDescriptorCollection GetProperties() {
            return GetProperties(null);
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes) {
            return propertyDescriptors;
        }

        public object GetPropertyOwner(PropertyDescriptor pd) {
            return entity;
        }

        public WorkitemDescriptor GetDetailedDescriptor(ColumnSetting[] settings, PropertyUpdateSource requiredUpdateSource) {
            return new WorkitemDescriptor(entity, settings, requiredUpdateSource, true);
        }

        private bool ShouldSkipColumnDueToEffortTracking(ColumnSetting column) {
            return column.EffortTracking && !dataLayer.EffortTracking.TrackEffort;
        }

        private void ConfigurePropertyDescriptors(IEnumerable<ColumnSetting> columns) {
            foreach (var column in columns) {
                if (ShouldSkipColumnDueToEffortTracking(column)) {
                    continue;
                }

                var attrs = new List<Attribute> {new CategoryAttribute(column.Category)};

                var name = dataLayer.LocalizerResolve(column.Name);

                switch (column.Type) {
                    case "String":
                    case "Effort":
                        break;
                    case "List":
                        attrs.Add(new EditorAttribute(typeof(ListPropertyEditor), typeof(UITypeEditor)));
                        break;
                    case "Multi":
                        attrs.Add(new EditorAttribute(typeof(MultiValueEditor), typeof(UITypeEditor)));
                        break;
                    case "RichText":
                        attrs.Add(new EditorAttribute(typeof(RichTextTypeEditor), typeof(UITypeEditor)));
                        break;
                }
                
                propertyDescriptors.Add(new WorkitemPropertyDescriptor(entity, name, column, attrs.ToArray(), updateSource));
            }

            if (!iconless) {
                propertyDescriptors.Add(new IconDescriptor("Icon"));
            }
        }

        #region Equals(), GetHashCode(), ==, !=

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) {
                return false;
            }
            if (ReferenceEquals(this, obj)) {
                return true;
            }
            if (obj.GetType() != typeof(WorkitemDescriptor)) {
                return false;
            }
            var other = (WorkitemDescriptor)obj;
            return Equals(other.entity, entity);
        }

        public override int GetHashCode() {
            unchecked {
                return entity.GetHashCode();
            }
        }

        public static bool operator ==(WorkitemDescriptor left, WorkitemDescriptor right) {
            return Equals(left, right);
        }

        public static bool operator !=(WorkitemDescriptor left, WorkitemDescriptor right) {
            return !Equals(left, right);
        }

        #endregion
    }
}