// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Composition;
using Sitecore.Pathfinder.Configuration.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Languages.Media;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Files;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Projects.Templates;

namespace Sitecore.Pathfinder.Emitting.Emitters
{
    public abstract class DirectoryProjectEmitterBase : ProjectEmitterBase
    {
        protected DirectoryProjectEmitterBase([NotNull] IConfiguration configuration, [NotNull] ITraceService trace, [ItemNotNull, NotNull, ImportMany] IEnumerable<IEmitter> emitters, [NotNull] IFileSystem fileSystem) : base(configuration, trace, emitters)
        {
            FileSystem = fileSystem;
            OutputDirectory = PathHelper.Combine(Configuration.GetProjectDirectory(), Configuration.GetString(Constants.Configuration.Output.Directory));
        }

        [NotNull]
        public IFileSystem FileSystem { get; }

        [NotNull]
        public string OutputDirectory { get; }

        public virtual void EmitFile([NotNull] IEmitContext context, [NotNull] File file)
        {
            if (!file.IsEmittable)
            {
                return;
            }

            var fileName = PathHelper.NormalizeFilePath(file.FilePath);
            if (fileName.StartsWith("~\\"))
            {
                fileName = fileName.Mid(2);
            }

            Trace.TraceInformation(Msg.I1011, "Publishing", "~\\" + fileName);

            var forceUpdate = Configuration.GetBool(Constants.Configuration.BuildProject.ForceUpdate, true);
            var destinationFileName = PathHelper.Combine(OutputDirectory, fileName);

            FileSystem.CreateDirectoryFromFileName(destinationFileName);
            FileSystem.Copy(file.Snapshot.SourceFile.AbsoluteFileName, destinationFileName, forceUpdate);
        }

        public virtual void EmitItem([NotNull] IEmitContext context, [NotNull] Item item)
        {
        }

        protected virtual void EmitMediaFile([NotNull] IEmitContext context, [NotNull] MediaFile mediaFile)
        {
            EmitFile(context, mediaFile);
        }

        protected override void EmitProjectItems(IEmitContext context, IEnumerable<IProjectItem> projectItems, List<IEmitter> emitters, ICollection<Tuple<IProjectItem, Exception>> retries)
        {
            var unemittedItems = new List<IProjectItem>(projectItems);

            foreach (var projectItem in projectItems)
            {
                if (projectItem is MediaFile mediaFile)
                {
                    unemittedItems.Remove(projectItem);
                    EmitMediaFile(context, mediaFile);
                }
                else if (projectItem is File file)
                {
                    unemittedItems.Remove(projectItem);
                    EmitFile(context, file);
                }
                else if (projectItem is Item item)
                {
                    unemittedItems.Remove(projectItem);
                    EmitItem(context, item);
                }
                else if (projectItem is Template template)
                {
                    unemittedItems.Remove(projectItem);
                    EmitTemplate(context, template);
                }
            }

            base.EmitProjectItems(context, unemittedItems, emitters, retries);
        }

        protected virtual void EmitTemplate([NotNull] IEmitContext context, [NotNull] Template template)
        {
        }
    }
}
