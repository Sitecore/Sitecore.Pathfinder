// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Composition;
using Sitecore.Pathfinder.Configuration.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Files;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Projects.Templates;

namespace Sitecore.Pathfinder.Emitting.Emitters
{
    public abstract class DirectoryProjectEmitterBase : ProjectEmitterBase
    {
        [ImportingConstructor]
        protected DirectoryProjectEmitterBase([NotNull] IConfiguration configuration, [NotNull] ITraceService traceService, [ItemNotNull, NotNull, ImportMany] IEnumerable<IEmitter> emitters, [NotNull] IFileSystemService fileSystem) : base(configuration, traceService, emitters)
        {
            FileSystem = fileSystem;
            OutputDirectory = PathHelper.Combine(Configuration.GetProjectDirectory(), Configuration.GetString(Constants.Configuration.Output.Directory));
        }

        [NotNull]
        public IFileSystemService FileSystem { get; }

        [NotNull]
        public string OutputDirectory { get; protected set; }

        public virtual void EmitFile([NotNull] IEmitContext context, [NotNull] string sourceFileAbsoluteFileName, [NotNull] string filePath)
        {
            var fileName = PathHelper.NormalizeFilePath(filePath);
            if (fileName.StartsWith("~\\"))
            {
                fileName = fileName.Mid(2);
            }

            context.Trace.TraceInformation(Msg.I1011, "Publishing", "~\\" + fileName);

            var forceUpdate = Configuration.GetBool(Constants.Configuration.BuildProject.ForceUpdate, true);
            var destinationFileName = PathHelper.Combine(OutputDirectory, fileName);

            FileSystem.CreateDirectoryFromFileName(destinationFileName);
            FileSystem.Copy(sourceFileAbsoluteFileName, destinationFileName, forceUpdate);
        }

        public abstract void EmitItem([NotNull] IEmitContext context, [NotNull] Item item);

        protected override void EmitProjectItems(IEmitContext context, IEnumerable<IProjectItem> projectItems, List<IEmitter> emitters, ICollection<Tuple<IProjectItem, Exception>> retries)
        {
            var unemittedItems = new List<IProjectItem>(projectItems);

            foreach (var projectItem in projectItems)
            {
                if (projectItem is File file)
                {
                    unemittedItems.Remove(projectItem);
                    if (file.IsEmittable)
                    {
                        EmitFile(context, projectItem.Snapshot.SourceFile.AbsoluteFileName, file.FilePath);
                    }
                }

                if (projectItem is Item item)
                {
                    unemittedItems.Remove(projectItem);
                    if (item.IsEmittable)
                    {
                        EmitItem(context, item);
                    }
                }

                if (projectItem is Template template)
                {
                    unemittedItems.Remove(projectItem);
                    if (template.IsEmittable)
                    {
                        EmitTemplate(context, template);
                    }
                }
            }

            base.EmitProjectItems(context, unemittedItems, emitters, retries);
        }

        protected virtual void EmitTemplate([NotNull] IEmitContext context, [NotNull] Template template)
        {
        }
    }
}
