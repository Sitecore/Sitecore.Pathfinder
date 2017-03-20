// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Composition;
using Sitecore.Pathfinder.Parsing;

namespace Sitecore.Pathfinder.Languages.Content
{
    [Export(typeof(IParser)), Shared]
    public class ContentFileParser : ParserBase
    {
        public ContentFileParser() : base(Constants.Parsers.ContentFiles)
        {
        }

        public override bool CanParse(IParseContext context)
        {
            return !string.IsNullOrEmpty(context.FilePath);
        }

        public override void Parse(IParseContext context)
        {
            var contentFile = context.Factory.ContentFile(context.Project, context.Snapshot, context.FilePath);
            context.Project.AddOrMerge(contentFile);
        }
    }
}
