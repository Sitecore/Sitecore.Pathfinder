// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
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

    [DebuggerDisplay("\\{{GetType().Name,nq}\\}: {Key,nq} = {GetValue()}")]
    public class SourceProperty<T> : INotifyPropertyChanged, IHasSourceTextNodes
    {
        [NotNull]
        private readonly T _defaultValue;

        [NotNull]
        private T _value;

        public SourceProperty([NotNull] string key, [NotNull] T defaultValue)
        {
            Key = key;

            _value = defaultValue;
            _defaultValue = defaultValue;
        }

        public SourceProperty([NotNull] string key, [NotNull] T defaultValue, SourcePropertyFlags flags)
        {
            Key = key;
            Flags = flags;

            _value = defaultValue;
            _defaultValue = defaultValue;
        }

        public SourcePropertyFlags Flags { get; set; }

        [NotNull]
        public string Key { get; private set; }

        [CanBeNull]
        public ITextNode SourceTextNode => SourceTextNodes.LastOrDefault();

        public ICollection<ITextNode> SourceTextNodes { get; } = new List<ITextNode>();

        public virtual bool AddSourceTextNode([CanBeNull] ITextNode textNode)
        {
            if (textNode == TextNode.Empty)
            {
                return true;
            }

            if (SourceTextNodes.Contains(textNode))
            {
                return true;
            }

            SourceTextNodes.Add(textNode);
            return true;
        }

        public virtual bool AddSourceTextNode([NotNull] ISnapshot snapshot)
        {
            return AddSourceTextNode(new FileNameTextNode(PathHelper.GetFileNameWithoutExtensions(snapshot.SourceFile.AbsoluteFileName), snapshot));
        }

        [NotNull]
        public T GetValue() => _value;

        public virtual void Parse([NotNull] ITextNode textNode, [CanBeNull] T defaultValue = default(T))
        {
            var attribute = textNode.GetAttribute(Key);
            if (attribute != null)
            {
                SetValue(attribute, SetValueOptions.DisableUpdates);
            }
            else if (defaultValue != null)
            {
                SetValue(defaultValue, SetValueOptions.DisableUpdates);
            }
            else
            {
                SetValue(_defaultValue, SetValueOptions.DisableUpdates);
            }
        }

        public virtual void Parse([NotNull] string newKey, [NotNull] ITextNode textNode, [CanBeNull] T defaultValue = default(T))
        {
            Key = newKey;
            Parse(textNode, defaultValue);
        }

        public virtual bool ParseIfHasAttribute([NotNull] ITextNode textNode)
        {
            var attribute = textNode.GetAttribute(Key);
            if (attribute == null)
            {
                return false;
            }

            Parse(textNode);
            return true;
        }

        public virtual bool ParseIfHasAttribute([NotNull] string newKey, [NotNull] ITextNode textNode)
        {
            Key = newKey;
            return ParseIfHasAttribute(textNode);
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
                foreach (var sourceTextNode in SourceTextNodes.OfType<IMutableTextNode>())
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

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CanBeNull] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [NotNull]
        protected virtual string ToSourceValue([NotNull] string value)
        {
            if ((Flags & SourcePropertyFlags.IsShort) == SourcePropertyFlags.IsShort)
            {
                var n = value.LastIndexOf('/');
                value = n < 0 ? value : value.Mid(n + 1);
            }

            if ((Flags & SourcePropertyFlags.ConvertToXmlIdentifier) == SourcePropertyFlags.ConvertToXmlIdentifier)
            {
                value = value.EscapeXmlElementName();
            }

            if ((Flags & SourcePropertyFlags.ConvertToCodelIdentifier) == SourcePropertyFlags.ConvertToCodelIdentifier)
            {
                value = value.GetSafeCodeIdentifier();
            }

            return value;
        }
    }
}
