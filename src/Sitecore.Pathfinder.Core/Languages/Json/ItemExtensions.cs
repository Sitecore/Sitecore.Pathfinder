// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Linq;
using Newtonsoft.Json;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Projects.Templates;

namespace Sitecore.Pathfinder.Languages.Json
{
    public static class ItemExtensions
    {
        public static void WriteAsJson([NotNull] this Item item, [NotNull] JsonTextWriter output, [CanBeNull] Action<JsonTextWriter> writeInner = null)
        {
            output.WriteStartObject();

            output.WriteStartObject("Item");
            output.WritePropertyString("Name", item.ItemName);
            output.WritePropertyStringIf("Id", item.Uri.Guid.Format());
            output.WritePropertyStringIf("Database", item.DatabaseName);
            output.WritePropertyStringIf("Path", item.ItemIdOrPath);
            output.WritePropertyStringIf("Template", item.TemplateIdOrPath);

            output.WriteStartObject("Fields");

            var sharedFields = item.Fields.Where(f => string.IsNullOrEmpty(f.Language) && f.Version == 0).ToList();
            var unversionedFields = item.Fields.Where(f => !string.IsNullOrEmpty(f.Language) && f.Version == 0).ToList();
            var versionedFields = item.Fields.Where(f => !string.IsNullOrEmpty(f.Language) && f.Version != 0).ToList();

            foreach (var field in sharedFields)
            {
                output.WritePropertyString(field.FieldName, field.Value);
            }

            if (unversionedFields.Any())
            {
                output.WriteStartObject("Unversioned");

                foreach (var language in unversionedFields.Select(f => f.Language).Distinct())
                {
                    output.WriteStartObject(language);

                    foreach (var field in unversionedFields.Where(f => f.Language == language))
                    {
                        output.WritePropertyString(field.FieldName, field.Value);
                    }

                    output.WriteEndObject();
                }

                output.WriteEndObject();
            }

            if (versionedFields.Any())
            {
                output.WriteStartObject("Versioned");

                foreach (var language in versionedFields.Select(f => f.Language).Distinct())
                {
                    output.WriteStartObject(language);

                    foreach (var version in versionedFields.Where(f => f.Language == language).Select(f => f.Version).Distinct())
                    {
                        output.WriteStartObject(version.ToString());

                        foreach (var field in versionedFields.Where(f => f.Language == language && f.Version == version))
                        {
                            output.WritePropertyString(field.FieldName, field.Value);
                        }

                        output.WriteEndObject();
                    }

                    output.WriteEndObject();
                }

                output.WriteEndObject();
            }

            output.WriteEndObject();

            if (writeInner != null)
            {
                writeInner(output);
            }

            output.WriteEndObject();

            output.WriteEndObject();
        }

        public static void WriteAsJson([NotNull] this Template template, [NotNull] JsonTextWriter output)
        {
            output.WriteStartObject();

            output.WriteStartObject("Item");
            output.WritePropertyString("Name", template.ItemName);
            output.WritePropertyStringIf("Id", template.Uri.Guid.Format());
            output.WritePropertyStringIf("Database", template.DatabaseName);
            output.WritePropertyStringIf("Path", template.ItemIdOrPath);
            output.WritePropertyStringIf("BaseTemplates", template.BaseTemplates);

            output.WriteStartObject("Sections");

            foreach (var section in template.Sections)
            {
                output.WriteStartObject(section.SectionName);
                output.WritePropertyStringIf("Id", section.Uri.Guid.Format());
                output.WritePropertyStringIf("Icon", section.Icon);

                output.WriteStartObject("Fields");

                foreach (var field in section.Fields)
                {
                    output.WriteStartObject(field.FieldName);
                    output.WritePropertyStringIf("Id", field.Uri.Guid.Format());
                    output.WritePropertyStringIf("Sortorder", field.SortOrder);
                    output.WritePropertyStringIf("Type", field.Type);
                    output.WritePropertyStringIf("Source", field.Source);
                    output.WritePropertyStringIf("Sharing", field.Shared ? "Shared" : field.Unversioned ? "Unversioned" : string.Empty);
                    output.WritePropertyStringIf("ShortHelp", field.ShortHelp);
                    output.WritePropertyStringIf("LongHelp", field.LongHelp);
                    output.WriteEndObject();
                }

                output.WriteEndObject();

                output.WriteEndObject();
            }

            output.WriteEndObject();

            output.WriteEndObject();

            output.WriteEndObject();
        }
    }
}