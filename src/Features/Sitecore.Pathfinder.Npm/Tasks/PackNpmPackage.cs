// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Npm.Tasks
{
    public class PackNpmPackage : BuildTaskBase
    {
        [ImportingConstructor]
        public PackNpmPackage([NotNull] IFileSystemService fileSystem, [NotNull] IPathMapperService pathMapper) : base("pack-npm")
        {
            FileSystem = fileSystem;
            PathMapper = pathMapper;
        }

        [NotNull]
        protected IFileSystemService FileSystem { get; }

        [NotNull]
        protected IPathMapperService PathMapper { get; }

        public override void Run(IBuildContext context)
        {
            context.Trace.TraceInformation(Msg.D1023, Texts.Creating_npm_module___);

            var npmFileName = Path.Combine(context.Project.ProjectDirectory, "package.json");
            if (!FileSystem.FileExists(npmFileName))
            {
                context.Trace.TraceInformation(Msg.D1024, Texts._package_json__file_not_found__Skipping_);
                return;
            }

            var process = new Process();
            process.StartInfo.FileName = "npm";
            process.StartInfo.Arguments = "pack";
            process.StartInfo.WorkingDirectory = context.Project.ProjectDirectory;
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.CreateNoWindow = true;
            process.Start();
            process.WaitForExit();

            var outputFileName = GetOutputFileName(context, npmFileName);
            if (!string.IsNullOrEmpty(outputFileName) && FileSystem.FileExists(outputFileName))
            {
                context.OutputFiles.Add(outputFileName);
            }
        }

        [NotNull]
        protected virtual string GetOutputFileName([NotNull] IBuildContext context, [NotNull] string npmFileName)
        {
            var root = JToken.Parse(FileSystem.ReadAllText(npmFileName)) as JObject;
            return root == null ? string.Empty : Path.Combine(context.Project.ProjectDirectory, root["name"] + "-" + root["version"] + ".tgz");
        }
    }
}
