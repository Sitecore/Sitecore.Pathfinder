// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using Sitecore.Pathfinder.CodeGeneration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks
{
    public class GenerateCode : BuildTaskBase
    {
        [ImportingConstructor]
        public GenerateCode([NotNull] IFileSystemService fileSystem, [NotNull, ImportMany, ItemNotNull] IEnumerable<IProjectCodeGenerator> projectCodeGenerators, [NotNull, ImportMany, ItemNotNull] IEnumerable<IProjectItemCodeGenerator> projectItemCodeGenerators) : base("generate-code")
        {
            FileSystem = fileSystem;
            ProjectCodeGenerators = projectCodeGenerators;
            ProjectItemCodeGenerators = projectItemCodeGenerators;
        }

        [NotNull, ItemNotNull]
        public IEnumerable<IProjectCodeGenerator> ProjectCodeGenerators { get; }

        [NotNull]
        protected IFileSystemService FileSystem { get; }

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

        protected virtual void Generate([NotNull] IBuildContext context, [NotNull] IProjectItemCodeGenerator projectItemCodeGenerator, [NotNull] IProjectItem projectItem)
        {
            var baseFileName = Path.GetDirectoryName(projectItem.Snapshot.SourceFile.AbsoluteFileName) ?? string.Empty;
            baseFileName = Path.Combine(baseFileName, projectItem.ShortName);

            FileSystem.CreateDirectoryFromFileName(baseFileName);

            projectItemCodeGenerator.Generate(baseFileName, projectItem);

            context.Trace.TraceInformation(Msg.G1010, PathHelper.UnmapPath(context.ProjectDirectory, baseFileName));
        }
    }
}
