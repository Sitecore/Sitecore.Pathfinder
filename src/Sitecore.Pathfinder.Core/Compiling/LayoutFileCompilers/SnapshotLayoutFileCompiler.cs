// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.ComponentModel.Composition;
using System.IO;
using Sitecore.Pathfinder.Compiling.Compilers;
using Sitecore.Pathfinder.Compiling.FieldCompilers;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Parsing;
using Sitecore.Pathfinder.Parsing.LayoutFiles;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Compiling.LayoutFileCompilers
{
    public class SnapshotLayoutFileCompiler : LayoutFileCompilerBase
    {
        [ImportingConstructor]
        public SnapshotLayoutFileCompiler([NotNull] IFileSystemService fileSystem, [NotNull] IFactoryService factory, [NotNull] ISnapshotService snapshotService, [NotNull] IPathMapperService pathMapper)
        {
            FileSystem = fileSystem;
            Factory = factory;
            SnapshotService = snapshotService;
            PathMapper = pathMapper;
        }

        [NotNull]
        protected IFactoryService Factory { get; }

        [NotNull]
        protected IFileSystemService FileSystem { get; }

        [NotNull]
        protected IPathMapperService PathMapper { get; }

        [NotNull]
        protected ISnapshotService SnapshotService { get; }

        public override bool CanCompile(ICompileContext context, IProjectItem projectItem, SourceProperty<string> property)
        {
            var item = projectItem as Item;
            if (item == null)
            {
                return false;
            }

            if (!item.ContainsProperty(LayoutFileItemParser.LayoutFile))
            {
                return false;
            }

            var fileName = item.GetValue<string>(LayoutFileItemParser.LayoutFile);
            if (string.IsNullOrEmpty(fileName))
            {
                return false;
            }

            var extension = PathHelper.GetExtension(fileName);

            // todo: come up with a better name
            if (string.Equals(extension, ".layoutfile.xml", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (string.Equals(extension, ".layoutfile.json", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (string.Equals(extension, ".layoutfile.yaml", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }

        public override void Compile(ICompileContext context, IProjectItem projectItem, SourceProperty<string> property)
        {
            var item = projectItem as Item;
            Assert.Cast(item, nameof(item));

            var value = property.GetValue().Trim();
            if (string.IsNullOrEmpty(value))
            {
                return;
            }

            var project = projectItem.Project;

            var fileName = value;
            if (fileName.StartsWith("~/"))
            {
                fileName = fileName.Mid(2);
            }

            fileName = Path.Combine(project.ProjectDirectory, fileName);
            if (!FileSystem.FileExists(fileName))
            {
                context.Trace.TraceError(Msg.C1061, "File not found", fileName);
                return;
            }

            var sourceFile = Factory.SourceFile(FileSystem, project.ProjectDirectory, fileName);
            var pathMappingContext = new PathMappingContext(PathMapper).Parse(project, sourceFile);

            var snapshot = SnapshotService.LoadSnapshot(project, sourceFile, pathMappingContext) as ITextSnapshot;
            if (snapshot == null)
            {
                return;
            }

            var layoutResolveContext = new LayoutCompileContext(context.Trace, project, item.Database, snapshot);
            var layoutCompiler = new LayoutCompiler(FileSystem);

            var xml = layoutCompiler.Compile(layoutResolveContext, snapshot.Root);

            SetRenderingsField(context, item, xml);
        }
    }
}
