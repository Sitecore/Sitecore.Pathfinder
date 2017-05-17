// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Composition;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Snapshots;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.CodeGeneration.Razor
{
    // [Export(typeof(IProjectItemCodeGenerator)), Shared]
    public class RazorFileCodeGenerator : IProjectItemCodeGenerator
    {
        [ImportingConstructor]
        public RazorFileCodeGenerator([NotNull] IFileSystemService fileSystem)
        {
            FileSystem = fileSystem;
        }

        [NotNull]
        protected IFileSystemService FileSystem { get; }

        public bool CanGenerate(IProjectItem projectItem)
        {
            return projectItem is Item item && !item.IsImport;
        }

        public void Generate(IBuildContext context, ITextTemplatingEngine textTemplatingEngine, IProjectItem projectItem, string templateFileName)
        {
            try
            {
                var outputFileName = templateFileName;
                if (outputFileName.EndsWith(".rzi", StringComparison.OrdinalIgnoreCase))
                {
                    outputFileName = outputFileName.Left(outputFileName.Length - 4);
                }

                if (outputFileName.IndexOf("__Name__", StringComparison.Ordinal) >= 0)
                {
                    outputFileName = outputFileName.Replace("__Name__", projectItem.ShortName);
                }
                else
                {
                    outputFileName = projectItem.ShortName + "." + outputFileName;
                }

                var template = FileSystem.ReadAllText(templateFileName);

                var result = textTemplatingEngine.Generate(template, projectItem);

                FileSystem.WriteAllText(outputFileName, result);
            }
            catch (Exception ex)
            {
                context.Trace.TraceError(Msg.G1000, ex.Message, templateFileName, TextSpan.Empty);
            }
        }
    }
}
