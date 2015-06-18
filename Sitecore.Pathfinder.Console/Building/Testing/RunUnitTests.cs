// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Testing;

namespace Sitecore.Pathfinder.Building.Testing
{
    [Export(typeof(ITask))]
    public class RunUnitTests : RequestTaskBase
    {
        public RunUnitTests() : base("run-unittests")
        {
        }

        public override void Run(IBuildContext context)
        {
            context.Trace.TraceInformation(Texts.Running_unit_tests___);

            var directory = Path.Combine(context.SolutionDirectory, ".tests");
            context.FileSystem.CreateDirectory(directory);

            CopyTestFilesToWebsite(context, directory);

            RunTests(context, directory);
        }

        private void CopyTestFilesToWebsite([NotNull] IBuildContext context, [NotNull] string directory)
        {
            var targetDirectory = context.Configuration.Get(Constants.Configuration.Wwwroot);
            targetDirectory = PathHelper.Combine(targetDirectory, context.Configuration.Get(Constants.Configuration.WebsiteFolderName));
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

        private void RunTests([NotNull] IBuildContext context, [NotNull] string directory)
        {
            /*
            var sourceDirectory = Path.Combine(directory, "local");
            context.FileSystem.CreateDirectory(sourceDirectory);

            var sourceFileNames = context.FileSystem.GetFiles(sourceDirectory, SearchOption.AllDirectories).ToList();

            var references = new List<string>
            {
                Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty, "files\\website\\bin\\nunit.framework.dll")
            };

            var testRunner = new NUnitTestRunner();
            var testAssembly = testRunner.CompileAssembly(references, sourceFileNames);
            if (testAssembly == null)
            {
                return;
            }

            testRunner.RunTests(testAssembly);
            */

            var hostName = context.Configuration.GetString(Constants.Configuration.HostName).TrimEnd('/');
            var webTestRunnerUrl = "/sitecore/shell/client/Applications/Pathfinder/WebTestRunner.ashx";
            var url = hostName + "/" + webTestRunnerUrl;

            Request(context, url);
        }
    }
}
