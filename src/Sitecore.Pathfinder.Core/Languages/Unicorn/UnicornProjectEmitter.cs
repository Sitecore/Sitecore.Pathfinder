// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Composition;
using System.IO;
using Sitecore.Pathfinder.Compiling.Pipelines.CompilePipelines;
using Sitecore.Pathfinder.Configuration.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Emitting;
using Sitecore.Pathfinder.Emitting.Emitters;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Languages.Media;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Items;

namespace Sitecore.Pathfinder.Languages.Unicorn
{
    [Export(typeof(IProjectEmitter)), Shared]
    public class UnicornProjectEmitter : DirectoryProjectEmitterBase
    {
        [ImportingConstructor]
        public UnicornProjectEmitter([NotNull] IConfiguration configuration, [NotNull] ITraceService trace, [ItemNotNull, NotNull, ImportMany] IEnumerable<IEmitter> emitters, [NotNull] IFileSystemService fileSystem) : base(configuration, trace, emitters, fileSystem)
        {
        }

        public override bool CanEmit(string format)
        {
            return string.Equals(format, "unicorn", StringComparison.OrdinalIgnoreCase);
        }

        public override void Emit(IEmitContext context, IProject project)
        {
            base.Emit(context, project);

            if (Configuration.GetBool(Constants.Configuration.Output.Unicorn.EmitMissingItems))
            {
                EmitImportedItems(context, project);
            }

            MirrorItems(context);
        }

        public override void EmitItem(IEmitContext context, Item item)
        {
            var sourceBag = item as ISourcePropertyBag;

            if (!item.IsEmittable && sourceBag.GetValue<string>("__origin_reason") != nameof(CreateItemsFromTemplates))
            {
                return;
            }

            Trace.TraceInformation(Msg.I1011, "Publishing", item.ItemIdOrPath);

            var destinationFileName = GetItemFileName(item.DatabaseName, item.ItemIdOrPath);

            FileSystem.CreateDirectoryFromFileName(destinationFileName);

            using (var stream = new FileStream(destinationFileName, FileMode.Create))
            {
                using (var writer = new StreamWriter(stream))
                {
                    item.WriteAsUnicornYaml(writer);
                }
            }
        }

        protected virtual void EmitImportedItems([NotNull] IEmitContext context, [NotNull] IProject project)
        {
            // Unicorn requires item files for the entire path, so fill in the gaps

            foreach (var item in project.Items)
            {
                var parentItem = item.GetParent();
                if (parentItem != null && parentItem.IsEmittable)
                {
                    continue;
                }

                var itemPath = PathHelper.GetItemParentPath(item.ItemIdOrPath);

                while (!string.IsNullOrEmpty(itemPath))
                {
                    var fileName = GetItemFileName(item.DatabaseName, itemPath);
                    if (!FileSystem.FileExists(fileName))
                    {
                        var i = item.Database.GetItem(itemPath);
                        if (i == null)
                        {
                            Trace.TraceError(Msg.E1045, "Required item not found for Unicorn serialization", itemPath);
                            break;
                        }

                        EmitItem(context, i);
                    }

                    itemPath = PathHelper.GetItemParentPath(itemPath);
                }
            }
        }

        protected override void EmitMediaFile(IEmitContext context, MediaFile mediaFile)
        {
            if (!mediaFile.IsEmittable)
            {
                return;
            }

            var item = context.Project.Indexes.FindQualifiedItem<Item>(mediaFile.MediaItemUri);
            if (item == null)
            {
                Trace.TraceInformation(Msg.E1047, "No media item - skipping", mediaFile.Snapshot.SourceFile);
                return;
            }

            Trace.TraceInformation(Msg.I1011, "Publishing", item.ItemIdOrPath);

            var destinationFileName = GetItemFileName(item.DatabaseName, item.ItemIdOrPath);

            FileSystem.CreateDirectoryFromFileName(destinationFileName);

            using (var stream = new FileStream(destinationFileName, FileMode.Create))
            {
                using (var writer = new StreamWriter(stream))
                {
                    item.WriteAsUnicornYaml(writer);
                }
            }
        }

        [NotNull]
        protected virtual string GetItemFileName([NotNull] string databaseName, [NotNull] string itemIdOrPath)
        {
            var outputDirectory = PathHelper.Combine(OutputDirectory, Configuration.GetString(Constants.Configuration.Output.Unicorn.ItemsDirectory));
            var destinationFileName = PathHelper.Combine(outputDirectory, databaseName + "\\" + PathHelper.NormalizeFilePath(itemIdOrPath).TrimStart('\\')) + ".yml";

            return destinationFileName;
        }

        protected virtual void MirrorItems([NotNull] IEmitContext context)
        {
            if (!Configuration.GetBool(Constants.Configuration.Output.Unicorn.MirrorItems))
            {
                return;
            }

            var sourceDirectory = PathHelper.Combine(OutputDirectory, Configuration.GetString(Constants.Configuration.Output.Unicorn.ItemsDirectory));
            sourceDirectory = PathHelper.Combine(sourceDirectory, PathHelper.NormalizeFilePath(Configuration.GetString(Constants.Configuration.Output.Unicorn.MirrorItemsSourceDirectory)).TrimStart('\\'));

            var destinationDirectory = Configuration.GetString(Constants.Configuration.Output.Unicorn.UnicornRootPath);

            Trace.TraceInformation(Msg.E1046, "Mirroring item files", sourceDirectory + " => " + destinationDirectory);

            FileSystem.CreateDirectory(destinationDirectory);
            FileSystem.Mirror(sourceDirectory, destinationDirectory);
        }
    }
}
