// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Parsing;

namespace Sitecore.Pathfinder.Languages.Renderings
{
    public abstract class RenderingParser : ParserBase
    {
        protected RenderingParser([NotNull] string fileExtension, [NotNull] string templateIdOrPath) : base(Constants.Parsers.Renderings)
        {
            FileExtension = fileExtension;
            TemplateIdOrPath = templateIdOrPath;
        }

        protected RenderingParser([NotNull] string fileExtension, [NotNull] string templateIdOrPath, double priority) : base(priority)
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
            if (string.IsNullOrEmpty(context.FilePath) && string.IsNullOrEmpty(context.ItemPath))
            {
                return false;
            }

            var extension = PathHelper.GetExtension(context.Snapshot.SourceFile.AbsoluteFileName);
            return string.Equals(extension, FileExtension, StringComparison.OrdinalIgnoreCase);
        }

        public override void Parse(IParseContext context)
        {
            var rendering = context.Factory.Rendering(context.Project, context.Snapshot, context.DatabaseName, context.ItemPath, context.ItemName, context.FilePath, TemplateIdOrPath);
            context.Project.AddOrMerge(rendering);

            var contents = context.Snapshot.SourceFile.ReadAsText();

            var placeholders = GetPlaceholders(contents);

            rendering.Placeholders.AddRange(placeholders);

            context.Project.Ducats += 100;
        }

        [NotNull, ItemNotNull]
        protected abstract IEnumerable<string> GetPlaceholders([NotNull] string contents);
    }
}
