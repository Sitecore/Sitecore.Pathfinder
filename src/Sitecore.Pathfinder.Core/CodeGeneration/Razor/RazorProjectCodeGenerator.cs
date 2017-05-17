// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Composition;
using System.IO;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Snapshots;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.CodeGeneration.Razor
{
    [Export(typeof(IProjectCodeGenerator)), Shared]
    public class RazorProjectCodeGenerator : IProjectCodeGenerator
    {
        [ImportingConstructor]
        public RazorProjectCodeGenerator([NotNull] IFileSystemService fileSystem)
        {
            FileSystem = fileSystem;
        }

        [NotNull]
        protected IFileSystemService FileSystem { get; }

        public void Generate(IBuildContext context, ITextTemplatingEngine textTemplatingEngine, IProjectBase project)
        {
            foreach (var fileName in FileSystem.GetFiles(context.ProjectDirectory, "*.rzp", SearchOption.AllDirectories))
            {
                context.Trace.TraceInformation(Msg.G1008, Texts.Generating_code, PathHelper.UnmapPath(context.ProjectDirectory, fileName));

                try
                {
                    var template = FileSystem.ReadAllText(fileName);

                    var result = textTemplatingEngine.Generate(template, project);

                    FileSystem.WriteAllText(fileName.Left(fileName.Length - 4), result);
                }
                catch (Exception ex)
                {
                    context.Trace.TraceError(Msg.G1007, ex.Message, fileName, TextSpan.Empty);
                }
            }
        }
    }
}
