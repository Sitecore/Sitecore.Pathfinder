// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Linq;
using Sitecore.Pathfinder.Compiling.Compilers;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.References;
using Sitecore.Pathfinder.Snapshots;
using Sitecore.Pathfinder.Text;

namespace Sitecore.Pathfinder.Languages.Renderings
{
    public class RenderingCompiler : CompilerBase
    {
        public RenderingCompiler() : base(1000)
        {
        }

        public override bool CanCompile(ICompileContext context, IProjectItem projectItem)
        {
            return projectItem is Rendering;
        }

        public override void Compile(ICompileContext context, IProjectItem projectItem)
        {
            var rendering = projectItem as Rendering;
            Assert.Cast(rendering, nameof(rendering));

            var project = rendering.Project;
            var snapshot = rendering.Snapshots.First();
            var snapshotTextNode = new SnapshotTextNode(snapshot);
            var guid = StringHelper.GetGuid(project, rendering.ItemPath);
            var item = context.Factory.Item(project, snapshotTextNode, guid, rendering.DatabaseName, rendering.ItemName, rendering.ItemPath, rendering.TemplateIdOrPath);
            item.ItemNameProperty.AddSourceTextNode(new FileNameTextNode(rendering.ItemName, snapshot));
            item.OverwriteWhenMerging = true;

            var field = context.Factory.Field(item, snapshotTextNode, "Path", rendering.FilePath);
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
