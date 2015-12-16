// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.IO;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Parsing;

namespace Sitecore.Pathfinder.Languages.Content
{
    public class ContentFileParser : ParserBase
    {
        public ContentFileParser() : base(Constants.Parsers.ContentFiles)
        {
        }

        public override bool CanParse(IParseContext context)
        {
            // todo: potential incorrect as an extension might match part of another extension
            var fileExtensions = context.Configuration.GetString(Constants.Configuration.MappingContentFiles);
            var extension = Path.GetExtension(context.Snapshot.SourceFile.AbsoluteFileName);

            return fileExtensions.IndexOf(extension, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public override void Parse(IParseContext context)
        {
            var contentFile = context.Factory.ContentFile(context.Project, context.Snapshot, context.FilePath);
            context.Project.AddOrMerge(contentFile);
        }
    }
}
