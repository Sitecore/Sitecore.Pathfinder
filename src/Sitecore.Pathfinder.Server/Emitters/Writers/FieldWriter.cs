// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Emitters.Writers
{
    public class FieldWriter
    {
        public FieldWriter([Diagnostics.NotNull] SourceProperty<Guid> fieldIdProperty, [Diagnostics.NotNull] SourceProperty<string> fieldNameProperty, [Diagnostics.NotNull] string language, int version, [Diagnostics.NotNull] string databaseValue)
        {
            FieldIdProperty = fieldIdProperty;
            FieldNameProperty = fieldNameProperty;
            Language = language;
            Version = version;
            DatabaseValue = databaseValue;
        }

        [Diagnostics.NotNull]
        public string DatabaseValue { get; set; }

        [Diagnostics.NotNull]
        public Guid FieldId
        {
            get { return FieldIdProperty.GetValue(); }
            set { FieldIdProperty.SetValue(value); }
        }

        [Diagnostics.NotNull]
        public SourceProperty<Guid> FieldIdProperty { get; }

        [NotNull]
        public string FieldName
        {
            get { return FieldNameProperty.GetValue(); }
            set { FieldNameProperty.SetValue(value); }
        }

        [Diagnostics.NotNull]
        public SourceProperty<string> FieldNameProperty { get; }

        [Diagnostics.NotNull]
        public string Language { get; }

        public int Version { get; }
    }
}
