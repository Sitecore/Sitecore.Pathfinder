// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.IO;
using System.Linq;
using Rainbow.Storage.Yaml;
using Sitecore.Pathfinder.Compiling.Compilers;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Unicorn.Languages.Unicorn
{
    public class UnicornFileCompiler : CompilerBase
    {
        public UnicornFileCompiler() : base(1000)
        {
        }

        public override bool CanCompile(ICompileContext context, IProjectItem projectItem)
        {
            return projectItem is UnicornFile;
        }

        public override void Compile(ICompileContext context, IProjectItem projectItem)
        {
            var unicornFile = projectItem as UnicornFile;
            Assert.Cast(unicornFile, nameof(unicornFile));

            var snapshot = unicornFile.Snapshots.First();

            // todo: use real Unicorn configuration instead of hacking it
            var formatter = new YamlSerializationFormatter(null, new AllFieldFilter());
            using (var stream = new FileStream(snapshot.SourceFile.AbsoluteFileName, FileMode.Open))
            {
                var serializedItem = formatter.ReadSerializedItem(stream, unicornFile.ShortName);

                var guid = serializedItem.Id;
                var databaseName = serializedItem.DatabaseName ?? unicornFile.DatabaseName;
                var itemIdOrPath = serializedItem.Path;
                var itemName = serializedItem.Name;
                var templateIdOrPath = serializedItem.TemplateId.Format();

                var item = context.Factory.Item(unicornFile.Project, snapshot, guid, databaseName, itemName, itemIdOrPath, templateIdOrPath);
                item.ItemNameProperty.AddSourceTextNode(snapshot);
                item.IsEmittable = false;
                item.IsImport = false;

                item.References.AddRange(context.ReferenceParser.ParseReferences(item, item.TemplateIdOrPathProperty));

                foreach (var sharedField in serializedItem.SharedFields)
                {
                    var field = context.Factory.Field(item);

                    if (!string.IsNullOrEmpty(sharedField.NameHint))
                    {
                        field.FieldName = sharedField.NameHint;
                    }

                    field.FieldId = sharedField.FieldId;
                    field.Value = sharedField.Value;

                    context.ReferenceParser.ParseReferences(item, field.ValueProperty);
                }

                foreach (var version in serializedItem.Versions)
                {
                    foreach (var versionedField in version.Fields)
                    {
                        var field = context.Factory.Field(item);

                        if (!string.IsNullOrEmpty(versionedField.NameHint))
                        {
                            field.FieldName = versionedField.NameHint;
                        }

                        field.FieldId = versionedField.FieldId;
                        field.Value = versionedField.Value;
                        field.Language = version.Language.ToString();
                        field.Version = version.VersionNumber;

                        context.ReferenceParser.ParseReferences(item, field.ValueProperty);
                    }
                }

                unicornFile.Project.AddOrMerge(item);
            }
        }
    }
}
