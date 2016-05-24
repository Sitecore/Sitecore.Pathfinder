// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.ComponentModel.Composition;
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
        [NotNull]
        protected IFileSystemService FileSystem { get; }

        [ImportingConstructor]
        public PackNupkgPackage([NotNull] IFileSystemService fileSystem) : base("pack-nuget")
        {
            FileSystem = fileSystem;
        }

        public override void Run(IBuildContext context)
        {
            context.Trace.TraceInformation(Msg.D1018, Texts.Creating_Nupkg_file___);

            var packageDirectory = context.Configuration.GetString(Constants.Configuration.PackNuGet.Directory);

            string directory;
            if (!context.Configuration.IsProjectConfigured())
            {
                directory = PathHelper.Combine(context.ToolsDirectory, "files\\project.noconfig\\sitecore.project");
            }
            else
            {
                directory = PathHelper.Combine(context.Project.ProjectDirectory, packageDirectory);
            }

            var pathMatcher = new PathMatcher(context.Configuration.GetString(Constants.Configuration.PackNuGet.Include), context.Configuration.GetString(Constants.Configuration.PackNuGet.Exclude));

            foreach (var nuspecFileName in FileSystem.GetFiles(directory, "*", SearchOption.AllDirectories))
            {
                if (!pathMatcher.IsMatch(nuspecFileName))
                {
                    continue;
                }

                string nupkgFileName;
                if (context.Configuration.IsProjectConfigured())
                {
                    nupkgFileName = Path.ChangeExtension(nuspecFileName, ".nupkg");
                }
                else
                {
                    // otherwise create the sitecore.tools directory
                    nupkgFileName = Path.Combine(context.ToolsDirectory, "sitecore.project\\" + Path.GetFileNameWithoutExtension(nuspecFileName) + ".nupkg");
                    FileSystem.CreateDirectoryFromFileName(nupkgFileName);
                }

                Pack(context, nuspecFileName, nupkgFileName);
            }
        }

        protected virtual void Pack([NotNull] IBuildContext context, [NotNull] string nuspecFileName, [NotNull] string nupkgFileName)
        {
            if (FileSystem.FileExists(nupkgFileName))
            {
                FileSystem.DeleteFile(nupkgFileName);
            }

            BuildNupkgFile(context, nuspecFileName, nupkgFileName);

            context.OutputFiles.Add(nupkgFileName);

            context.Trace.TraceInformation(Msg.D1019, Texts.NuGet_file_size, $"{PathHelper.UnmapPath(context.Project.ProjectDirectory, nupkgFileName)} ({new FileInfo(nupkgFileName).Length.ToString("#,##0 bytes")})");
        }

        protected virtual void BuildNupkgFile([NotNull] IBuildContext context, [NotNull] string nuspecFileName, [NotNull] string nupkgFileName)
        {
            var nuspec = GetNuspec(context, nuspecFileName);

            var stream = new MemoryStream(Encoding.UTF8.GetBytes(nuspec));
            try
            {
                var packageBuilder = new PackageBuilder(stream, context.Project.ProjectDirectory);

                using (var nupkg = FileSystem.OpenWrite(nupkgFileName))
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

            var nuspec = FileSystem.ReadAllText(nuspecFileName);

            nuspec = nuspec.Replace("$global.scconfig.json$", configFileName);
            nuspec = nuspec.Replace("$toolsDirectory$", context.ToolsDirectory);

            // replace dependencies macro with dependencies from /packages.config
            var packagesConfig = GetDependencies(context, nuspec);
            nuspec = nuspec.Replace("<dependency id=\"$packages.config\" version=\"1.0.0\" />", packagesConfig);

            return nuspec;
        }

        [Diagnostics.NotNull]
        protected virtual string GetDependencies([Diagnostics.NotNull] IBuildContext context, [Diagnostics.NotNull] string nuspec)
        {
            var writer = new StringWriter();

            // load dependencies from scconfig.json
            foreach (var pair in context.Configuration.GetSubKeys(Constants.Configuration.Dependencies))
            {
                var id = pair.Key;
                var version = context.Configuration.GetString(Constants.Configuration.Dependencies + ":" + id);

                // check if the "Sitecore.Pathfinder.Core" package is already included in the nuspec
                if (id == "Sitecore.Pathfinder.Core" && nuspec.IndexOf(" id=\"Sitecore.Pathfinder.Core\" ", StringComparison.Ordinal) >= 0)
                {
                    continue;
                }

                writer.WriteLine($"    <dependency id=\"{id}\" version=\"{version}\" />");
            }

            // load dependencies from packages.config
            var fileName = Path.Combine(context.Project.ProjectDirectory, "packages.config");
            if (!FileSystem.FileExists(fileName))
            {
                return writer.ToString();
            }

            var text = FileSystem.ReadAllText(fileName);
            var root = text.ToXElement();
            if (root == null)
            {
                return writer.ToString();
            }

            foreach (var element in root.Elements())
            {
                var id = element.GetAttributeValue("id");
                var version = element.GetAttributeValue("version");

                // check if the "Sitecore.Pathfinder.Core" package is already included in the nuspec
                if (id == "Sitecore.Pathfinder.Core" && nuspec.IndexOf(" id=\"Sitecore.Pathfinder.Core\" ", StringComparison.Ordinal) >= 0)
                {
                    continue;
                }

                writer.WriteLine($"    <dependency id=\"{id}\" version=\"{version}\" />");
            }

            return writer.ToString();
        }
    }
}
