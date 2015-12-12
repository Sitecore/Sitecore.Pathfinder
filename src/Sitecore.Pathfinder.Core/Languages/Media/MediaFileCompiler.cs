// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.IO;
using System.Linq;
using Sitecore.Pathfinder.Compiling.Compilers;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Snapshots;
using Sitecore.Pathfinder.Text;

namespace Sitecore.Pathfinder.Languages.Media
{
    public class MediaFileCompiler : CompilerBase
    {
        public MediaFileCompiler() : base(1000)
        {
        }

        public override bool CanCompile(ICompileContext context, IProjectItem projectItem)
        {
            return projectItem is MediaFile;
        }

        public override void Compile(ICompileContext context, IProjectItem projectItem)
        {
            var mediaFile = projectItem as MediaFile;
            Assert.Cast(mediaFile, nameof(mediaFile));

            var extension = Path.GetExtension(mediaFile.Snapshots.First().SourceFile.AbsoluteFileName).TrimStart('.').ToLowerInvariant();

            var templateIdOrPath = context.Configuration.GetString(Constants.Configuration.BuildProjectMediaTemplate + ":" + extension, "/sitecore/templates/System/Media/Unversioned/File");

            var project = mediaFile.Project;
            var snapshot = mediaFile.Snapshots.First();

            var guid = StringHelper.GetGuid(project, mediaFile.ItemPath);
            var item = context.Factory.Item(project, new SnapshotTextNode(snapshot), guid, mediaFile.DatabaseName, mediaFile.ItemName, mediaFile.ItemPath, string.Empty);
            item.ItemNameProperty.AddSourceTextNode(new FileNameTextNode(mediaFile.ItemName, snapshot));
            item.TemplateIdOrPathProperty.SetValue(templateIdOrPath);
            item.IsEmittable = false;
            item.OverwriteWhenMerging = true;
            item.MergingMatch = MergingMatch.MatchUsingSourceFile;

            var addedItem = project.AddOrMerge(item);
            mediaFile.MediaItemUri = addedItem.Uri;
        }
    }
}
