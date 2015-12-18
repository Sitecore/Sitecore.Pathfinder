// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Pathfinder.Compiling.Compilers;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensibility.Pipelines;

namespace Sitecore.Pathfinder.Languages.BinFiles.Pipelines
{
    public class BinFileCompilerPipeline : PipelineBase<BinFileCompilerPipeline>
    {
        [NotNull]
        public BinFile BinFile { get; private set; }

        [NotNull]
        public ICompileContext Context { get; private set; }

        [NotNull]
        public Type Type { get; private set; }

        [NotNull]
        public BinFileCompilerPipeline Execute([NotNull] ICompileContext context, [NotNull] BinFile binFile, [NotNull] Type type)
        {
            Context = context;
            BinFile = binFile;
            Type = type;

            Execute();

            return this;
        }
    }
}
