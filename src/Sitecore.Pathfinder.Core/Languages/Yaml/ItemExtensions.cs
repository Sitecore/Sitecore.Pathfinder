// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Projects.Templates;

namespace Sitecore.Pathfinder.Languages.Yaml
{
    public static class ItemExtensions
    {
        public static void WriteAsYaml([NotNull] this Item item, [NotNull] YamlTextWriter output, [CanBeNull] Action<YamlTextWriter> writeInner = null)
        {
            var sharedFields = item.Fields.Where(f => string.IsNullOrEmpty(f.Language) && f.Version == 0).ToList();
            var unversionedFields = item.Fields.Where(f => !string.IsNullOrEmpty(f.Language) && f.Version == 0).ToList();
            var versionedFields = item.Fields.Where(f => !string.IsNullOrEmpty(f.Language) && f.Version != 0).ToList();

            output.WriteStartElement("Item", string.Empty, false);

            output.WriteAttributeString("Id", item.Uri.Guid.Format());
            output.WriteAttributeStringIf("Database", item.DatabaseName);
            output.WriteAttributeStringIf("Name", item.ItemName);
            output.WriteAttributeStringIf("Path", item.ItemIdOrPath);
            output.WriteAttributeStringIf("Template", item.TemplateIdOrPath);

            output.WriteStartElement("Fields");

            foreach (var field in sharedFields)
            {
                output.WriteStartElement("Field", field.FieldName);
                output.WriteAttributeString("Value", field.Value);
                output.WriteEndElement();
            }

            if (unversionedFields.Any())
            {
                output.WriteStartElement("Unversioned");

                foreach (var language in unversionedFields.Select(f => f.Language).Distinct())
                {
                    output.WriteStartElement(language);

                    foreach (var field in unversionedFields.Where(f => f.Language == language))
                    {
                        output.WriteStartElement("Field", field.FieldName);
                        output.WriteAttributeString("Value", field.Value);
                        output.WriteEndElement();
                    }

                    output.WriteEndElement();
                }

                output.WriteEndElement();
            }

            if (versionedFields.Any())
            {
                output.WriteStartElement("Versioned");

                foreach (var language in versionedFields.Select(f => f.Language).Distinct())
                {
                    output.WriteStartElement(language);

                    foreach (var version in versionedFields.Where(f => f.Language == language).Select(f => f.Version).Distinct())
                    {
                        output.WriteStartElement(version.ToString());

                        foreach (var field in versionedFields.Where(f => f.Language == language && f.Version == version))
                        {
                            output.WriteStartElement("Field", field.FieldName);
                            output.WriteAttributeString("Value", field.Value);
                            output.WriteEndElement();
                        }

                        output.WriteEndElement();
                    }

                    output.WriteEndElement();
                }

                output.WriteEndElement();
            }

            if (writeInner != null)
            {
                writeInner(output);
            }
        }

        public static void WriteAsYaml([NotNull] this Template template, [NotNull] YamlTextWriter output)
        {
            output.WriteStartElement("Template", string.Empty, false);
            output.WriteAttributeString("Name", template.ItemName);
            output.WriteAttributeStringIf("Id", template.Uri.Guid.Format());
            output.WriteAttributeStringIf("Database", template.DatabaseName);
            output.WriteAttributeStringIf("Path", template.ItemIdOrPath);
            output.WriteAttributeStringIf("BaseTemplates", template.BaseTemplates);

            foreach (var section in template.Sections)
            {
                output.WriteStartElement("Section");
                output.WriteAttributeString("Name", section.SectionName);
                output.WriteAttributeStringIf("Id", section.Uri.Guid.Format());
                output.WriteAttributeStringIf("Icon", section.Icon);

                foreach (var field in section.Fields)
                {
                    output.WriteStartElement("Field");
                    output.WriteAttributeString("Name", field.FieldName);
                    output.WriteAttributeStringIf("Id", field.Uri.Guid.Format());
                    output.WriteAttributeStringIf("Sortorder", field.SortOrder);
                    output.WriteAttributeStringIf("Type", field.Type);
                    output.WriteAttributeStringIf("Source", field.Source);
                    output.WriteAttributeStringIf("Sharing", field.Shared ? "Shared" : field.Unversioned ? "Unversioned" : string.Empty);
                    output.WriteAttributeStringIf("ShortHelp", field.ShortHelp);
                    output.WriteAttributeStringIf("LongHelp", field.LongHelp);

                    output.WriteEndElement();
                }

                output.WriteEndElement();
            }

            output.WriteEndElement();
        }
    }
}
