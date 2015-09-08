// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Projects.References;
using Sitecore.Pathfinder.Snapshots;
using Sitecore.Pathfinder.Text;

namespace Sitecore.Pathfinder.Parsing.Layouts
{
    public abstract class RenderingParser : ParserBase
    {
        protected RenderingParser([NotNull] string fileExtension, [NotNull] string templateIdOrPath) : base(Constants.Parsers.Renderings)
        {
            FileExtension = fileExtension;
            TemplateIdOrPath = templateIdOrPath;
        }

        [NotNull]
        public string TemplateIdOrPath { get; }

        [NotNull]
        protected string FileExtension { get; }

        public override bool CanParse(IParseContext context)
        {
            return context.Snapshot.SourceFile.FileName.EndsWith(FileExtension, StringComparison.OrdinalIgnoreCase);
        }

        public override void Parse(IParseContext context)
        {
            var path = context.FilePath;
            var snapshotTextNode = new SnapshotTextNode(context.Snapshot);

            var guid = StringHelper.GetGuid(context.Project, context.ItemPath);
            var item = context.Factory.Item(context.Project, guid, snapshotTextNode, context.DatabaseName, context.ItemName, context.ItemPath, TemplateIdOrPath);
            item.ItemNameProperty.AddSourceTextNode(new FileNameTextNode(context.ItemName, context.Snapshot));
            item.OverwriteWhenMerging = true;

            var field = context.Factory.Field(item, snapshotTextNode, "Path", path);
            item.Fields.Add(field);
            item.References.Add(new FileReference(item, field.ValueProperty));

            // todo: make this configurable
            if (string.Compare(context.DatabaseName, "core", StringComparison.OrdinalIgnoreCase) == 0)
            {
                AddPlaceholdersField(context, item);
            }

            var addedItem = context.Project.AddOrMerge(context, item);

            var rendering = context.Factory.Rendering(context.Project, context.Snapshot, context.FilePath, addedItem);
            context.Project.AddOrMerge(context, rendering);

            context.Project.Ducats += 100;
        }

        [NotNull]
        protected abstract IEnumerable<string> GetPlaceholders([NotNull] string contents);

        private void AddPlaceholdersField([NotNull] IParseContext context, [NotNull] Item item)
        {
            var contents = context.Snapshot.SourceFile.ReadAsText();
            var placeHolders = GetPlaceholders(contents);

            var field = context.Factory.Field(item, TextNode.Empty, "Place Holders", string.Join(",", placeHolders));
            item.Fields.Add(field);
        }
    }
}
