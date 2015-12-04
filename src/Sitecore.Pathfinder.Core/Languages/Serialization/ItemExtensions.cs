// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.IO;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Items;

namespace Sitecore.Pathfinder.Languages.Serialization
{
    [Flags]
    public enum WriteAsSerializationOptions
    {
        None = 0,

        WriteCompiledFieldValues = 1
    }

    public static class ItemExtensions
    {
        public static void WriteAsSerialization([NotNull] this Item item, [NotNull] TextWriter writer, WriteAsSerializationOptions options = WriteAsSerializationOptions.None)
        {
            var parentId = string.Empty;

            var n = item.ItemIdOrPath.LastIndexOf('/');
            if (n >= 0)
            {
                var parentPath = item.ItemIdOrPath.Left(n);
                var parent = item.Project.FindQualifiedItem<IProjectItem>(parentPath);

                if (parent != null)
                {
                    parentId = parent.Uri.Guid.Format();
                }
            }

            writer.WriteLine("----item----");
            writer.WriteLine("version: 1");
            writer.WriteLine("id: " + item.Uri.Guid.Format());
            writer.WriteLine("database: " + item.DatabaseName);
            writer.WriteLine("path: " + item.ItemIdOrPath);
            writer.WriteLine("parent: " + parentId);
            writer.WriteLine("name: " + item.ItemName);
            writer.WriteLine("master: {00000000-0000-0000-0000-000000000000}");
            writer.WriteLine("template: " + item.Template.Uri.Guid.Format());
            writer.WriteLine("templatekey: " + item.Template.ItemName);
            writer.WriteLine();

            var sharedFields = item.Fields.Where(f => f.Version == 0 && string.IsNullOrEmpty(f.Language)).ToList();
            var versionedFields = item.Fields.Where(f => !string.IsNullOrEmpty(f.Language)).ToList();

            foreach (var field in sharedFields)
            {
                var value = options.HasFlag(WriteAsSerializationOptions.WriteCompiledFieldValues) ? field.CompiledValue : field.Value;

                writer.WriteLine("----field----");
                writer.WriteLine("field: " + (field.FieldId == Guid.Empty ? string.Empty : field.FieldId.Format()));
                writer.WriteLine("name: " + field.FieldName);
                writer.WriteLine("key: " + field.FieldName.ToLowerInvariant());
                writer.WriteLine("content-length: " + value.Length);
                writer.WriteLine();

                writer.WriteLine(value);
            }

            foreach (var language in versionedFields.Select(f => f.Language).Distinct().OrderBy(v => v))
            {
                foreach (var version in versionedFields.Where(f => f.Language == language).Select(f => f.Version).Distinct().OrderBy(v => v))
                {
                    writer.WriteLine("----version----");
                    writer.WriteLine("language: " + language);
                    writer.WriteLine("version: " + version);
                    writer.WriteLine("revision: ");
                    writer.WriteLine();

                    foreach (var field in item.Fields.Where(f => f.Language == language & f.Version == version))
                    {
                        var value = options.HasFlag(WriteAsSerializationOptions.WriteCompiledFieldValues) ? field.CompiledValue : field.Value;

                        writer.WriteLine("----field----");
                        writer.WriteLine("field: " + (field.FieldId == Guid.Empty ? string.Empty : field.FieldId.Format()));
                        writer.WriteLine("name: " + field.FieldName);
                        writer.WriteLine("key: " + field.FieldName.ToLowerInvariant());
                        writer.WriteLine("content-length: " + value.Length);
                        writer.WriteLine();

                        writer.WriteLine(value);
                    }
                }
            }
        }
    }
}
