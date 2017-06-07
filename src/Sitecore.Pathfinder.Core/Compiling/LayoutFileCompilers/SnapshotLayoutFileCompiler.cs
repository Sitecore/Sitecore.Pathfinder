// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Composition;
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
    [Export(typeof(ILayoutFileCompiler)), Shared]
    public class SnapshotLayoutFileCompiler : LayoutFileCompilerBase
    {
        [ImportingConstructor]
        public SnapshotLayoutFileCompiler([NotNull] ITraceService trace, [NotNull] IFileSystemService fileSystem, [NotNull] IFactoryService factory, [NotNull] ISnapshotService snapshotService, [NotNull] IPathMapperService pathMapper)
        {
            Trace = trace;
            FileSystem = fileSystem;
            Factory = factory;
            SnapshotService = snapshotService;
            PathMapper = pathMapper;
        }

        [NotNull]
        protected IFactoryService Factory { get; }

        [NotNull]
        protected ITraceService Trace { get; }

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

            var sourcePropertyBag = (ISourcePropertyBag)item;
            if (!sourcePropertyBag.ContainsSourceProperty(LayoutFileItemParser.LayoutFile))
            {
                return false;
            }

            var fileName = sourcePropertyBag.GetValue<string>(LayoutFileItemParser.LayoutFile);
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
                Trace.TraceError(Msg.C1061, Texts.File_not_found, fileName);
                return;
            }

            var sourceFile = Factory.SourceFile(FileSystem, fileName);
            var pathMappingContext = new PathMappingContext(PathMapper).Parse(project, sourceFile);

            var snapshot = SnapshotService.LoadSnapshot(project, sourceFile, pathMappingContext) as ITextSnapshot;
            if (snapshot == null)
            {
                return;
            }

            var layoutResolveContext = new LayoutCompileContext(Trace, project, item.Database, snapshot);
            var layoutCompiler = new LayoutCompiler(FileSystem);

            var xml = layoutCompiler.Compile(layoutResolveContext, snapshot.Root);

            SetRenderingsField(context, item, xml);
        }
    }
}
