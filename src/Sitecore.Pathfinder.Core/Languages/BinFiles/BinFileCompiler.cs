// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Linq;
using System.Reflection;
using Sitecore.Pathfinder.Compiling.Compilers;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Languages.BinFiles.Pipelines;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Languages.BinFiles
{
    public class BinFileCompiler : CompilerBase
    {
        public BinFileCompiler() : base(1000)
        {
        }

        public override bool CanCompile(ICompileContext context, IProjectItem projectItem)
        {
            return projectItem is BinFile;
        }

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
                var assembly = Assembly.LoadFrom(binFile.Snapshot.SourceFile.AbsoluteFileName);

                foreach (var type in assembly.GetExportedTypes())
                {
                    context.Pipelines.Resolve<BinFileCompilerPipeline>().Execute(context, binFile, type);
                }
            }
            catch (Exception ex)
            {
                context.Trace.TraceError(Msg.C1059, ex.Message, binFile.FilePath);
            }
        }
    }
}
