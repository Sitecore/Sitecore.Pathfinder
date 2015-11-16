// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.IO;
using System.Linq;
using Rainbow.Storage.Yaml;
using Sitecore.Pathfinder.Compiling.Compilers;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Unicorn.Languages.Unicorn
{
    public class UnicornFileCompiler : CompilerBase
    {
        public override bool CanCompile(ICompileContext context, IProjectItem projectItem)
        {
            return projectItem is UnicornFile;
        }

        public override void Compile(ICompileContext context, IProjectItem projectItem)
        {
            var unicornFile = projectItem as UnicornFile;
            if (unicornFile == null)
            {
                return;
            }

            var formatter = new YamlSerializationFormatter(null, null);

            using (var stream = new FileStream(unicornFile.FilePath, FileMode.Open))
            {
                var serializedItem = formatter.ReadSerializedItem(stream, unicornFile.ShortName);

                var snapshot = unicornFile.Snapshots.First();
                var snapshotTextNode = new SnapshotTextNode(snapshot);

                var guid = serializedItem.Id;
                var databaseName = serializedItem.DatabaseName;
                var itemIdOrPath = serializedItem.Path;
                var itemName = serializedItem.Name;
                var templateIdOrPath = serializedItem.TemplateId.Format();

                var item = context.Factory.Item(unicornFile.Project, guid, snapshotTextNode, databaseName, itemName, itemIdOrPath, templateIdOrPath);
                item.ItemNameProperty.AddSourceTextNode(new FileNameTextNode(itemName, snapshot));
                item.IsEmittable = false;
                item.IsExtern = false;

                item.References.AddRange(context.ReferenceParser.ParseReferences(item, item.TemplateIdOrPathProperty));

                foreach (var sharedField in serializedItem.SharedFields)
                {
                    var field = context.Factory.Field(item, TextNode.Empty);
                    field.FieldNameProperty.SetValue(sharedField.NameHint);
                    field.ValueProperty.SetValue(sharedField.Value);

                    context.ReferenceParser.ParseReferences(item, field.ValueProperty);
                }

                foreach (var itemVersion in serializedItem.Versions)
                {
                    foreach (var fieldVersion in itemVersion.Fields)
                    {
                        var field = context.Factory.Field(item, TextNode.Empty);
                        field.FieldNameProperty.SetValue(fieldVersion.NameHint);
                        field.ValueProperty.SetValue(fieldVersion.Value);
                        field.LanguageProperty.SetValue(itemVersion.Language.ToString());
                        field.VersionProperty.SetValue(itemVersion.VersionNumber);

                        context.ReferenceParser.ParseReferences(item, field.ValueProperty);
                    }
                }

                unicornFile.Project.AddOrMerge(item);
            }
        }
    }
}
