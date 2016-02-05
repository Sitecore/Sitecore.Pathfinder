// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Pathfinder.Parsing;

namespace Sitecore.Pathfinder.PageHtml
{
    public class PageHtmlParser : ParserBase
    {
        private const string FileExtension = ".page.html";

        public PageHtmlParser() : base(Constants.Parsers.Renderings - 10)
        {
        }

        public override bool CanParse(IParseContext context)
        {
            return context.Snapshot.SourceFile.AbsoluteFileName.EndsWith(FileExtension, StringComparison.OrdinalIgnoreCase);
        }

        public override void Parse(IParseContext context)
        {
            var pageHtmlFile = new PageHtmlFile(context.Project, context.Snapshot, context.FilePath);
            context.Project.AddOrMerge(pageHtmlFile);
        }
    }
}
