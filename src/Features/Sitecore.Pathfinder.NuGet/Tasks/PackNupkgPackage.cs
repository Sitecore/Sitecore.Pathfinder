// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
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

                string nupkgDirectory;
                if (context.Configuration.IsProjectConfigured())
                {
                    nupkgDirectory = Path.GetDirectoryName(nuspecFileName) ?? string.Empty;
                }
                else
                {
                    // otherwise create the sitecore.tools directory
                    nupkgDirectory = Path.Combine(context.ToolsDirectory, "sitecore.project\\" + Path.GetFileNameWithoutExtension(nuspecFileName) + ".nupkg");
                }

                Pack(context, nuspecFileName, nupkgDirectory);
            }
        }

        protected virtual void Pack([NotNull] IBuildContext context, [NotNull] string nuspecFileName, [NotNull] string nupkgDirectory)
        {
            var nupkgFileName = BuildNupkgFile(context, nuspecFileName, nupkgDirectory);
            if (string.IsNullOrEmpty(nupkgFileName))
            {
                return;
            }

            context.OutputFiles.Add(nupkgFileName);
            context.Trace.TraceInformation(Msg.D1019, Texts.NuGet_file_size, $"{PathHelper.UnmapPath(context.Project.ProjectDirectory, nupkgFileName)} ({new FileInfo(nupkgFileName).Length.ToString("#,##0 bytes")})");
        }

        [NotNull]
        protected virtual string BuildNupkgFile([NotNull] IBuildContext context, [NotNull] string nuspecFileName, [NotNull] string nupkgDirectory)
        {
            var nuspec = GetNuspec(context, nuspecFileName);

            var basePath = PathHelper.Combine(context.Project.ProjectDirectory, context.Configuration.GetString(Constants.Configuration.PackNuGet.BasePath));

            var nupkgFileName = string.Empty;

            var stream = new MemoryStream(Encoding.UTF8.GetBytes(nuspec));
            try
            {
                var packageBuilder = new PackageBuilder(stream, basePath);

                nupkgFileName = Path.Combine(nupkgDirectory, packageBuilder.Id + "." + packageBuilder.Version + ".nupkg");

                FileSystem.CreateDirectoryFromFileName(nupkgFileName);

                if (FileSystem.FileExists(nupkgFileName))
                {
                    FileSystem.DeleteFile(nupkgFileName);
                }

                using (var nupkg = FileSystem.OpenWrite(nupkgFileName))
                {
                    packageBuilder.Save(nupkg);
                }
            }
            catch (Exception ex)
            {
                context.Trace.TraceError(Msg.D1020, Texts.Failed_to_create_the_Nupkg_file, ex.Message);
            }

            return nupkgFileName;
        }

        [Diagnostics.NotNull]
        protected virtual string GetNuspec([NotNull] IBuildContext context, [NotNull] string nuspecFileName)
        {
            var nuspec = FileSystem.ReadAllText(nuspecFileName);

            var tokens = GetTokens(context, nuspec);
            foreach (var token in tokens)
            {
                nuspec = nuspec.Replace(token.Key, token.Value);
            }

            return nuspec;
        }

        [NotNull]
        protected virtual IDictionary<string, string> GetTokens([NotNull] IBuildContext context, [NotNull] string nuspec)
        {
            // replace dependencies macro with dependencies from /packages.config
            var dependencies = GetDependencies(context, nuspec);
            var safeProjectUniqueId = Regex.Replace(context.Project.ProjectUniqueId, Constants.SafeProjectUniqueIdRegex, string.Empty);

            var tokens = new Dictionary<string, string>
            {
                ["$toolsDirectory$"] = context.ToolsDirectory,
                ["$projectDirectory$"] = context.ProjectDirectory,
                ["$projectUniqueId$"] = context.Project.ProjectUniqueId,
                ["$safeProjectUniqueId$"] = safeProjectUniqueId,
                ["<dependency id=\"$packages.config$\" version=\"1.0.0\" />"] = dependencies
            };

            foreach (var pair in context.Configuration.GetDictionary(Constants.Configuration.PackNuGet.Tokens))
            {
                tokens[pair.Key] = pair.Value;
            }

            return tokens;
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
