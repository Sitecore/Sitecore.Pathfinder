// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects.References;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Projects
{
    [DebuggerDisplay("{GetType().Name,nq}: {Uri}")]
    public abstract class ProjectItem : IProjectItem, ISourcePropertyBag
    {
        [CanBeNull]
        private Dictionary<string, object> _propertyDictionary;

        protected ProjectItem([NotNull] IProject project, [NotNull] ISnapshot snapshot, [NotNull] ProjectItemUri uri)
        {
            Project = project;
            Uri = uri;

            Snapshots.Add(snapshot);

            References = new ReferenceCollection(this);
        }

        public IProject Project { get; }

        /// <summary>The qualified name of the project item. For items it is the path of the item.</summary>
        public abstract string QualifiedName { get; }

        public ReferenceCollection References { get; }

        /// <summary>The unqualified name of the project item. For items it is the name of the item.</summary>
        public abstract string ShortName { get; }

        public ICollection<ISnapshot> Snapshots { get; } = new List<ISnapshot>();

        public ProjectItemState State { get; set; }

        /// <summary>The unique identification of the project item. For items the Uri.Guid is the ID of the item.</summary>
        public ProjectItemUri Uri { get; private set; }

        // holds the properties in a private dictionary
        [NotNull]
        private IDictionary<string, object> PropertyDictionary => _propertyDictionary ?? (_propertyDictionary = new Dictionary<string, object>());

        // exposes the properties only to the ISourcePropertyBag interface - this hides the properties from IntelliSense
        IDictionary<string, object> ISourcePropertyBag.PropertyBag => PropertyDictionary;

        public bool ContainsProperty(string name)
        {
            return _propertyDictionary != null && PropertyDictionary.ContainsKey(name);
        }

        public SourceProperty<T> GetProperty<T>(string name)
        {
            if (_propertyDictionary == null)
            {
                return null;
            }

            object property;
            return PropertyDictionary.TryGetValue(name, out property) ? (SourceProperty<T>)property : null;
        }

        /// <summary>Get a value from the property bag.</summary>
        public T GetValue<T>(string name)
        {
            if (_propertyDictionary == null)
            {
                return default(T);
            }

            object property;
            return PropertyDictionary.TryGetValue(name, out property) ? ((SourceProperty<T>)property).GetValue() : default(T);
        }

        /// <summary>Creates a new property in the property bag.</summary>
        public SourceProperty<T> NewProperty<T>(string name, T defaultValue, SourcePropertyFlags flags = SourcePropertyFlags.None)
        {
            if (PropertyDictionary.ContainsKey(name))
            {
                throw new ArgumentException("A property with the same name already exists");
            }

            var sourceProperty = new SourceProperty<T>(name, defaultValue, flags);
            PropertyDictionary[name] = sourceProperty;
            return sourceProperty;
        }

        /// <summary>Expertimental. Do not use.</summary>
        public abstract void Rename(string newShortName);

        /// <summary>Sets a value in the property bag.</summary>
        public void SetValue<T>(string name, T value, SetValueOptions options = SetValueOptions.EnableUpdates)
        {
            object property;
            if (!PropertyDictionary.TryGetValue(name, out property))
            {
                throw new KeyNotFoundException();
            }

            ((SourceProperty<T>)property).SetValue(value, options);
        }

        public override string ToString()
        {
            return Uri.ToString();
        }

        protected virtual void Merge([NotNull] IProjectItem newProjectItem, bool overwrite)
        {
            foreach (var snapshot in newProjectItem.Snapshots)
            {
                if (!Snapshots.Contains(snapshot))
                {
                    Snapshots.Add(snapshot);
                }
            }

            var propertyBag = newProjectItem as ISourcePropertyBag;
            if (propertyBag != null)
            {
                foreach (var pair in propertyBag.PropertyBag)
                {
                    PropertyDictionary[pair.Key] = propertyBag.PropertyBag[pair.Key];
                }
            }

            if (overwrite)
            {
                Uri = newProjectItem.Uri;
            }
        }
    }
}
