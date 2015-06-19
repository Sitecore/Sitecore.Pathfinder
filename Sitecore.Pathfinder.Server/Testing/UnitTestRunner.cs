// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using Sitecore.IO;

namespace Sitecore.Pathfinder.Testing
{
    public class UnitTestRunner
    {
        private static DateTime _lastCompile = DateTime.MinValue;

        private static Assembly _testAssembly;

        public UnitTestRunner()
        {
            var startup = new Startup();
            var compositionContainer = startup.RegisterCompositionService();

            compositionContainer.SatisfyImportsOnce(this);
        }

        [ImportMany]
        [Diagnostics.NotNull]
        public IEnumerable<ITestRunner> TestRunners { get; protected set; }

        public void RunTests([Diagnostics.NotNull] string testRunnerName)
        {
            var testRunner = TestRunners.FirstOrDefault(t => string.Compare(t.Name, testRunnerName, StringComparison.OrdinalIgnoreCase) == 0);
            if (testRunner == null)
            {
                Console.WriteLine("Test Runner not found: " + testRunnerName);
                return;
            }

            var testDirectory = FileUtil.MapPath("/sitecore/shell/client/Applications/Pathfinder/Tests");
            if (!Directory.Exists(testDirectory))
            {
                Console.WriteLine("No tests were found");
                return;
            }

            var testAssembly = GetTestAssembly(testRunner, testDirectory);
            if (testAssembly == null)
            {
                return;
            }

            testRunner.RunTests(testAssembly);
        }

        [Diagnostics.CanBeNull]
        private Assembly GetTestAssembly([Diagnostics.NotNull] ITestRunner testRunner, [NotNull] string testDirectory)
        {
            var fileNames = Directory.GetFiles(testDirectory, "*.cs", SearchOption.AllDirectories).ToList();

            /*
            if (_testAssembly != null)
            {
                if (fileNames.All(f => File.GetLastWriteTimeUtc(f) < _lastCompile))
                {
                    return _testAssembly;
                }
            }
            */

            var references = new List<string>
            {
                FileUtil.MapPath("/bin/nunit.framework.dll")
            };

            _testAssembly = testRunner.CompileAssembly(references, fileNames);
            _lastCompile = DateTime.UtcNow;

            return _testAssembly;
        }
    }
}
