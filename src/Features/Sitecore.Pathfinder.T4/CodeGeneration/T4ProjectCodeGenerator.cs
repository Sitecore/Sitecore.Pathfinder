// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.IO;
using Microsoft.VisualStudio.TextTemplating;
using Mono.TextTemplating;
using Sitecore.Pathfinder.Building;
using Sitecore.Pathfinder.CodeGeneration;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.T4.CodeGeneration
{
    public class T4ProjectCodeGenerator : ProjectCodeGeneratorBase
    {
        public override void Generate(IBuildContext context, IProject project)
        {
            var templateDirectory = Path.Combine(context.ProjectDirectory, "sitecore.project\\extensions\\codegen");

            var engine = new Engine();

            foreach (var fileName in context.FileSystem.GetFiles(templateDirectory, "*.tt", SearchOption.AllDirectories))
            {
                var templateText = context.FileSystem.ReadAllText(fileName);

                var host = new TemplateGenerator();

                var output = engine.ProcessTemplate(templateText, host);

                var targetFileName = fileName.Left(fileName.Length - 3);

                context.FileSystem.WriteAllText(targetFileName, output);
            }
        }
    }
}
