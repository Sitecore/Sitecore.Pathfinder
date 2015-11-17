// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Pathfinder.Parsing;

namespace Sitecore.Pathfinder.Unicorn.Languages.Unicorn
{
    public class UnicornFileParser : ParserBase
    {
        private const string FileExtension = ".yml";

        public UnicornFileParser() : base(Constants.Parsers.Items)
        {
        }

        public override bool CanParse(IParseContext context)
        {
            return context.Snapshot.SourceFile.AbsoluteFileName.EndsWith(FileExtension, StringComparison.OrdinalIgnoreCase);
        }

        public override void Parse(IParseContext context)
        {
            var unicornFile = new UnicornFile(context.Project, context.Snapshot, context.FilePath, context.DatabaseName);
            context.Project.AddOrMerge(unicornFile);
        }
    }
}
