// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Emitters.Writers
{
    public class FieldWriter
    {
        public FieldWriter([NotNull] ItemWriter itemWriter, [NotNull] SourceProperty<Guid> fieldIdProperty, [NotNull] SourceProperty<string> fieldNameProperty, [NotNull] string language, int version, [NotNull] string databaseValue)
        {
            ItemWriter = itemWriter;
            FieldIdProperty = fieldIdProperty;
            FieldNameProperty = fieldNameProperty;
            Language = language;
            Version = version;
            DatabaseValue = databaseValue;
        }

        [NotNull]
        public string DatabaseValue { get; set; }

        public Guid FieldId
        {
            get { return FieldIdProperty.GetValue(); }
            set { FieldIdProperty.SetValue(value); }
        }

        [NotNull]
        public ItemWriter ItemWriter { get; }

        [NotNull]
        public SourceProperty<Guid> FieldIdProperty { get; }

        [NotNull]
        public string FieldName
        {
            get { return FieldNameProperty.GetValue(); }
            set { FieldNameProperty.SetValue(value); }
        }

        [NotNull]
        public SourceProperty<string> FieldNameProperty { get; }

        [NotNull]
        public string Language { get; }

        public int Version { get; }
    }
}
