// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Composition;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Parsing;

namespace Sitecore.Pathfinder.Languages.Content
{
    [Export(typeof(IParser)), Shared]
    public class ContentFileParser : ParserBase
    {
        [ImportingConstructor]
        public ContentFileParser() : base(Constants.Parsers.ContentFiles)
        {
        }

        [NotNull]
        protected PathMatcher PathMatcher { get; }

        public override bool CanParse(IParseContext context)
        {
            return !context.IsParsed && !string.IsNullOrEmpty(context.FilePath);
        }

        public override void Parse(IParseContext context)
        {
            var contentFile = context.Factory.ContentFile(context.Project, context.Snapshot, context.FilePath);
            context.Project.AddOrMerge(contentFile);
        }
    }
}
