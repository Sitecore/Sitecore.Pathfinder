// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Linq;
using System.Reflection;
using Sitecore.Pathfinder.Compiling.Compilers;
using Sitecore.Pathfinder.Diagnostics;
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
            // todo: restrict the assemblies and types to compile to increase performance

            var binFile = projectItem as BinFile;
            Assert.Cast(binFile, nameof(binFile));

            try
            {
                var assembly = Assembly.ReflectionOnlyLoadFrom(binFile.Snapshots.First().SourceFile.AbsoluteFileName);

                foreach (var type in assembly.GetExportedTypes())
                {
                    context.Pipelines.Resolve<BinFileCompilerPipeline>().Execute(context, binFile, type);
                }
            }
            catch (Exception ex)
            {
                context.Trace.TraceError(ex.Message, binFile.FilePath);
            }

        }
    }
}
