// © 2015 Sitecore Corporation A/S. All rights reserved.
// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.ComponentModel.Composition;
using System.IO;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Snapshots;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.T4.CodeGeneration
{
    public class T4ProjectCodeGenerator : T4GeneratorBase
    {
        [ImportingConstructor]
        public T4ProjectCodeGenerator([NotNull] IFileSystemService fileSystem)
        {
            FileSystem = fileSystem;
        }

        [NotNull]
        protected IFileSystemService FileSystem { get; }


        public override void Generate(IBuildContext context, IProject project)
        {
            var host = GetHost(context, project);

            foreach (var fileName in FileSystem.GetFiles(context.Project.ProjectDirectory, "*.project.tt", SearchOption.AllDirectories))
            {
                if (Ignore(fileName))
                {
                    continue;
                }

                context.Trace.TraceInformation(Msg.G1008, Texts.Generating_code, PathHelper.UnmapPath(context.Project.ProjectDirectory, fileName));

                try
                {
                    if (!host.ProcessTemplate(fileName, fileName.Left(fileName.Length - 11)))
                    {
                        TraceErrors(context, host, fileName);
                    }
                }
                catch (Exception ex)
                {
                    context.Trace.TraceError(Msg.G1007, ex.Message, fileName, TextSpan.Empty);
                }
            }
        }
    }
}
