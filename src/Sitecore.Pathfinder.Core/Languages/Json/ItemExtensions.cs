// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Linq;
using Newtonsoft.Json;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects.Items;

namespace Sitecore.Pathfinder.Languages.Json
{
    public static class ItemExtensions
    {
        public static void WriteAsJson([NotNull] this Item item, [NotNull] JsonTextWriter output, [CanBeNull] Action<JsonTextWriter> writeInner = null)
        {
            output.WriteStartObject();

            output.WriteStartObject("Item");
            output.WritePropertyString("Id", item.Uri.Guid.Format());
            output.WritePropertyString("Database", item.DatabaseName);
            output.WritePropertyString("Name", item.ItemName);
            output.WritePropertyString("Path", item.ItemIdOrPath);
            output.WritePropertyString("Template", item.TemplateIdOrPath);

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
    }
}
