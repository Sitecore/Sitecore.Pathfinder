// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using Sitecore.IO;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Testing
{
    public class UnitTestRunner
    {
        [Diagnostics.CanBeNull]
        private static Assembly _testAssembly;

        public UnitTestRunner()
        {
            var startup = new Startup();
            var compositionContainer = startup.RegisterCompositionService();

            compositionContainer.SatisfyImportsOnce(this);
        }

        [ImportMany]
        [Diagnostics.NotNull]
        [ItemNotNull]
        public IEnumerable<ITestRunner> TestRunners { get; protected set; }

        public void RunTests([Diagnostics.NotNull] string testRunnerName)
        {
            var testRunner = TestRunners.FirstOrDefault(t => string.Compare(t.Name, testRunnerName, StringComparison.OrdinalIgnoreCase) == 0);
            if (testRunner == null)
            {
                Console.WriteLine(Texts.Test_Runner_not_found__ + testRunnerName);
                return;
            }

            var testDirectory = FileUtil.MapPath("/sitecore/shell/client/Applications/Pathfinder/Tests");
            if (!Directory.Exists(testDirectory))
            {
                Console.WriteLine(Texts.No_tests_were_found);
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

            var references = new List<string>
            {
                FileUtil.MapPath("/bin/nunit.framework.dll")
            };

            _testAssembly = testRunner.CompileAssembly(references, fileNames);

            return _testAssembly;
        }
    }
}
