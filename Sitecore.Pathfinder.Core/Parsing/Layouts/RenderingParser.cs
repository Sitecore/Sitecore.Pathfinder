// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Documents;
using Sitecore.Pathfinder.Projects;

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
            var contents = context.Snapshot.SourceFile.ReadAsText();
            var placeHolders = GetPlaceholders(contents);
            var path = context.FilePath;
            var snapshotTextNode = new SnapshotTextNode(context.Snapshot);

            var item = context.Factory.Item(context.Project, context.ItemPath, snapshotTextNode, context.DatabaseName, context.ItemName, context.ItemPath, TemplateIdOrPath);
            item.ItemName.Source = new FileNameTextNode(context.ItemName, context.Snapshot);
            item.OverwriteWhenMerging = true;

            var field = context.Factory.Field(item, "Path", string.Empty, 0, path);
            item.Fields.Add(field);

            // todo: make this configurable
            if (string.Compare(context.DatabaseName, "core", StringComparison.OrdinalIgnoreCase) == 0)
            {
                field = context.Factory.Field(item, "Place Holders", string.Empty, 0, string.Join(",", placeHolders));
                item.Fields.Add(field);
            }

            item.References.Add(context.Factory.FileReference(item, new Attribute<string>(snapshotTextNode, SourceFlags.IsFileName), path));

            item = context.Project.AddOrMerge(context, item);

            var rendering = context.Factory.Rendering(context.Project, context.Snapshot, context.FilePath, item);
            context.Project.AddOrMerge(context, rendering);

            context.Project.Ducats += 100;
        }

        [NotNull]
        protected abstract IEnumerable<string> GetPlaceholders([NotNull] string contents);
    }
}
