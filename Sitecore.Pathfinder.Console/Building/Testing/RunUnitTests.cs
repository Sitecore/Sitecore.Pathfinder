// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.ComponentModel.Composition;
using System.IO;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensibility;
using Sitecore.Pathfinder.IO;

namespace Sitecore.Pathfinder.Building.Testing
{
    [Export(typeof(ITask))]
    public class RunUnitTests : TaskBase
    {
        public RunUnitTests() : base("run-unittests")
        {
        }

        public override void Run(IBuildContext context)
        {
            context.Trace.TraceInformation(Texts.Running_unit_tests___);

            var directory = Path.Combine(context.SolutionDirectory, ".tests");
            context.FileSystem.CreateDirectory(directory);

            var baseFileName = Path.Combine(directory, "PathfinderTests");

            var fileName = GenerateTestFiles(context, baseFileName);

            CopyTestFilesToWebsite(context, baseFileName);

            RunTests(fileName);
        }

        private void CopyTestFilesToWebsite([NotNull] IBuildContext context, [NotNull] string baseFileName)
        {
            context.Trace.TraceInformation("Copying test files to website...");

            var testDirectory = context.Configuration.Get(Constants.Configuration.Wwwroot);
            testDirectory = PathHelper.Combine(testDirectory, context.Configuration.Get(Constants.Configuration.WebsiteFolderName));
            testDirectory = PathHelper.Combine(testDirectory, "sitecore\\shell\\client\\Applications\\Pathfinder\\Tests");

            context.FileSystem.CreateDirectory(testDirectory);
            var targetBaseFileName = Path.Combine(testDirectory, Path.GetFileName(baseFileName));

            context.FileSystem.Copy(baseFileName + ".ashx", targetBaseFileName + ".ashx");
            context.FileSystem.Copy(baseFileName + ".cs", targetBaseFileName + ".cs");
        }

        [NotNull]
        private string GenerateTestFiles([NotNull] IBuildContext context, [NotNull] string baseFileName)
        {
            var fileName = baseFileName + ".test.cs";

            if (!context.FileSystem.FileExists(fileName))
            {
                var generateUnitTests = new GenerateUnitTests();
                generateUnitTests.Run(context);

                if (!context.FileSystem.FileExists(fileName))
                {
                    return string.Empty;
                }

                return fileName;
            }

               
            return fileName;
        }

        private void RunTests([NotNull] string fileName)
        {
            var compiler = new ExtensionsCompiler();
            var assembly = compiler.GetUnitTestAssembly(fileName);
            if (assembly == null)
            {
                return;
            }

            var type = assembly.GetType("Sitecore.Pathfinder.Tests.PathfinderLocalTests");
            var instance = Activator.CreateInstance(type);

            foreach (var methodInfo in type.GetMethods())
            {
                if (!methodInfo.Name.StartsWith("Test"))
                {
                    continue;
                }

                methodInfo.Invoke(instance, null);
            }
        }
    }
}
