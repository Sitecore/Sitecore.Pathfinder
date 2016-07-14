// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Projects
{
    public abstract class SourcePropertyBag : ISourcePropertyBag, ILockable
    {
        [CanBeNull]
        private Dictionary<string, object> _propertyDictionary;

        public abstract Locking Locking { get; }

        [NotNull]
        protected IDictionary<string, object> PropertyDictionary => _propertyDictionary ?? (_propertyDictionary = new Dictionary<string, object>());

        IDictionary<string, object> ISourcePropertyBag.PropertyBag => PropertyDictionary;

        [NotNull]
        protected SourceProperty<T> NewSourceProperty<T>([NotNull] string name, [NotNull] T defaultValue, SourcePropertyFlags flags = SourcePropertyFlags.None)
        {
            if (Locking == Locking.ReadOnly)
            {
                throw new InvalidOperationException("ProjectItem cannot be modified as it is frozen");
            }

            if (PropertyDictionary.ContainsKey(name))
            {
                throw new ArgumentException("A property with the same name already exists");
            }

            var sourceProperty = new SourceProperty<T>(this, name, defaultValue, flags);
            PropertyDictionary[name] = sourceProperty;
            return sourceProperty;
        }

        bool ISourcePropertyBag.ContainsSourceProperty(string name)
        {
            return _propertyDictionary != null && PropertyDictionary.ContainsKey(name);
        }

        SourceProperty<T> ISourcePropertyBag.GetSourceProperty<T>(string name)
        {
            if (_propertyDictionary == null)
            {
                return null;
            }

            object property;
            return PropertyDictionary.TryGetValue(name, out property) ? (SourceProperty<T>)property : null;
        }

        /// <summary>Get a value from the property bag.</summary>
        T ISourcePropertyBag.GetValue<T>(string name)
        {
            if (_propertyDictionary == null)
            {
                return default(T);
            }

            object property;
            return PropertyDictionary.TryGetValue(name, out property) ? ((SourceProperty<T>)property).GetValue() : default(T);
        }

        /// <summary>Creates a new property in the property bag.</summary>
        SourceProperty<T> ISourcePropertyBag.NewSourceProperty<T>(string name, T defaultValue, SourcePropertyFlags flags)
        {
            return NewSourceProperty(name, defaultValue, flags);
        }

        /// <summary>Sets a value in the property bag.</summary>
        void ISourcePropertyBag.SetValue<T>(string name, T value)
        {
            if (Locking == Locking.ReadOnly)
            {
                throw new InvalidOperationException("ProjectItem cannot be modified as it is frozen");
            }

            object property;
            if (!PropertyDictionary.TryGetValue(name, out property))
            {
                throw new KeyNotFoundException();
            }

            ((SourceProperty<T>)property).SetValue(value);
        }
    }
}
