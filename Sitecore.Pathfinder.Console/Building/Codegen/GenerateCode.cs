// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using Sitecore.Pathfinder.CodeGeneration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Building.Codegen
{
    [Export(typeof(ITask))]
    public class GenerateCode : TaskBase
    {
        public GenerateCode() : base("generate-code")
        {
        }

        [NotNull]
        [ImportMany(typeof(ICodeGenerator))]
        public IEnumerable<ICodeGenerator> CodeGenerators { get; protected set; }

        public override void Run(IBuildContext context)
        {
            context.Trace.TraceInformation(Texts.Generating_code___);

            foreach (var projectItem in context.Project.Items)
            {
                foreach (var codeGenerator in CodeGenerators)
                {
                    if (codeGenerator.CanGenerate(projectItem))
                    {
                        Generate(context, codeGenerator, projectItem);
                    }
                }
            }
        }

        protected virtual void Generate([NotNull] IBuildContext context, [NotNull] ICodeGenerator codeGenerator, [NotNull] IProjectItem projectItem)
        {
            var targetFileName = Path.GetDirectoryName(projectItem.Snapshots.First().SourceFile.FileName) ?? string.Empty;
            targetFileName = Path.Combine(targetFileName, projectItem.ShortName) + ".g.cs";

            context.FileSystem.CreateDirectory(Path.GetDirectoryName(targetFileName) ?? string.Empty);

            using (var output = new StreamWriter(targetFileName, false, Encoding.UTF8))
            {
                codeGenerator.Generate(output, targetFileName, projectItem);
            }

            context.Trace.TraceInformation(PathHelper.UnmapPath(context.SolutionDirectory, targetFileName));
        }
    }
}
