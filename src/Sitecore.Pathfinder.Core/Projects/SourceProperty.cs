// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Projects.Items;
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

    [DebuggerDisplay("\\{{GetType().Name,nq}\\}: {Key,nq} = {GetValue()}")]
    public class SourceProperty<T> : INotifyPropertyChanged, IHasSourceTextNodes
    {
        [NotNull, ItemNotNull]
        private readonly ICollection<ITextNode> _additionalSourceTextNodes;

        [NotNull]
        private readonly T _defaultValue;

        [NotNull]
        private T _value;

        public SourceProperty([NotNull] ILockable owner, [NotNull] string key, [NotNull] T defaultValue)
        {
            Owner = owner;
            Key = key;

            _value = defaultValue;
            _defaultValue = defaultValue;
            _additionalSourceTextNodes = new List<ITextNode>();
        }

        public SourceProperty([NotNull] ILockable owner, [NotNull] string key, [NotNull] T defaultValue, SourcePropertyFlags flags)
        {
            Owner = owner;
            Key = key;
            Flags = flags;

            _value = defaultValue;
            _defaultValue = defaultValue;
            _additionalSourceTextNodes = new List<ITextNode>();
        }

        public IEnumerable<ITextNode> AdditionalSourceTextNodes => _additionalSourceTextNodes;

        public SourcePropertyFlags Flags { get; set; }

        [NotNull]
        public string Key { get; private set; }

        [NotNull]
        public ILockable Owner { get; }

        public ITextNode SourceTextNode { get; private set; } = TextNode.Empty;

        protected Locking Locking => Owner.Locking;

        [NotNull]
        public virtual SourceProperty<T> AddAdditionalSourceTextNode([CanBeNull] ITextNode textNode)
        {
            if (Locking == Locking.ReadOnly)
            {
                throw new InvalidOperationException("SourceProperty cannot be modified as it is read only: " + Key);
            }

            if (textNode == TextNode.Empty || textNode == null)
            {
                return this;
            }

            if (SourceTextNode == TextNode.Empty)
            {
                SourceTextNode = textNode;
                return this;
            }

            if (!AdditionalSourceTextNodes.Contains(textNode))
            {
                _additionalSourceTextNodes.Add(textNode);
            }

            return this;
        }

        [NotNull]
        public virtual SourceProperty<T> AddSourceTextNode([CanBeNull] ITextNode textNode)
        {
            if (Locking == Locking.ReadOnly)
            {
                throw new InvalidOperationException("SourceProperty cannot be modified as it is read only: " + Key);
            }

            if (textNode == TextNode.Empty || textNode == null)
            {
                return this;
            }

            if (SourceTextNode != TextNode.Empty && !AdditionalSourceTextNodes.Contains(SourceTextNode))
            {
                _additionalSourceTextNodes.Add(SourceTextNode);
            }

            SourceTextNode = textNode;

            return this;
        }

        [NotNull]
        public virtual SourceProperty<T> AddSourceTextNode([NotNull] ISnapshot snapshot)
        {
            return AddAdditionalSourceTextNode(new FileNameTextNode(PathHelper.GetFileNameWithoutExtensions(snapshot.SourceFile.AbsoluteFileName), snapshot));
        }

        [NotNull]
        public T GetValue() => _value;

        public virtual void Parse([NotNull] ITextNode textNode, [CanBeNull] T defaultValue = default(T))
        {
            if (Locking == Locking.ReadOnly)
            {
                throw new InvalidOperationException("SourceProperty cannot be modified as it is read only: " + Key);
            }

            var attribute = textNode.GetAttribute(Key);
            if (attribute != null)
            {
                SetValue(attribute);
            }
            else if (defaultValue != null)
            {
                SetValue(defaultValue);
            }
            else
            {
                SetValue(_defaultValue);
            }
        }

        public virtual void Parse([NotNull] string newKey, [NotNull] ITextNode textNode, [CanBeNull] T defaultValue = default(T))
        {
            if (Locking == Locking.ReadOnly)
            {
                throw new InvalidOperationException("SourceProperty cannot be modified as it is read only: " + Key);
            }

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
            if (Locking == Locking.ReadOnly)
            {
                throw new InvalidOperationException("SourceProperty cannot be modified as it is read only: " + Key);
            }

            Key = newKey;
            return ParseIfHasAttribute(textNode);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotNull]
        public virtual SourceProperty<T> SetValue([NotNull] SourceProperty<T> sourceProperty)
        {
            SetValue(sourceProperty.GetValue());

            if (sourceProperty.SourceTextNode != TextNode.Empty)
            {
                AddAdditionalSourceTextNode(sourceProperty.SourceTextNode);
            }

            foreach (var sourceTextNode in sourceProperty.AdditionalSourceTextNodes)
            {
                AddAdditionalSourceTextNode(sourceTextNode);
            }

            return this;
        }

        [NotNull]
        public virtual SourceProperty<T> SetValue([NotNull] ITextNode textNode)
        {
            object value;
            if (typeof(T) == typeof(string))
            {
                value = textNode.Value;
            }
            else if (typeof(T) == typeof(int))
            {
                int i;
                if (!int.TryParse(textNode.Value, out i))
                {
                    throw new InvalidOperationException("Cannot convert string to int");
                }

                value = i;
            }
            else if (typeof(T) == typeof(bool))
            {
                value = textNode.Value == "True";
            }
            else if (typeof(T) == typeof(Language))
            {
                value = new Language(textNode.Value);
            }
            else if (typeof(T) == typeof(Items.Version))
            {
                Items.Version version;
                if (!Items.Version.TryParse(textNode.Value, out version))
                {
                    throw new InvalidOperationException("Cannot convert string to version");
                }

                value = version;
            }
            else
            {
                value = (T)Convert.ChangeType(textNode.Value, typeof(T));
            }

            SetValue((T)value);
            AddAdditionalSourceTextNode(textNode);
            return this;
        }

        [NotNull]
        public virtual SourceProperty<T> SetValue([NotNull] T value)
        {
            if (Locking == Locking.ReadOnly)
            {
                throw new InvalidOperationException("SourceProperty cannot be modified as it is read only: " + Key);
            }

            if (value.Equals(_value))
            {
                return this;
            }

            _value = value;

            // ReSharper disable once NotResolvedInText
            OnPropertyChanged("Value");

            return this;
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
    }
}
