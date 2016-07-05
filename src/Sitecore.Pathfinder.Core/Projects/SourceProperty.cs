// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

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

    [DebuggerDisplay("\\{{GetType().Name,nq}\\}: {Key,nq} = {GetValue()}")]
    public class SourceProperty<T> : INotifyPropertyChanged, IHasSourceTextNodes
    {
        [NotNull]
        private readonly T _defaultValue;

        [NotNull, ItemNotNull]
        private readonly ICollection<ITextNode> _additionalSourceTextNodes;

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

        public SourcePropertyFlags Flags { get; set; }

        [NotNull]
        public string Key { get; private set; }

        [NotNull]
        public ILockable Owner { get; }

        public ITextNode SourceTextNode { get; private set; } = TextNode.Empty;

        public IEnumerable<ITextNode> AdditionalSourceTextNodes => _additionalSourceTextNodes;

        protected Locking Locking => Owner.Locking;

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

        public virtual bool SetValue([NotNull] SourceProperty<T> sourceProperty)
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

            return true;
        }

        public virtual bool SetValue([NotNull] ITextNode textNode)
        {
            SetValue((T)Convert.ChangeType(textNode.Value, typeof(T)));
            AddAdditionalSourceTextNode(textNode);
            return true;
        }

        public virtual void SetValue([NotNull] T value)
        {
            if (Locking == Locking.ReadOnly)
            {
                throw new InvalidOperationException("SourceProperty cannot be modified as it is read only: " + Key);
            }

            if (value.Equals(_value))
            {
                return;
            }

            _value = value;

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
    }
}
