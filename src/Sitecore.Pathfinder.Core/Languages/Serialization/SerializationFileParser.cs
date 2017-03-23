// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Composition;
using Sitecore.Pathfinder.Parsing;

namespace Sitecore.Pathfinder.Languages.Serialization
{
    [Export(typeof(IParser)), Shared]
    public class SerializationFileParser : ParserBase
    {
        private const string FileExtension = ".item";

        public SerializationFileParser() : base(Constants.Parsers.ContentFiles)
        {
        }

        public override bool CanParse(IParseContext context)
        {
            if (string.IsNullOrEmpty(context.ItemPath))
            {
                return false;
            }

            return context.Snapshot.SourceFile.AbsoluteFileName.EndsWith(FileExtension, StringComparison.OrdinalIgnoreCase);
        }

        public override void Parse(IParseContext context)
        {
            var serializationFile = context.Factory.SerializationFile(context.Project, context.Snapshot, context.FilePath);
            context.Project.AddOrMerge(serializationFile);
        }
    }
}
