// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Emitters.Writers
{
    public class FieldWriter
    {
        public FieldWriter([Diagnostics.NotNull] SourceProperty<string> fieldNameProperty, [Diagnostics.NotNull] string language, int version, [Diagnostics.NotNull] string databaseValue)
        {
            FieldNameProperty = fieldNameProperty;
            Language = language;
            Version = version;
            DatabaseValue = databaseValue;
        }

        [Diagnostics.NotNull]
        public string DatabaseValue { get; set; }

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
