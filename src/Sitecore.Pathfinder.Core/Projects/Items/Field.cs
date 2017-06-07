// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
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
    public class Field : TextNodeSourcePropertyBag, IVersioned
    {
        [CanBeNull]
        private TemplateField _templateField;

        public Field([NotNull] Item item)
        {
            Item = item;

            FieldIdProperty = NewSourceProperty("Id", Guid.Empty);
            FieldNameProperty = NewSourceProperty("Name", string.Empty);
            LanguageProperty = NewSourceProperty("Language", Language.Undefined);
            ValueProperty = NewSourceProperty("Value", string.Empty);
            VersionProperty = NewSourceProperty("Version", Version.Undefined);

            ValueProperty.PropertyChanged += HandlePropertyChanged;
        }

        [NotNull]
        public string CompiledValue { get; private set; } = string.Empty;

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
            set => FieldIdProperty.SetValue(value);
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
            set => FieldNameProperty.SetValue(value);
        }

        [NotNull]
        public SourceProperty<string> FieldNameProperty { get; }

        [NotNull, Obsolete("Use FieldId instead", false)]
        // ReSharper disable once InconsistentNaming
        public ID ID => new ID(FieldId);

        protected bool IsCompiled { get; set; }

        [NotNull]
        public Item Item { get; set; }

        public Language Language
        {
            get => LanguageProperty.GetValue();
            set => LanguageProperty.SetValue(value);
        }

        [NotNull]
        public SourceProperty<Language> LanguageProperty { get; }

        public override Locking Locking => Item.Locking;

        [NotNull, Obsolete("Use FieldName instead", false)]
        public string Name => FieldName;

        [NotNull]
        public virtual TemplateField TemplateField
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
            get => ValueProperty.GetValue();
            set => ValueProperty.SetValue(value);
        }

        [NotNull]
        public SourceProperty<string> ValueProperty { get; }

        public Version Version
        {
            get => VersionProperty.GetValue();
            set => VersionProperty.SetValue(value);
        }

        [NotNull]
        public SourceProperty<Version> VersionProperty { get; }

        [NotNull]
        public virtual string Compile([NotNull] IFieldCompileContext context)
        {
            if (IsCompiled)
            {
                return CompiledValue;
            }

            CompiledValue = Value;

            if (!context.FieldCompilers.Any())
            {
                return CompiledValue;
            }

            foreach (var compiler in context.FieldCompilers.OrderBy(r => r.Priority))
            {
                if (!compiler.CanCompile(context, this))
                {
                    continue;
                }

                CompiledValue = compiler.Compile(context, this);
                IsCompiled = true;

                if (compiler.IsExclusive)
                {
                    break;
                }
            }

            return CompiledValue;
        }

        public virtual void Invalidate()
        {
            IsCompiled = false;
        }

        [NotNull]
        public virtual Field With([NotNull] ITextNode textNode)
        {
            WithSourceTextNode(textNode);
            return this;
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
