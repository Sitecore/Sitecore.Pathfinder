// // © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Composition;
using System.Linq;
using Sitecore.Pathfinder.Compiling.Compilers;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Snapshots;
using Sitecore.Pathfinder.Text;

namespace Sitecore.Pathfinder.Languages.Renderings
{
    [Export(typeof(ICompiler)), Shared]
    public class RenderingCompiler : CompilerBase
    {
        [ImportingConstructor]
        public RenderingCompiler([NotNull] IFactory factory) : base(1000)
        {
            Factory = factory;
        }

        [NotNull]
        protected IFactory Factory { get; }

        public override bool CanCompile(ICompileContext context, IProjectItem projectItem) => projectItem is Rendering;

        public override void Compile(ICompileContext context, IProjectItem projectItem)
        {
            var rendering = projectItem as Rendering;
            Assert.Cast(rendering, nameof(rendering));

            if (string.IsNullOrEmpty(rendering.ItemPath))
            {
                return;
            }

            var project = rendering.Project;
            var snapshot = rendering.Snapshot;
            var snapshotTextNode = Factory.SnapshotTextNode(snapshot);
            var guid = StringHelper.GetGuid(project, rendering.ItemPath);

            var item = Factory.Item(rendering.Database, guid, rendering.ItemName, rendering.ItemPath, rendering.TemplateIdOrPath).With(snapshotTextNode);
            item.OverwriteWhenMerging = true;
            item.ItemNameProperty.AddSourceTextNode(new FileNameTextNode(rendering.ItemName, snapshot));

            var field = Factory.Field(item, "Path", rendering.FilePath).With(snapshotTextNode);
            field.ValueProperty.Flags = SourcePropertyFlags.IsFileName;
            item.Fields.Add(field);
            item.References.Add(Factory.FileReference(item, field.ValueProperty, field.Value));

            if (rendering.Placeholders.Any() && rendering.Extension != ".aspx")
            {
                var placeholdersField = Factory.Field(item, "Place Holders", string.Join(",", rendering.Placeholders));
                item.Fields.Add(placeholdersField);
            }

            var addedItem = context.Project.AddOrMerge(item);
            rendering.RenderingItemUri = addedItem.Uri;
        }
    }
}
