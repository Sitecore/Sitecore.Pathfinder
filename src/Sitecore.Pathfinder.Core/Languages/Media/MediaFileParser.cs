using System.Composition;
using System.IO;
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
        public MediaFileParser([NotNull] IConfiguration configuration) : base(Constants.Parsers.Media)
        {
            Configuration = configuration;
        }

        [NotNull]
        protected IConfiguration Configuration { get; }

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
            var mediaFile = context.Factory.MediaFile(context.Database, context.Snapshot, context.ItemName, context.ItemPath, context.FilePath);
            mediaFile.UploadMedia = context.UploadMedia;
            context.Project.AddOrMerge(mediaFile);

            context.Project.Ducats += 100;
        }
    }
}
