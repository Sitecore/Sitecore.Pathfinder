// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Composition;
using System.IO;
using Sitecore.Pathfinder.Configuration.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Emitting;
using Sitecore.Pathfinder.Emitting.Emitters;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Items;

namespace Sitecore.Pathfinder.Languages.Unicorn
{
    [Export(typeof(IProjectEmitter)), Shared]
    public class UnicornProjectEmitter : DirectoryProjectEmitterBase
    {
        [ImportingConstructor]
        public UnicornProjectEmitter([NotNull] IConfiguration configuration, [NotNull] ITraceService traceService, [ItemNotNull, NotNull, ImportMany] IEnumerable<IEmitter> emitters, [NotNull] IFileSystemService fileSystem) : base(configuration, traceService, emitters, fileSystem)
        {
        }

        public override bool CanEmit(string format)
        {
            return string.Equals(format, "unicorn", StringComparison.OrdinalIgnoreCase);
        }

        public override void Emit(IEmitContext context, IProject project)
        {
            base.Emit(context, project);
            MirrorItems(context);
        }

        public override void EmitItem(IEmitContext context, Item item)
        {
            context.Trace.TraceInformation(Msg.I1011, "Publishing", item.ItemIdOrPath);

            var outputDirectory = PathHelper.Combine(OutputDirectory, Configuration.GetString(Constants.Configuration.Output.Unicorn.ItemsDirectory));
            var destinationFileName = PathHelper.Combine(outputDirectory, PathHelper.NormalizeFilePath(item.ItemIdOrPath).TrimStart('\\'));

            destinationFileName += ".yml";

            FileSystem.CreateDirectoryFromFileName(destinationFileName);

            using (var stream = new FileStream(destinationFileName, FileMode.Create))
            {
                using (var writer = new StreamWriter(stream))
                {
                    item.WriteAsUnicornYaml(writer);
                }
            }
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

            context.Trace.TraceInformation(Msg.I1011, "Mirroring item files", sourceDirectory + " => " + destinationDirectory);

            FileSystem.CreateDirectory(destinationDirectory);
            FileSystem.Mirror(sourceDirectory, destinationDirectory);
        }
    }
}
