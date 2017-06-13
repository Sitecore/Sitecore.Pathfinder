// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Composition;
using Sitecore.Pathfinder.CodeGeneration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks
{
    [Export(typeof(ITask)), Shared]
    public class GenerateCode : BuildTaskBase
    {
        [ImportingConstructor]
        public GenerateCode([NotNull] IFileSystem fileSystem, [NotNull] ITextTemplatingEngine textTemplatingEngine, [NotNull, ImportMany, ItemNotNull] IEnumerable<ICodeGenerator> codeGenerators) : base("generate-code")
        {
            FileSystem = fileSystem;
            TextTemplatingEngine = textTemplatingEngine;
            CodeGenerators = codeGenerators;
        }

        [NotNull, ItemNotNull]
        public IEnumerable<ICodeGenerator> CodeGenerators { get; }

        [NotNull]
        public ITextTemplatingEngine TextTemplatingEngine { get; }

        [NotNull]
        protected IFileSystem FileSystem { get; }

        public override void Run(IBuildContext context)
        {
            var project = context.LoadProject();

            context.Trace.TraceInformation(Msg.G1009, Texts.Generating_code___);

            foreach (var codeGenerator in CodeGenerators)
            {
                codeGenerator.Generate(context, TextTemplatingEngine, project);
            }
        }
    }
}
