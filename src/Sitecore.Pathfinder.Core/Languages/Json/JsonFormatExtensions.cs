// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects.Items;

namespace Sitecore.Pathfinder.Languages.Json
{
    public static class JsonFormatExtensions
    {
        public static void WriteAsJson([NotNull] this Item item, [NotNull] TextWriter writer, bool writeChildren = false)
        {
            var output = new JsonTextWriter(writer)
            {
                Formatting = Formatting.Indented,
                Indentation = 4
            };

            output.WriteStartObject();
            output.WriteStartObject(item.TemplateName);
            output.WritePropertyString("Name", item.ItemName);
            output.WritePropertyString("Id", item.Uri.Guid.Format());
            output.WritePropertyStringIf("ItemPath", item.ItemIdOrPath);
            output.WritePropertyStringIf("Database", item.DatabaseName);

            if (item.Fields.Any())
            {
                output.WriteStartObject("Fields");

                foreach (var field in item.Versions.GetSharedFields().OrderBy(f => f.FieldName))
                {
                    output.WritePropertyString(field.FieldName, field.CompiledValue);
                }

                foreach (var language in item.Versions.GetLanguages())
                {
                    var unversionedFields = item.Versions.GetUnversionedFields(language);
                    var versions = item.Versions.GetVersions(language);

                    if (!unversionedFields.Any() && !versions.Any())
                    {
                        continue;
                    }

                    output.WriteStartObject(language.LanguageName);

                    foreach (var field in unversionedFields.OrderBy(f => f.FieldName))
                    {
                        output.WritePropertyString(field.FieldName, field.CompiledValue);
                    }

                    foreach (var version in versions.OrderByDescending(v => v.Number))
                    {
                        var versionedFields = item.Versions.GetVersionedFields(language, version);
                        if (!versionedFields.Any())
                        {
                            continue;
                        }

                        output.WriteStartObject(version.Number.ToString());

                        foreach (var field in versionedFields.OrderBy(f => f.FieldName))
                        {
                            output.WritePropertyString(field.FieldName, field.CompiledValue);
                        }

                        output.WriteEndObject();
                    }

                    output.WriteEndObject();
                }

                output.WriteEndObject();
            }

            if (writeChildren)
            {
                var children = item.Children.ToArray();
                if (children.Any())
                {
                    output.WriteStartArray("Children");
                    foreach (var child in children)
                    {
                        child.WriteAsJson(writer, true);
                    }

                    output.WriteEndArray();
                }
            }

            output.WriteEndObject();
            output.WriteEndObject();
        }
    }
}
