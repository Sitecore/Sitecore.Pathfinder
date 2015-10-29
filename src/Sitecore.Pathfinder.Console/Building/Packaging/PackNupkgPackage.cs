// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.IO;
using NuGet;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.IO;

namespace Sitecore.Pathfinder.Building.Packaging
{
    public class PackNupkgPackage : TaskBase
    {
        public PackNupkgPackage() : base("pack-nuget")
        {
        }

        public override void Run(IBuildContext context)
        {
            if (context.Project.HasErrors)
            {
                context.Trace.TraceInformation(Texts.Package_contains_errors_and_will_not_be_deployed);
                context.IsAborted = true;
                return;
            }

            context.Trace.TraceInformation(Texts.Creating_Nupkg_file___);

            var packageFileName = context.Configuration.Get(Constants.Configuration.PackNugetDirectory);
            var directory = PathHelper.Combine(context.SolutionDirectory, packageFileName);
            var pathMatcher = new PathMatcher(context.Configuration.Get(Constants.Configuration.PackNugetInclude), context.Configuration.Get(Constants.Configuration.PackNugetExclude));

            foreach (var fileName in context.FileSystem.GetFiles(directory, "*", SearchOption.AllDirectories))
            {
                if (pathMatcher.IsMatch(fileName))
                {
                    Pack(context, fileName);
                }
            }
        }

        public override void WriteHelp(HelpWriter helpWriter)
        {
            helpWriter.Summary.Write("Creates packages from the project.");
            helpWriter.Remarks.Write("The Nuget specifications and Nuget packages are located in the /sitecore.project folder.");
        }

        protected virtual void Pack([NotNull] IBuildContext context, [NotNull] string nuspecFileName)
        {
            var nupkgFileName = Path.ChangeExtension(nuspecFileName, ".nupkg");

            if (context.FileSystem.FileExists(nupkgFileName))
            {
                context.FileSystem.DeleteFile(nupkgFileName);
            }

            BuildNupkgFile(context, nuspecFileName, nupkgFileName);

            context.OutputFiles.Add(nupkgFileName);

            context.Trace.TraceInformation(Texts.NuGet_file_size, $"{PathHelper.UnmapPath(context.SolutionDirectory, nupkgFileName)} ({new FileInfo(nupkgFileName).Length.ToString("#,##0 bytes")})");
        }

        protected virtual void BuildNupkgFile([NotNull] IBuildContext context, [NotNull] string nuspecFileName, [NotNull] string nupkgFileName)
        {
            try
            {
                using (var nuspec = new FileStream(nuspecFileName, FileMode.Open, FileAccess.Read))
                {
                    var packageBuilder = new PackageBuilder(nuspec, context.SolutionDirectory);

                    using (var nupkg = new FileStream(nupkgFileName, FileMode.Create))
                    {
                        packageBuilder.Save(nupkg);
                    }
                }
            }
            catch (Exception ex)
            {
                context.Trace.TraceError(Texts.Failed_to_create_the_Nupkg_file, ex.Message);
            }
        }
    }
}
