// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.ComponentModel.Composition;
using Sitecore.Pathfinder.Parsing;

namespace Sitecore.Pathfinder.Languages.BinFiles
{
    public class BinFileParser : ParserBase
    {
        private const string FileExtension = ".dll";

        public BinFileParser() : base(Constants.Parsers.BinFiles)
        {
        }

        public override bool CanParse(IParseContext context)
        {
            return context.Snapshot.SourceFile.AbsoluteFileName.EndsWith(FileExtension, StringComparison.OrdinalIgnoreCase);
        }

        public override void Parse(IParseContext context)
        {
            var binFile = context.Factory.BinFile(context.Project, context.Snapshot, context.FilePath);
            context.Project.AddOrMerge(binFile);
        }
    }
}
