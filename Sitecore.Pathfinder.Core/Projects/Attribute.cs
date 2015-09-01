// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Diagnostics;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Projects
{
    [Flags]
    public enum SourceFlags
    {
        None = 0,

        IsQualified = 0x01,

        IsShort = 0x02,

        IsGuid = 0x04,

        IsFileName = 0x08,

        ConvertToXmlIdentifier = 0x10,

        ConvertToCodelIdentifier = 0x11
    }

    [DebuggerDisplay("\\{{GetType().Name,nq}\\}: {Name,nq} = {Value}")]
    public class Attribute<T>
    {
        [NotNull]
        private readonly T _defaultValue;

        public Attribute([NotNull] string name, [NotNull] T defaultValue)
        {
            Name = name;
            Value = defaultValue;
            _defaultValue = defaultValue;
        }

        public Attribute([NotNull] string name, [NotNull] T defaultValue, SourceFlags sourceFlags)
        {
            Name = name;
            Value = defaultValue;
            _defaultValue = defaultValue;
            SourceFlags = sourceFlags;
        }

        [NotNull]
        public string Name { get; private set; }

        // todo: attributes can be set in multiple sources - must be a list
        [CanBeNull]
        public ITextNode Source { get; private set; }

        public SourceFlags SourceFlags { get; set; }

        [NotNull]
        public T Value { get; private set; }

        public virtual bool AddSource([CanBeNull] ITextNode source)
        {
            if (source == null)
            {
                Value = _defaultValue;
            }
            else
            {
                Value = (T)Convert.ChangeType(source.Value, typeof(T));
            }

            Source = source;
            return true;
        }

        public virtual bool Merge([NotNull] Attribute<T> newAttribute)
        {
            Value = newAttribute.Value;

            // todo: add source to sources
            Source = newAttribute.Source;
            return true;
        }

        [NotNull]
        public static implicit operator string([NotNull] Attribute<T> attribute)
        {
            return attribute.Value.ToString();
        }

        public void Parse([NotNull] ITextNode textNode, T defaultValue = default(T))
        {
            var source = textNode.GetAttributeTextNode(Name);
            if (source == null)
            {
                if (defaultValue == null)
                {
                    defaultValue = _defaultValue;
                }

                SetValue(defaultValue);
                return;
            }

            AddSource(source);
        }

        public void Parse([NotNull] string newName, [NotNull] ITextNode textNode, T defaultValue = default(T))
        {
            Name = newName;
            Parse(textNode, defaultValue);
        }

        public virtual void SetValue([NotNull] T value)
        {
            Value = value;

            if (Source == null)
            {
                return;
            }

            var s = ConvertValue(value.ToString());
            Source.SetValue(s);
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        [NotNull]
        protected virtual string ConvertValue([NotNull] string value)
        {
            if ((SourceFlags & SourceFlags.IsShort) == SourceFlags.IsShort)
            {
                var n = value.LastIndexOf('/');
                value = n < 0 ? value : value.Mid(n + 1);
            }

            if ((SourceFlags & SourceFlags.ConvertToXmlIdentifier) == SourceFlags.ConvertToXmlIdentifier)
            {
                value = value.GetSafeXmlIdentifier();
            }

            if ((SourceFlags & SourceFlags.ConvertToCodelIdentifier) == SourceFlags.ConvertToCodelIdentifier)
            {
                value = value.GetSafeCodeIdentifier();
            }

            return value;
        }
    }
}
