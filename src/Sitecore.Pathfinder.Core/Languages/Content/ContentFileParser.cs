// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Composition;
using Sitecore.Pathfinder.Configuration.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Parsing;

namespace Sitecore.Pathfinder.Languages.Content
{
    [Export(typeof(IParser)), Shared]
    public class ContentFileParser : ParserBase
    {
        [ImportingConstructor]
        public ContentFileParser([NotNull] IConfiguration configuration) : base(Constants.Parsers.ContentFiles)
        {
            PathMatcher = new PathMatcher(configuration.GetString(Constants.Configuration.Items.Include), configuration.GetString(Constants.Configuration.Items.Exclude));
        }

        [NotNull]
        protected PathMatcher PathMatcher { get; }

        public override bool CanParse(IParseContext context)
        {
            // exclude items files
            var fileName = context.Snapshot.SourceFile.AbsoluteFileName;
            return !string.IsNullOrEmpty(context.FilePath) && !PathMatcher.IsMatch(fileName);
        }

        public override void Parse(IParseContext context)
        {
            var contentFile = context.Factory.ContentFile(context.Project, context.Snapshot, context.FilePath);
            context.Project.AddOrMerge(contentFile);
        }
    }
}
