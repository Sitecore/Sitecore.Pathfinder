// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.IO;
using System.Text;
using NuGet;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.NuGet.Tasks
{
    public class PackNupkgPackage : BuildTaskBase
    {
        public PackNupkgPackage() : base("pack-nuget")
        {
            CanRunWithoutConfig = true;
        }

        public override void Run(IBuildContext context)
        {
            if (context.Project.HasErrors)
            {
                context.Trace.TraceInformation(Msg.D1017, Texts.Package_contains_errors_and_will_not_be_deployed);
                context.IsAborted = true;
                return;
            }

            context.Trace.TraceInformation(Msg.D1018, Texts.Creating_Nupkg_file___);

            var packageDirectory = context.Configuration.Get(Constants.Configuration.PackNugetDirectory);

            string directory;
            if (context.Configuration.GetBool(Constants.Configuration.BuildingWithNoConfig))
            {
                directory = PathHelper.Combine(context.Configuration.GetString(Constants.Configuration.ToolsDirectory), "files\\project.noconfig\\sitecore.project");
            }
            else
            {
                directory = PathHelper.Combine(context.ProjectDirectory, packageDirectory);
            }

            var pathMatcher = new PathMatcher(context.Configuration.Get(Constants.Configuration.PackNugetInclude), context.Configuration.Get(Constants.Configuration.PackNugetExclude));

            foreach (var nuspecFileName in context.FileSystem.GetFiles(directory, "*", SearchOption.AllDirectories))
            {
                if (!pathMatcher.IsMatch(nuspecFileName))
                {
                    continue;
                }

                string nupkgFileName;
                if (context.IsRunningWithNoConfig)
                {
                    nupkgFileName = Path.Combine(context.ProjectDirectory, "sitecore.project\\" + Path.GetFileNameWithoutExtension(nuspecFileName) + ".nupkg");
                    context.FileSystem.CreateDirectoryFromFileName(nupkgFileName);
                }
                else
                {
                    nupkgFileName = Path.ChangeExtension(nuspecFileName, ".nupkg");
                }

                Pack(context, nuspecFileName, nupkgFileName);
            }
        }

        protected virtual void Pack([NotNull] IBuildContext context, [NotNull] string nuspecFileName, [NotNull] string nupkgFileName)
        {
            if (context.FileSystem.FileExists(nupkgFileName))
            {
                context.FileSystem.DeleteFile(nupkgFileName);
            }

            BuildNupkgFile(context, nuspecFileName, nupkgFileName);

            context.OutputFiles.Add(nupkgFileName);

            context.Trace.TraceInformation(Msg.D1019, Texts.NuGet_file_size, $"{PathHelper.UnmapPath(context.ProjectDirectory, nupkgFileName)} ({new FileInfo(nupkgFileName).Length.ToString("#,##0 bytes")})");
        }

        protected virtual void BuildNupkgFile([NotNull] IBuildContext context, [NotNull] string nuspecFileName, [NotNull] string nupkgFileName)
        {
            var nuspec = GetNuspec(context, nuspecFileName);

            var stream = new MemoryStream(Encoding.UTF8.GetBytes(nuspec));
            try
            {
                var packageBuilder = new PackageBuilder(stream, context.ProjectDirectory);

                using (var nupkg = new FileStream(nupkgFileName, FileMode.Create))
                {
                    packageBuilder.Save(nupkg);
                }
            }
            catch (Exception ex)
            {
                context.Trace.TraceError(Msg.D1020, Texts.Failed_to_create_the_Nupkg_file, ex.Message);
            }
        }

        [Diagnostics.NotNull]
        protected virtual string GetNuspec([NotNull] IBuildContext context, [NotNull] string nuspecFileName)
        {
            var configFileName = Path.Combine(context.ToolsDirectory, context.Configuration.GetString(Constants.Configuration.ProjectConfigFileName));

            var nuspec = context.FileSystem.ReadAllText(nuspecFileName);

            nuspec = nuspec.Replace("$global.scconfig.json$", configFileName);
            nuspec = nuspec.Replace("$toolsDirectory$", context.ToolsDirectory);

            // replace dependencies macro with dependencies from /packages.config
            var packagesConfig = GetPackagesConfig(context, nuspec);
            nuspec = nuspec.Replace("<dependency id=\"$packages.config\" version=\"1.0.0\" />", packagesConfig);

            return nuspec;
        }

        [Diagnostics.NotNull]
        protected virtual string GetPackagesConfig([Diagnostics.NotNull] IBuildContext context, [Diagnostics.NotNull] string nuspec)
        {
            var fileName = Path.Combine(context.ProjectDirectory, "packages.config");
            if (!context.FileSystem.FileExists(fileName))
            {
                return string.Empty;
            }

            var text = context.FileSystem.ReadAllText(fileName);
            var root = text.ToXElement();
            if (root == null)
            {
                return string.Empty;
            }

            var sb = new StringBuilder();

            foreach (var element in root.Elements())
            {
                var id = element.GetAttributeValue("id");
                var version = element.GetAttributeValue("version");

                // Check if the "Sitecore.Pathfinder.Core" package is already included in the nuspec
                if (id == "Sitecore.Pathfinder.Core" && nuspec.IndexOf(" id=\"Sitecore.Pathfinder.Core\" ", StringComparison.Ordinal) >= 0)
                {
                    continue;
                }

                sb.Append($"    <dependency id=\"{id}\" version=\"{version}\" />");
            }

            return sb.ToString();

        }
    }
}
