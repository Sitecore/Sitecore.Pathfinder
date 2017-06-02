// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.IO;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects.Items;

namespace Sitecore.Pathfinder.Languages.Yaml
{
    public static class YamlFormatExtensions
    {
        public static void WriteAsYaml([NotNull] this Item item, [NotNull] TextWriter writer, bool writeChildren = false)
        {
            var output = new YamlTextWriter(writer);

            output.WriteStartElement(item.TemplateName, item.ItemName);
            output.WriteAttributeString("Id", item.Uri.Guid.Format());
            output.WriteAttributeStringIf("ItemPath", item.ItemIdOrPath);
            output.WriteAttributeStringIf("Database", item.DatabaseName);

            if (item.Fields.Any())
            {
                output.WriteStartElement("Fields");

                foreach (var field in item.Versions.GetSharedFields().OrderBy(f => f.FieldName))
                {
                    output.WriteAttributeString(field.FieldName, field.CompiledValue);
                }

                foreach (var language in item.Versions.GetLanguages())
                {
                    var unversionedFields = item.Versions.GetUnversionedFields(language);
                    var versions = item.Versions.GetVersions(language);

                    if (!unversionedFields.Any() && !versions.Any())
                    {
                        continue;
                    }

                    output.WriteStartElement(language.LanguageName);

                    foreach (var field in unversionedFields.OrderBy(f => f.FieldName))
                    {
                        output.WriteAttributeString(field.FieldName, field.CompiledValue);
                    }

                    foreach (var version in versions.OrderByDescending(v => v.Number))
                    {
                        var versionedFields = item.Versions.GetVersionedFields(language, version);
                        if (!versionedFields.Any())
                        {
                            continue;
                        }

                        output.WriteStartElement(version.Number.ToString());

                        foreach (var field in versionedFields.OrderBy(f => f.FieldName))
                        {
                            output.WriteAttributeString(field.FieldName, field.CompiledValue);
                        }

                        output.WriteEndElement();
                    }

                    output.WriteEndElement();
                }

                output.WriteEndElement();
            }

            if (writeChildren)
            {
                var children = item.Children.ToArray();
                if (children.Any())
                {
                    output.WriteStartElement("Children");
                    foreach (var child in children)
                    {
                        child.WriteAsYaml(writer, true);
                    }

                    output.WriteEndElement();
                }
            }

            output.WriteEndElement();
        }
    }
}
