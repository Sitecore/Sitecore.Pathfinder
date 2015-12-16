// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using Sitecore.Pathfinder.CodeGeneration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Building.Codegen
{
    public class GenerateCode : BuildTaskBase
    {
        [ImportingConstructor]
        public GenerateCode([NotNull, ImportMany, ItemNotNull]   IEnumerable<IProjectCodeGenerator> projectCodeGenerators, [NotNull, ImportMany, ItemNotNull]   IEnumerable<IProjectItemCodeGenerator> projectItemCodeGenerators) : base("generate-code")
        {
            ProjectCodeGenerators = projectCodeGenerators;
            ProjectItemCodeGenerators = projectItemCodeGenerators;
        }

        [NotNull, ItemNotNull]
        public IEnumerable<IProjectCodeGenerator> ProjectCodeGenerators { get; }

        [NotNull, ItemNotNull]
        protected IEnumerable<IProjectItemCodeGenerator> ProjectItemCodeGenerators { get; }

        public override void Run(IBuildContext context)
        {
            context.Trace.TraceInformation(Msg.G1009, Texts.Generating_code___);

            foreach (var projectCodeGenerator in ProjectCodeGenerators)
            {
                projectCodeGenerator.Generate(context, context.Project);
            }

            foreach (var projectItem in context.Project.ProjectItems)
            {
                foreach (var projectItemCodeGenerator in ProjectItemCodeGenerators)
                {
                    if (projectItemCodeGenerator.CanGenerate(projectItem))
                    {
                        Generate(context, projectItemCodeGenerator, projectItem);
                    }
                }
            }
        }

        public override void WriteHelp(HelpWriter helpWriter)
        {
            helpWriter.Summary.Write("Generates code from items and files in the project.");
            helpWriter.Remarks.Write(@"Pathfinder can generate code based on your project. The most obvious thing is to generate a C# class for each template in the project.

To generate code, execute the task `generate-code`. This wil iterate through the elements in the project and check if a code generator is available for that item. If so, the code generator is executed.

Code generators are simply extensions that are located in the /sitecore.project/extensions/codegen directory.

Normally you want to run the `generate-code` task before building an assembly, so the C# source files are up-to-date.");
        }

        protected virtual void Generate([NotNull] IBuildContext context, [NotNull] IProjectItemCodeGenerator projectItemCodeGenerator, [NotNull] IProjectItem projectItem)
        {
            var baseFileName = Path.GetDirectoryName(projectItem.Snapshots.First().SourceFile.AbsoluteFileName) ?? string.Empty;
            baseFileName = Path.Combine(baseFileName, projectItem.ShortName);

            context.FileSystem.CreateDirectory(Path.GetDirectoryName(baseFileName) ?? string.Empty);

            projectItemCodeGenerator.Generate(baseFileName, projectItem);

            context.Trace.TraceInformation(Msg.G1010, PathHelper.UnmapPath(context.ProjectDirectory, baseFileName));
        }
    }
}
