// © 2015 Sitecore Corporation A/S. All rights reserved.
// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.IO;
using Sitecore.Pathfinder.Building;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.T4.CodeGeneration
{
    public class T4ProjectCodeGenerator : T4GeneratorBase
    {
        public override void Generate(IBuildContext context, IProject project)
        {
            var host = GetHost(context, project);

            foreach (var fileName in context.FileSystem.GetFiles(context.ProjectDirectory, "*.project.tt", SearchOption.AllDirectories))
            {
                if (Ignore(fileName))
                {
                    continue;
                }

                context.Trace.TraceInformation("Generating code", PathHelper.UnmapPath(context.ProjectDirectory, fileName));

                try
                {
                    if (!host.ProcessTemplate(fileName, fileName.Left(fileName.Length - 11)))
                    {
                        TraceErrors(context, host, fileName);
                    }
                }
                catch (Exception ex)
                {
                    context.Trace.TraceError(ex.Message, fileName, TextSpan.Empty);
                }
            }
        }
    }
}
