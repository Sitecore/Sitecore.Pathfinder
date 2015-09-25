// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;

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
            var rendering = context.Factory.Rendering(context.Project, context.Snapshot, context.DatabaseName, context.ItemPath, context.ItemName, context.FilePath, TemplateIdOrPath);
            context.Project.AddOrMerge(rendering);

            // todo: make this configurable
            if (string.Compare(rendering.DatabaseName, "core", StringComparison.OrdinalIgnoreCase) == 0)
            {
                var contents = context.Snapshot.SourceFile.ReadAsText();

                var placeholders = GetPlaceholders(contents);

                rendering.Placeholders.AddRange(placeholders);
            }

            context.Project.Ducats += 100;
        }

        [NotNull]
        [ItemNotNull]
        protected abstract IEnumerable<string> GetPlaceholders([NotNull] string contents);
    }
}
