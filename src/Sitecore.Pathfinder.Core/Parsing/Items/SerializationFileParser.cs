// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.ComponentModel.Composition;

namespace Sitecore.Pathfinder.Parsing.Items
{
    [Export(typeof(IParser))]
    public class SerializationFileParser : ParserBase
    {
        private const string FileExtension = ".item";

        public SerializationFileParser() : base(Constants.Parsers.ContentFiles)
        {
        }

        public override bool CanParse(IParseContext context)
        {
            return context.Snapshot.SourceFile.AbsoluteFileName.EndsWith(FileExtension, StringComparison.OrdinalIgnoreCase);
        }

        public override void Parse(IParseContext context)
        {
            var serializationFile = context.Factory.SerializationFile(context.Project, context.Snapshot, context.FilePath);
            context.Project.AddOrMerge(serializationFile);
        }
    }
}
