// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using Sitecore.Pathfinder.Compiling.FieldCompilers;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects.Templates;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Projects.Items
{
    // todo: consider basing this on ProjectElement
    [DebuggerDisplay("{GetType().Name,nq}: {FieldName,nq} = {Value}")]
    public class Field : SourcePropertyBag, IHasSourceTextNodes
    {
        [CanBeNull]
        private TemplateField _templateField;

        public Field([NotNull] Item item, [NotNull] ITextNode textNode)
        {
            Item = item;

            FieldIdProperty = NewSourceProperty("Id", Guid.Empty);
            FieldNameProperty = NewSourceProperty("Name", string.Empty);
            LanguageProperty = NewSourceProperty("Language", string.Empty);
            ValueHintProperty = NewSourceProperty("Value.Hint", string.Empty);
            ValueProperty = NewSourceProperty("Value", string.Empty);
            VersionProperty = NewSourceProperty("Version", 0);

            SourceTextNodes = new LockableList<ITextNode>(this)
            {
                textNode
            };

            ValueProperty.PropertyChanged += HandlePropertyChanged;
        }

        [NotNull]
        public string CompiledValue { get; private set; }

        [NotNull]
        public Database Database => Item.Database;

        [NotNull]
        public string DatabaseName => Item.DatabaseName;

        public Guid FieldId
        {
            get
            {
                var guid = FieldIdProperty.GetValue();
                if (guid != Guid.Empty)
                {
                    return guid;
                }

                var templateField = Item.Template.GetField(FieldName);
                if (templateField == null)
                {
                    return Guid.Empty;
                }

                if (Locking == Locking.ReadWrite)
                {
                    FieldId = templateField.Uri.Guid;
                }

                return templateField.Uri.Guid;
            }
            set { FieldIdProperty.SetValue(value); }
        }

        [NotNull]
        public SourceProperty<Guid> FieldIdProperty { get; }

        [NotNull]
        public string FieldName
        {
            get
            {
                var fieldName = FieldNameProperty.GetValue();
                if (!string.IsNullOrEmpty(fieldName))
                {
                    return fieldName;
                }

                var templateField = Item.Template.GetField(FieldName);
                if (templateField == null)
                {
                    return string.Empty;
                }

                if (Locking == Locking.ReadWrite)
                {
                    FieldName = templateField.FieldName;
                }

                return templateField.FieldName;
            }
            set { FieldNameProperty.SetValue(value); }
        }

        [NotNull]
        public SourceProperty<string> FieldNameProperty { get; }

        [NotNull, Obsolete("Use FieldId instead", false)]
        public ID ID => new ID(FieldId);

        public bool IsCompiled { get; set; }

        public override Locking Locking => Item.Locking;

        [NotNull]
        public Item Item { get; set; }

        [NotNull]
        public string Language
        {
            get { return LanguageProperty.GetValue(); }
            set { LanguageProperty.SetValue(value); }
        }

        [NotNull]
        public SourceProperty<string> LanguageProperty { get; }

        [NotNull, Obsolete("Use FieldName instead", false)]
        public string Name => FieldName;

        public ICollection<ITextNode> SourceTextNodes { get; } 

        [NotNull]
        public TemplateField TemplateField
        {
            get
            {
                if (_templateField != null)
                {
                    return _templateField;
                }

                var fieldName = FieldName;
                _templateField = Item.Template.GetAllFields().FirstOrDefault(f => string.Equals(f.FieldName, fieldName, StringComparison.OrdinalIgnoreCase));
                return _templateField ?? TemplateField.Empty;
            }
        }

        [NotNull]
        public string Value
        {
            get { return ValueProperty.GetValue(); }
            set { ValueProperty.SetValue(value); }
        }

        [NotNull]
        public string ValueHint
        {
            get { return ValueHintProperty.GetValue(); }
            set { ValueHintProperty.SetValue(value); }
        }

        [NotNull]
        public SourceProperty<string> ValueHintProperty { get; }

        [NotNull]
        public SourceProperty<string> ValueProperty { get; }

        public int Version
        {
            get { return VersionProperty.GetValue(); }
            set { VersionProperty.SetValue(value); }
        }

        [NotNull]
        public SourceProperty<int> VersionProperty { get; }

        public void Compile([NotNull] IFieldCompileContext context)
        {
            if (!context.FieldCompilers.Any() || IsCompiled)
            {
                return;
            }

            CompiledValue = Value;

            foreach (var compiler in context.FieldCompilers.OrderBy(r => r.Priority))
            {
                if (compiler.CanCompile(context, this))
                {
                    CompiledValue = compiler.Compile(context, this);
                    IsCompiled = true;

                    if (compiler.IsExclusive)
                    {
                        break;
                    }
                }
            }
        }

        public void Invalidate()
        {
            IsCompiled = false;
        }

        private void HandlePropertyChanged([NotNull] object sender, [NotNull] PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Value")
            {
                Invalidate();
            }
        }
    }
}
