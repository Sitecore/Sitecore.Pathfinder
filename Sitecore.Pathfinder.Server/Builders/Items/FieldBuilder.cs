// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Builders.Items
{
    public class FieldBuilder
    {
        public FieldBuilder([Diagnostics.NotNull] Attribute<string> fieldName, [Diagnostics.NotNull] string language, int version, [Diagnostics.NotNull] string databaseValue)
        {
            FieldName = fieldName;
            Language = language;
            Version = version;
            DatabaseValue = databaseValue;
        }

        [Diagnostics.NotNull]
        public Attribute<string> FieldName { get; }

        [Diagnostics.NotNull]
        public string Language { get; }

        [Diagnostics.NotNull]
        public string DatabaseValue { get; set; }

        public int Version { get; }
    }
}
