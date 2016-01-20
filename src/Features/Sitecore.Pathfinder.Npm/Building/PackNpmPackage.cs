// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sitecore.Pathfinder.Building;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.IO;

namespace Sitecore.Pathfinder.Npm.Building
{
    public class PackNpmPackage : BuildTaskBase
    {
        [ImportingConstructor]
        public PackNpmPackage([NotNull] IPathMapperService pathMapper) : base("pack-npm")
        {
            PathMapper = pathMapper;

            CanRunWithoutConfig = true;
        }

        [NotNull]
        protected IPathMapperService PathMapper { get; }

        public override void Run(IBuildContext context)
        {
            if (context.Project.HasErrors)
            {
                context.Trace.TraceInformation(Msg.D1017, Texts.Package_contains_errors_and_will_not_be_deployed);
                context.IsAborted = true;
                return;
            }

            context.Trace.TraceInformation(Msg.D1023, Texts.Creating_npm_module___);

            var npmFileName = Path.Combine(context.ProjectDirectory, "package.json");
            if (!context.FileSystem.FileExists(npmFileName))
            {
                context.Trace.TraceInformation(Msg.D1024, Texts._package_json__file_not_found__Skipping_);
                return;
            }

            var process = new Process();
            process.StartInfo.FileName = "npm";
            process.StartInfo.Arguments = "pack";
            process.StartInfo.WorkingDirectory = context.ProjectDirectory;
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.CreateNoWindow = true;
            process.Start();
            process.WaitForExit();

            var outputFileName = GetOutputFileName(context, npmFileName);
            if (!string.IsNullOrEmpty(outputFileName) && context.FileSystem.FileExists(outputFileName))
            {
                context.OutputFiles.Add(outputFileName);
            }
        }

        [NotNull]
        protected virtual string GetOutputFileName([NotNull] IBuildContext context, [NotNull] string npmFileName)
        {
            var root = JToken.Parse(context.FileSystem.ReadAllText(npmFileName)) as JObject;
            return root == null ? string.Empty : Path.Combine(context.ProjectDirectory, root["name"] + "-" + root["version"] + ".tgz");
        }

        public override void WriteHelp(HelpWriter helpWriter)
        {
            helpWriter.Summary.Write("Creates an npm module from the project.");
        }
    }
}
