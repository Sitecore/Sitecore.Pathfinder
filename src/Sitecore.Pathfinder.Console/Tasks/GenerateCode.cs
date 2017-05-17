// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Composition;
using System.IO;
using Sitecore.Pathfinder.CodeGeneration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks
{
    [Export(typeof(ITask)), Shared]
    public class GenerateCode : BuildTaskBase
    {
        [ImportingConstructor]
        public GenerateCode([NotNull] IFileSystemService fileSystem, [NotNull] ITextTemplatingEngine textTemplatingEngine,  [NotNull, ImportMany, ItemNotNull] IEnumerable<IProjectCodeGenerator> projectCodeGenerators, [NotNull, ImportMany, ItemNotNull] IEnumerable<IProjectItemCodeGenerator> projectItemCodeGenerators) : base("generate-code")
        {
            FileSystem = fileSystem;
            TextTemplatingEngine = textTemplatingEngine;
            ProjectCodeGenerators = projectCodeGenerators;
            ProjectItemCodeGenerators = projectItemCodeGenerators;
        }

        [NotNull, ItemNotNull]
        public IEnumerable<IProjectCodeGenerator> ProjectCodeGenerators { get; }

        [NotNull]
        protected IFileSystemService FileSystem { get; }

        [NotNull]
        public ITextTemplatingEngine TextTemplatingEngine { get; }

        [NotNull, ItemNotNull]
        protected IEnumerable<IProjectItemCodeGenerator> ProjectItemCodeGenerators { get; }

        public override void Run(IBuildContext context)
        {
            var project = context.LoadProject();

            context.Trace.TraceInformation(Msg.G1009, Texts.Generating_code___);

            foreach (var projectCodeGenerator in ProjectCodeGenerators)
            {
                projectCodeGenerator.Generate(context, TextTemplatingEngine, project);
            }

            foreach (var projectItem in project.ProjectItems)
            {
                foreach (var projectItemCodeGenerator in ProjectItemCodeGenerators)
                {
                    if (projectItemCodeGenerator.CanGenerate(projectItem))
                    {
                        Generate(context, TextTemplatingEngine, projectItemCodeGenerator, projectItem);
                    }
                }
            }
        }

        protected virtual void Generate([NotNull] IBuildContext context, [NotNull] ITextTemplatingEngine textTemplatingEngine, [NotNull] IProjectItemCodeGenerator projectItemCodeGenerator, [NotNull] IProjectItem projectItem)
        {
            var baseFileName = Path.GetDirectoryName(projectItem.Snapshot.SourceFile.AbsoluteFileName) ?? string.Empty;
            baseFileName = Path.Combine(baseFileName, projectItem.ShortName);

            FileSystem.CreateDirectoryFromFileName(baseFileName);

            context.Trace.TraceInformation(Msg.G1010, PathHelper.UnmapPath(context.ProjectDirectory, baseFileName));

            projectItemCodeGenerator.Generate(context, textTemplatingEngine, projectItem, baseFileName);
        }
    }
}
