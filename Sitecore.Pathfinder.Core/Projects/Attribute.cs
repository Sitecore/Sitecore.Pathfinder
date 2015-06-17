// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Diagnostics;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Documents;
using Sitecore.Pathfinder.Extensions;

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

    [DebuggerDisplay("{GetType().Name,nq}: {Name,nq} = {Value}")]
    public class Attribute<T>
    {
        public Attribute([NotNull] string name, [NotNull] T value)
        {
            Name = name;
            Value = value;
        }

        public Attribute([NotNull] ITextNode sourceTextNode, SourceFlags sourceFlags = SourceFlags.None)
        {
            Name = sourceTextNode.Name;
            Value = (T)Convert.ChangeType(sourceTextNode.Value, typeof(T));
            Source = sourceTextNode;
            SourceFlags = sourceFlags;
        }

        [NotNull]
        public string Name { get; }

        [CanBeNull]
        public ITextNode Source { get; set; }

        public SourceFlags SourceFlags { get; set; }

        [NotNull]
        public T Value { get; private set; }

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

        public virtual bool SetValue([CanBeNull] ITextNode source)
        {
            if (source == null)
            {
                Value = default(T);
            }
            else
            {
                Value = (T)Convert.ChangeType(source.Value, typeof(T));
            }

            Source = source;
            return true;
        }

        public virtual bool SetValue([NotNull] Attribute<T> newValue)
        {
            Value = newValue.Value;

            // todo: add source to sources
            Source = newValue.Source;
            return true;
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
