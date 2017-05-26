using System;
using System.Composition;
using System.Runtime.Loader;
using Sitecore.Pathfinder.Compiling.Compilers;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensibility.Pipelines;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Languages.BinFiles.Pipelines;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Languages.BinFiles
{
    [Export(typeof(ICompiler)), Shared]
    public class BinFileCompiler : CompilerBase
    {
        [ImportingConstructor]
        public BinFileCompiler([NotNull] IPipelineService pipelines) : base(1000)
        {
            Pipelines = pipelines;
        }

        [NotNull]
        protected IPipelineService Pipelines { get; }

        public override bool CanCompile(ICompileContext context, IProjectItem projectItem) => projectItem is BinFile;

        public override void Compile(ICompileContext context, IProjectItem projectItem)
        {
            var include = context.Configuration.GetString(Constants.Configuration.BuildProject.CompileBinFilesInclude);
            if (string.IsNullOrEmpty(include))
            {
                return;
            }

            var binFile = projectItem as BinFile;
            Assert.Cast(binFile, nameof(binFile));

            var exclude = context.Configuration.GetString(Constants.Configuration.BuildProject.CompileBinFilesExclude);

            var pathMatcher = new PathMatcher(include, exclude);
            if (!pathMatcher.IsMatch(binFile.FilePath))
            {
                return;
            }

            try
            {
                var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(binFile.Snapshot.SourceFile.AbsoluteFileName);

                foreach (var type in assembly.GetExportedTypes())
                {
                    Pipelines.Resolve<BinFileCompilerPipeline>().Execute(context, binFile, type);
                }
            }
            catch (Exception ex)
            {
                context.Trace.TraceError(Msg.C1059, ex.Message, binFile.FilePath);
            }
        }
    }
}
