// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Composition;
using System.IO;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Configuration.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Parsing;

namespace Sitecore.Pathfinder.Languages.Media
{
    [Export(typeof(IParser)), Shared]
    public class MediaFileParser : ParserBase
    {
        [ImportingConstructor]
        public MediaFileParser([NotNull] IConfiguration configuration, [NotNull] IFactory factory) : base(Constants.Parsers.Media)
        {
            Configuration = configuration;
            Factory = factory;
        }

        [NotNull]
        protected IConfiguration Configuration { get; }

        [NotNull]
        protected IFactory Factory { get; }

        public override bool CanParse(IParseContext context)
        {
            if (string.IsNullOrEmpty(context.ItemPath))
            {
                return false;
            }

            var extension = Path.GetExtension(context.Snapshot.SourceFile.AbsoluteFileName).TrimStart('.').ToLowerInvariant();
            var templateIdOrPath = Configuration.GetString(Constants.Configuration.BuildProject.MediaTemplate + ":" + extension);

            return !string.IsNullOrEmpty(templateIdOrPath);
        }

        public override void Parse(IParseContext context)
        {
            var mediaFile = Factory.MediaFile(context.Database, context.Snapshot, context.ItemName, context.ItemPath, context.FilePath);
            mediaFile.UploadMedia = context.UploadMedia;
            context.Project.AddOrMerge(mediaFile);

            context.Project.Ducats += 100;
        }
    }
}
