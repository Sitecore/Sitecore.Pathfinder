// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Projects
{
    [Flags]
    public enum SourcePropertyFlags
    {
        None = 0,

        IsQualified = 0x01,

        IsShort = 0x02,

        IsGuid = 0x04,

        IsSoftGuid = 0x08,

        IsFileName = 0x10,

        ConvertToXmlIdentifier = 0x11,

        ConvertToCodelIdentifier = 0x12
    }

    public enum SetValueOptions
    {
        DisableUpdates,

        EnableUpdates
    }

    [DebuggerDisplay("\\{{GetType().Name,nq}\\}: {Name,nq} = {GetValue()}")]
    public class SourceProperty<T> : INotifyPropertyChanged, IHasSourceTextNodes
    {
        [NotNull]
        private readonly T _defaultValue;

        private T _value;

        public SourceProperty([NotNull] string name, [NotNull] T defaultValue)
        {
            Name = name;

            _value = defaultValue;
            _defaultValue = defaultValue;
        }

        public SourceProperty([NotNull] string name, [NotNull] T defaultValue, SourcePropertyFlags sourcePropertyFlags)
        {
            Name = name;
            SourcePropertyFlags = sourcePropertyFlags;

            _value = defaultValue;
            _defaultValue = defaultValue;
        }

        [NotNull]
        public string Name { get; private set; }

        public SourcePropertyFlags SourcePropertyFlags { get; set; }

        [CanBeNull]
        public ITextNode SourceTextNode => SourceTextNodes.LastOrDefault();

        public ICollection<ITextNode> SourceTextNodes { get; } = new List<ITextNode>();

        public virtual bool AddSourceTextNode([CanBeNull] ITextNode textNode)
        {
            SourceTextNodes.Add(textNode);
            return true;
        }

        [NotNull]
        public T GetValue() => _value;

        public virtual void Parse([NotNull] ITextNode textNode, [CanBeNull] T defaultValue = default(T))
        {
            var source = textNode.GetAttributeTextNode(Name);

            if (source != null)
            {
                SetValue((T)Convert.ChangeType(source.Value, typeof(T)), SetValueOptions.DisableUpdates);
                AddSourceTextNode(source);
                return;
            }

            if (defaultValue != null)
            {
                SetValue(defaultValue, SetValueOptions.DisableUpdates);
                return;
            }

            SetValue(_defaultValue, SetValueOptions.DisableUpdates);
        }

        public virtual void Parse([NotNull] string newName, [NotNull] ITextNode textNode, [CanBeNull] T defaultValue = default(T))
        {
            Name = newName;
            Parse(textNode, defaultValue);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual bool SetValue([NotNull] SourceProperty<T> sourceProperty, SetValueOptions options = SetValueOptions.EnableUpdates)
        {
            SetValue(sourceProperty.GetValue(), options);

            foreach (var sourceTextNode in sourceProperty.SourceTextNodes)
            {
                AddSourceTextNode(sourceTextNode);
            }

            return true;
        }

        public virtual bool SetValue([NotNull] ITextNode textNode, SetValueOptions options = SetValueOptions.EnableUpdates)
        {
            SetValue((T)Convert.ChangeType(textNode.Value, typeof(T)), options);
            AddSourceTextNode(textNode);
            return true;
        }

        public virtual void SetValue([NotNull] T value, SetValueOptions options = SetValueOptions.EnableUpdates)
        {
            if (value.Equals(_value))
            {
                return;
            }

            _value = value;

            if (options == SetValueOptions.EnableUpdates)
            {
                foreach (var sourceTextNode in SourceTextNodes)
                {
                    sourceTextNode.SetValue(ToSourceValue(_value.ToString()));
                }
            }

            // ReSharper disable once NotResolvedInText
            OnPropertyChanged("Value");
        }

        public override string ToString()
        {
            return GetValue().ToString();
        }

        public virtual bool TryParse([NotNull] ITextNode textNode, [CanBeNull] T defaultValue = default(T))
        {
            var source = textNode.GetAttributeTextNode(Name);
            if (source == null)
            {
                return false;
            }

            Parse(textNode, defaultValue);
            return true;
        }

        public virtual bool TryParse([NotNull] string newName, [NotNull] ITextNode textNode, [CanBeNull] T defaultValue = default(T))
        {
            Name = newName;
            return TryParse(textNode, defaultValue);
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CanBeNull] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [NotNull]
        protected virtual string ToSourceValue([NotNull] string value)
        {
            if ((SourcePropertyFlags & SourcePropertyFlags.IsShort) == SourcePropertyFlags.IsShort)
            {
                var n = value.LastIndexOf('/');
                value = n < 0 ? value : value.Mid(n + 1);
            }

            if ((SourcePropertyFlags & SourcePropertyFlags.ConvertToXmlIdentifier) == SourcePropertyFlags.ConvertToXmlIdentifier)
            {
                value = value.GetSafeXmlIdentifier();
            }

            if ((SourcePropertyFlags & SourcePropertyFlags.ConvertToCodelIdentifier) == SourcePropertyFlags.ConvertToCodelIdentifier)
            {
                value = value.GetSafeCodeIdentifier();
            }

            return value;
        }
    }
}
