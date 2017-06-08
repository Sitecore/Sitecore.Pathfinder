// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Composition;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Parsing;

namespace Sitecore.Pathfinder.Languages.Content
{
    [Export(typeof(IParser)), Shared]
    public class ContentFileParser : ParserBase
    {
        [ImportingConstructor]
        public ContentFileParser([NotNull] IFactory factory) : base(Constants.Parsers.ContentFiles)
        {
            Factory = factory;
        }

        [NotNull]
        protected IFactory Factory { get; }

        public override bool CanParse(IParseContext context) => !context.IsParsed && !string.IsNullOrEmpty(context.FilePath);

        public override void Parse(IParseContext context)
        {
            var contentFile = Factory.ContentFile(context.Project, context.Snapshot, context.FilePath);
            context.Project.AddOrMerge(contentFile);
        }
    }
}
