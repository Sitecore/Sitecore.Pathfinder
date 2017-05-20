// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.IO;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Languages.Media;
using Sitecore.Pathfinder.Languages.Yaml;
using Sitecore.Pathfinder.Projects.Items;

namespace Sitecore.Pathfinder.Languages.Unicorn
{
    public static class UnicornFormatExtensions
    {
        public static void WriteAsUnicornYaml([NotNull] this Item item, [NotNull] TextWriter writer)
        {
            var parent = item.GetParent();

            var output = new YamlTextWriter(writer);
            output.Indentation = 2;

            output.WriteLine("---");
            output.WriteAttributeString("ID", "\"" + item.Uri.Guid.ToString("D") + "\"");
            if (parent != null)
            {
                output.WriteAttributeString("Parent", "\"" + parent.Uri.Guid.ToString("D") + "\"");
            }

            output.WriteAttributeString("Template", "\"" + item.Template.Uri.Guid.ToString("D") + "\"");
            output.WriteAttributeString("Path", item.ItemIdOrPath);
            output.WriteAttributeString("DB", item.DatabaseName);

            var sharedFields = item.Fields.Where(f => f.TemplateField.Shared).OrderBy(f => f.FieldName);
            var unversionedFields = item.Fields.Where(f => !f.TemplateField.Shared && f.TemplateField.Unversioned).OrderBy(f => f.FieldName);
            var versionedFields = item.Fields.Where(f => !f.TemplateField.Shared && !f.TemplateField.Unversioned).OrderBy(f => f.FieldName);

            if (sharedFields.Any())
            {
                output.WriteAttributeString("SharedFields");
                foreach (var field in sharedFields)
                {
                    WriteField(output, field);
                }
            }

            output.WriteAttributeString("Languages");

            if (unversionedFields.Any() || versionedFields.Any())
            {
                var languages = item.Versions.GetLanguages().OrderBy(l => l.LanguageName).ToArray();

                foreach (var language in languages)
                {
                    var unversionedLanguageFields = unversionedFields.Where(f => f.Language == language).ToArray();
                    var versionedLanguageFields = versionedFields.Where(f => f.Language == language).ToArray();
                    if (!unversionedLanguageFields.Any() && !versionedLanguageFields.Any())
                    {
                        continue;
                    }

                    output.WriteStartElement("Language", language.LanguageName);
                    output.WriteAttributeString("Fields");

                    foreach (var field in unversionedLanguageFields.OrderBy(f => f.FieldName))
                    {
                        WriteField(output, field);
                    }

                    output.WriteAttributeString("Versions");

                    var versions = versionedLanguageFields.Select(f => f.Version).Distinct().OrderByDescending(v => v.Number).ToArray();
                    if (versions.Any())
                    {
                        foreach (var version in versions.OrderByDescending(v => v.Number))
                        {
                            output.WriteStartElement("Version", version.Number.ToString());
                            output.WriteAttributeString("Fields");

                            foreach (var field in versionedLanguageFields.Where(f => f.Version == version).OrderBy(f => f.FieldName))
                            {
                                WriteField(output, field);
                            }

                            output.WriteEndElement();
                        }
                    }
                    else
                    {
                        WriteVersion(output);
                    }

                    output.WriteEndElement();
                }
            }
            else
            {
                output.WriteStartElement("Language", item.Database.Languages.First().LanguageName);
                output.WriteAttributeString("Fields");

                output.WriteAttributeString("Versions");
                WriteVersion(output);

                output.WriteEndElement();
            }
        }

        private static void WriteField([NotNull] YamlTextWriter output, [NotNull] Field field)
        {
            output.WriteStartElement("ID", "\"" + field.FieldId.ToString("D") + "\"");
            output.WriteAttributeString("Hint", field.FieldName);

            var type = field.TemplateField.Type;
            if (string.Equals(type, "attachment", StringComparison.OrdinalIgnoreCase))
            {
                var mediaFile = field.Item.Project.Files.OfType<MediaFile>().FirstOrDefault(f => f.MediaItemUri == field.Item.Uri);
                if (mediaFile != null)
                {
                    // todo: use IFileSystemService
                    var bytes = File.ReadAllBytes(mediaFile.Snapshot.SourceFile.AbsoluteFileName);
                    var value = Convert.ToBase64String(bytes);

                    output.WriteAttributeString("BlobID", "\"" + Guid.Parse(field.CompiledValue).ToString("D") + "\"");
                    output.WriteAttributeString("Value", value);

                    return;
                }
            }

            if (!string.Equals(type, "text", StringComparison.OrdinalIgnoreCase) && !string.Equals(type, "single-line text", StringComparison.OrdinalIgnoreCase))
            {
                output.WriteAttributeString("Type", type);
            }

            output.WriteAttributeString("Value", field.CompiledValue);
            output.WriteEndElement();
        }

        private static void WriteVersion([NotNull] YamlTextWriter output)
        {
            output.WriteStartElement("Version", "1");
            output.WriteAttributeString("Fields");

            output.WriteStartElement("ID", Constants.Fields.CreatedBy.ToString("D"));
            output.WriteAttributeString("Hint", "__Created by");
            output.WriteAttributeString("Value", "sitecore\\admin");
            output.WriteEndElement();

            output.WriteEndElement();
        }
    }
}
