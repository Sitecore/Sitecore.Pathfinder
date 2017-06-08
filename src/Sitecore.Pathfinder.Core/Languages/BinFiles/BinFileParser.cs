// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Composition;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Parsing;

namespace Sitecore.Pathfinder.Languages.BinFiles
{
    [Export(typeof(IParser)), Shared]
    public class BinFileParser : ParserBase
    {
        private const string FileExtension = ".dll";

        [ImportingConstructor]
        public BinFileParser([NotNull] IFactory factory) : base(Constants.Parsers.BinFiles)
        {
            Factory = factory;
        }

        [NotNull]
        protected IFactory Factory { get; }

        public override bool CanParse(IParseContext context)
        {
            if (string.IsNullOrEmpty(context.FilePath))
            {
                return false;
            }

            return context.Snapshot.SourceFile.AbsoluteFileName.EndsWith(FileExtension, StringComparison.OrdinalIgnoreCase);
        }

        public override void Parse(IParseContext context)
        {
            var binFile = Factory.BinFile(context.Project, context.Snapshot, context.FilePath);
            context.Project.AddOrMerge(binFile);
        }
    }
}
