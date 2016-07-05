// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.ComponentModel.Composition;
using Rainbow.Storage.Yaml;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Parsing;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Unicorn.Languages.Unicorn
{
    public class UnicornFileParser : ParserBase
    {
        private const string FileExtension = ".yml";

        [ImportingConstructor]
        public UnicornFileParser([NotNull] IFileSystemService fileSystem) : base(Constants.Parsers.Items)
        {
            FileSystem = fileSystem;
        }

        [NotNull]
        protected IFileSystemService FileSystem { get; }

        public override bool CanParse(IParseContext context)
        {
            if (string.IsNullOrEmpty(context.ItemPath))
            {
                return false;
            }

            return context.Snapshot.SourceFile.AbsoluteFileName.EndsWith(FileExtension, StringComparison.OrdinalIgnoreCase);
        }

        public override void Parse(IParseContext context)
        {
            var unicornFile = new UnicornFile(context.Project, context.Snapshot, context.FilePath, context.DatabaseName);
            context.Project.AddOrMerge(unicornFile);

            try
            {
                CompileUnicornFile(context, context.Snapshot, unicornFile);
            }
            catch (NotImplementedException)
            {
                // using stub Sitecore.Kernel assembly in unit tests
            }
        }

        protected virtual void CompileUnicornFile([NotNull] IParseContext context, [NotNull] ISnapshot snapshot, [NotNull] UnicornFile unicornFile)
        {
            // todo: use real Unicorn configuration instead of hacking it
            var formatter = new YamlSerializationFormatter(null, new AllFieldFilter());
            using (var stream = FileSystem.OpenRead(snapshot.SourceFile.AbsoluteFileName))
            {
                var serializedItem = formatter.ReadSerializedItem(stream, unicornFile.ShortName);

                var guid = serializedItem.Id;
                var databaseName = serializedItem.DatabaseName ?? unicornFile.DatabaseName;
                var itemIdOrPath = serializedItem.Path;
                var itemName = serializedItem.Name;
                var templateIdOrPath = serializedItem.TemplateId.Format();

                var item = context.Factory.Item(unicornFile.Project, guid, databaseName, itemName, itemIdOrPath, templateIdOrPath).With(new SnapshotTextNode(snapshot));
                item.ItemNameProperty.AddSourceTextNode(snapshot);
                item.IsEmittable = false;
                item.IsImport = false;

                item.References.AddRange(context.ReferenceParser.ParseReferences(item, item.TemplateIdOrPathProperty));

                foreach (var sharedField in serializedItem.SharedFields)
                {
                    var field = context.Factory.Field(item).With(new StringTextNode(sharedField.Value, snapshot));

                    if (!string.IsNullOrEmpty(sharedField.NameHint))
                    {
                        field.FieldName = sharedField.NameHint;
                    }

                    var value = sharedField.Value;
                    if (sharedField.FieldType == "tree list")
                    {
                        value = value.Replace("\r\n", "|");
                    }

                    field.FieldId = sharedField.FieldId;
                    field.Value = value;

                    item.Fields.Add(field);

                    item.References.AddRange(context.ReferenceParser.ParseReferences(field));
                }

                foreach (var version in serializedItem.Versions)
                {
                    foreach (var versionedField in version.Fields)
                    {
                        var field = context.Factory.Field(item).With(new StringTextNode(versionedField.Value, snapshot));

                        if (!string.IsNullOrEmpty(versionedField.NameHint))
                        {
                            field.FieldName = versionedField.NameHint;
                        }

                        var value = versionedField.Value;
                        if (versionedField.FieldType == "tree list")
                        {
                            value = value.Replace("\r\n", "|");
                        }

                        field.FieldId = versionedField.FieldId;
                        field.Value = value;
                        field.Language = version.Language.ToString();
                        field.Version = version.VersionNumber;

                        item.Fields.Add(field);

                        item.References.AddRange(context.ReferenceParser.ParseReferences(field));
                    }
                }

                context.Project.AddOrMerge(item);
            }
        }
    }
}
