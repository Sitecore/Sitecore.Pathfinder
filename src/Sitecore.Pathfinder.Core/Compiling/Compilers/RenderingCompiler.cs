// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using System.Linq;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Layouts;
using Sitecore.Pathfinder.Projects.References;
using Sitecore.Pathfinder.Snapshots;
using Sitecore.Pathfinder.Text;

namespace Sitecore.Pathfinder.Compiling.Compilers
{
    [Export(typeof(ICompiler))]
    public class RenderingCompiler : CompilerBase
    {
        public override bool CanCompile(ICompileContext context, IProjectItem projectItem)
        {
            return projectItem is Rendering;
        }

        public override void Compile(ICompileContext context, IProjectItem projectItem)
        {
            var rendering = projectItem as Rendering;
            if (rendering == null)
            {
                return;
            }

            var project = rendering.Project;
            var snapshot = rendering.Snapshots.First();
            var path = rendering.FilePath;
            var snapshotTextNode = new SnapshotTextNode(snapshot);
            var guid = StringHelper.GetGuid(project, rendering.ItemPath);
            var item = context.Factory.Item(project, guid, snapshotTextNode, rendering.DatabaseName, rendering.ItemName, rendering.ItemPath, rendering.TemplateIdOrPath);
            item.ItemNameProperty.AddSourceTextNode(new FileNameTextNode(rendering.ItemName, snapshot));
            item.OverwriteWhenMerging = true;

            var field = context.Factory.Field(item, snapshotTextNode, "Path", path);
            field.ValueProperty.Flags = SourcePropertyFlags.IsFileName;
            item.Fields.Add(field);
            item.References.Add(new FileReference(item, field.ValueProperty));

            if (rendering.Placeholders.Any())
            {
                var placeholdersField = context.Factory.Field(item, TextNode.Empty, "Place Holders", string.Join(",", rendering.Placeholders));
                item.Fields.Add(placeholdersField);
            }

            var addedItem = rendering.Project.AddOrMerge(item);
            rendering.RenderingItemUri = addedItem.Uri;
        }
    }
}
