// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Projects.Templates;

namespace Sitecore.Pathfinder.Languages.Json
{
    public static class JsonFormatExtensions
    {
        public static void WriteAsContentJson([NotNull] this Item item, [NotNull] TextWriter writer, [CanBeNull] Action<TextWriter> writeInner = null)
        {
            var output = new JsonTextWriter(writer)
            {
                Formatting = Formatting.Indented
            };

            output.WriteStartObject();
            output.WriteStartObject(item.TemplateName);
            output.WritePropertyString("Name", item.ItemName);
            output.WritePropertyString("Id", item.Uri.Guid.Format());
            output.WritePropertyStringIf("ItemPath", item.ItemIdOrPath);
            output.WritePropertyStringIf("Database", item.DatabaseName);

            foreach (var field in item.Fields.Where(f => f.TemplateField.Shared).OrderBy(f => f.FieldName))
            {
                output.WritePropertyStringIf(field.FieldName, field.Value);
            }

            if (item.Fields.Any(f => !f.TemplateField.Shared && f.Language != Language.Undefined && f.Language != Language.Empty))
            {
                output.WriteStartObject("..versions");

                var languages = item.Fields.Select(f => f.Language).Where(l => l != Language.Undefined && l != Language.Empty).Distinct().ToArray();

                foreach (var language in languages.OrderBy(l => l.LanguageName))
                {
                    var unversionedFields = item.Fields.Where(f => f.Language == language && f.TemplateField.Unversioned && !f.TemplateField.Shared).ToArray();
                    var versionedFields = item.Fields.Where(f => f.Language == language && !f.TemplateField.Unversioned && !f.TemplateField.Shared).ToArray();
                    if (!unversionedFields.Any() && !versionedFields.Any())
                    {
                        continue;
                    }

                    output.WriteStartObject(language.LanguageName);

                    foreach (var field in unversionedFields.OrderBy(f => f.FieldName))
                    {
                        output.WritePropertyStringIf(field.FieldName, field.Value);
                    }

                    var versions = versionedFields.Select(f => f.Version).Distinct().ToArray();
                    foreach (var version in versions.OrderByDescending(v => v.Number))
                    {
                        output.WriteStartObject(version.Number.ToString());

                        foreach (var field in versionedFields.Where(f => f.Version == version).OrderBy(f => f.FieldName))
                        {
                            output.WritePropertyStringIf(field.FieldName, field.Value);
                        }

                        output.WriteEndObject();
                    }

                    output.WriteEndObject();
                }

                output.WriteEndObject();
            }

            output.WriteEndObject();
            output.WriteEndObject();

            if (writeInner != null)
            {
                writeInner(writer);
            }
        }

        public static void WriteAsJson([NotNull] this Item item, [NotNull] TextWriter writer, [CanBeNull] Action<TextWriter> writeInner = null)
        {
            var output = new JsonTextWriter(writer)
            {
                Formatting = Formatting.Indented
            };

            output.WriteStartObject();

            output.WriteStartObject("Item");
            output.WritePropertyString("Name", item.ItemName);
            output.WritePropertyStringIf("Id", item.Uri.Guid.Format());
            output.WritePropertyStringIf("Database", item.DatabaseName);
            output.WritePropertyStringIf("ItemPath", item.ItemIdOrPath);
            output.WritePropertyStringIf("Template", item.TemplateIdOrPath);

            output.WriteStartObject("Fields");

            var sharedFields = item.Fields.Where(f => f.Language == Language.Undefined && f.Version == Projects.Items.Version.Undefined).ToList();
            var unversionedFields = item.Fields.Where(f => f.Language != Language.Undefined && f.Version == Projects.Items.Version.Undefined).ToList();
            var versionedFields = item.Fields.Where(f => f.Language != Language.Undefined && f.Version != Projects.Items.Version.Undefined).ToList();

            foreach (var field in sharedFields)
            {
                if (string.Equals(field.TemplateField.Type, "Layout", StringComparison.OrdinalIgnoreCase) || string.Equals(field.FieldName, "__Renderings", StringComparison.OrdinalIgnoreCase) || string.Equals(field.FieldName, "__Final Renderings", StringComparison.OrdinalIgnoreCase))
                {
                    var layout = "{" + field.Value + "}";
                    var jToken = layout.ToJToken();
                    jToken?.First.WriteTo(output);
                    continue;
                }

                output.WritePropertyString(field.FieldName, field.Value);
            }

            if (unversionedFields.Any())
            {
                output.WriteStartObject("Unversioned");

                foreach (var language in unversionedFields.Select(f => f.Language).Distinct())
                {
                    output.WriteStartObject(language.LanguageName);

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
                    output.WriteStartObject(language.LanguageName);

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
                writeInner(writer);
            }

            output.WriteEndObject();

            output.WriteEndObject();
        }

        public static void WriteAsJson([NotNull] this Template template, [NotNull] TextWriter writer)
        {
            var output = new JsonTextWriter(writer)
            {
                Formatting = Formatting.Indented
            };

            output.WriteStartObject();

            output.WriteStartObject("Template");
            output.WritePropertyString("Name", template.ItemName);
            output.WritePropertyStringIf("Id", template.Uri.Guid.Format());
            output.WritePropertyStringIf("Database", template.DatabaseName);
            output.WritePropertyStringIf("ItemPath", template.ItemIdOrPath);
            output.WritePropertyStringIf("BaseTemplates", template.BaseTemplates);

            output.WriteStartObject("Sections");

            foreach (var section in template.Sections)
            {
                output.WriteStartObject(section.SectionName);
                output.WritePropertyStringIf("Id", section.Uri.Guid.Format());
                output.WritePropertyStringIf("Name", section.SectionName);
                output.WritePropertyStringIf("Icon", section.Icon);

                output.WriteStartObject("Fields");

                foreach (var field in section.Fields)
                {
                    output.WriteStartObject(field.FieldName);
                    output.WritePropertyStringIf("Id", field.Uri.Guid.Format());
                    output.WritePropertyStringIf("Name", field.FieldName);
                    output.WritePropertyStringIf("Sortorder", field.Sortorder);
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
