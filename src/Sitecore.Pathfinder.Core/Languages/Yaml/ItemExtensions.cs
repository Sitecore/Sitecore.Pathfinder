// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;
using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects.Items;

namespace Sitecore.Pathfinder.Languages.Yaml
{
    public static class ItemExtensions
    {
        public static void WriteAsYaml([NotNull] this Item item, [NotNull] TextWriter output, int indent = 0, [CanBeNull] Action<TextWriter, int> writeInner = null)
        {
            var sharedFields = item.Fields.Where(f => string.IsNullOrEmpty(f.Language) && f.Version == 0).ToList();
            var unversionedFields = item.Fields.Where(f => !string.IsNullOrEmpty(f.Language) && f.Version == 0).ToList();
            var versionedFields = item.Fields.Where(f => !string.IsNullOrEmpty(f.Language) && f.Version != 0).ToList();

            output.WriteLineIndented(indent, "Item");
            indent++;

            output.WriteLineIndented(indent, "Id", item.Uri.Guid.Format());
            output.WriteLineIndented(indent, "Database", item.DatabaseName);
            output.WriteLineIndented(indent, "Name", item.ItemName);
            output.WriteLineIndented(indent, "Path", item.ItemIdOrPath);
            output.WriteLineIndented(indent, "Template", item.TemplateIdOrPath);

            output.WriteLineIndented(indent, "- Fields");
            indent++;

            foreach (var field in sharedFields)
            {
                output.WriteLineIndented(indent, "- Field", field.FieldName);
                indent++;
                output.WriteLineIndented(indent, "Value", field.Value);
                indent--;
            }

            if (unversionedFields.Any())
            {
                output.WriteLineIndented(indent, "- Unversioned");
                indent++;

                foreach (var language in unversionedFields.Select(f => f.Language).Distinct())
                {
                    output.WriteLineIndented(indent, language);
                    indent++;

                    foreach (var field in unversionedFields.Where(f => f.Language == language))
                    {
                        output.WriteLineIndented(indent, "- Field", field.FieldName);
                        indent++;
                        output.WriteLineIndented(indent, "Value", field.Value);
                        indent--;
                    }

                    indent--;
                }

                indent--;
            }

            if (versionedFields.Any())
            {
                output.WriteLineIndented(indent, "- Versioned");
                indent++;

                foreach (var language in versionedFields.Select(f => f.Language).Distinct())
                {
                    output.WriteLineIndented(indent, "- " + language);
                    indent++;

                    foreach (var version in versionedFields.Where(f => f.Language == language).Select(f => f.Version).Distinct())
                    {
                        output.WriteLineIndented(indent, "- " + version);
                        indent++;

                        foreach (var field in versionedFields.Where(f => f.Language == language && f.Version == version))
                        {
                            output.WriteLineIndented(indent, "- Field", field.FieldName);
                            indent++;
                            output.WriteLineIndented(indent, "Value", field.Value);
                            indent--;
                        }

                        indent--;
                    }

                    indent--;
                }

                indent--;
            }

            if (writeInner != null)
            {
                writeInner(output, indent);
            }
        }

        public static void WriteLineIndented([NotNull] this TextWriter output, int indent, [NotNull] string key, [NotNull] string value = "")
        {
            output.Write(new string(' ', indent * 4));
            output.Write(key);
            output.Write(" : ");
            output.WriteLine(value);
        }
    }
}
