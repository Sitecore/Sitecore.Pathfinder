// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;

namespace Sitecore.Pathfinder.Building.Testing
{
    using System.Collections.Generic;

    [Export(typeof(ITask))]
    public class RunUnitTests : RequestTaskBase
    {
        public RunUnitTests() : base("run-unittests")
        {
        }

        public override void Run(IBuildContext context)
        {
            context.Trace.TraceInformation(Texts.Running_unit_tests___);

            var directory = Path.Combine(context.SolutionDirectory, context.Configuration.Get(Constants.Configuration.LocalTestDirectory));
            context.FileSystem.CreateDirectory(directory);

            CopyTestFilesToWebsite(context, directory);

            RunTests(context);
        }

        private void CopyTestFilesToWebsite([NotNull] IBuildContext context, [NotNull] string directory)
        {
            var targetDirectory = context.Configuration.Get(Constants.Configuration.Wwwroot);
            targetDirectory = PathHelper.Combine(targetDirectory, context.Configuration.Get(Constants.Configuration.WebsiteDirectoryName));
            targetDirectory = PathHelper.Combine(targetDirectory, "sitecore\\shell\\client\\Applications\\Pathfinder\\Tests");
            context.FileSystem.CreateDirectory(targetDirectory);

            var sourceDirectory = Path.Combine(directory, "server");
            context.FileSystem.CreateDirectory(sourceDirectory);

            var targetFileNames = context.FileSystem.GetFiles(targetDirectory, SearchOption.AllDirectories).ToList();
            var sourceFileNames = context.FileSystem.GetFiles(sourceDirectory, SearchOption.AllDirectories).ToList();

            var targets = targetFileNames.Select(f => new FileInfo(f)).ToList();
            var sources = sourceFileNames.Select(f => new FileInfo(f)).ToList();

            var targetNames = targets.Select(f => f.FullName.Mid(targetDirectory.Length)).ToList();
            var sourceNames = sources.Select(f => f.FullName.Mid(sourceDirectory.Length)).ToList();

            var hasNewerFiles = sources.Any(s => targets.Any(t => s.LastWriteTimeUtc > t.LastWriteTimeUtc));
            var hasNewFiles = sourceNames.Any(s => !targetNames.Contains(s));
            var hasOldFiles = targetNames.Any(t => !sourceNames.Contains(t));

            if (!hasNewerFiles && !hasNewFiles && !hasOldFiles)
            {
                return;
            }

            context.Trace.TraceInformation(Texts.Copying_test_files_to_website___);
            context.FileSystem.Mirror(sourceDirectory, targetDirectory);
        }

        private void RunTests([NotNull] IBuildContext context)
        {
            var testRunnerName = context.Configuration.GetString(Constants.Configuration.WebTestRunnerName);

            var queryStringParameters = new Dictionary<string, string>
            {
                ["n"] = testRunnerName
            };

            var url = MakeUrl(context, context.Configuration.GetString(Constants.Configuration.WebTestRunnerUrl), queryStringParameters);
            Request(context, url);
        }
    }
}
