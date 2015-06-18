// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
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

        public void RunTests()
        {
            var testRunner = new NUnitTestRunner();

            var testDirectory = FileUtil.MapPath("/sitecore/shell/client/Applications/Pathfinder/Tests");

            var testAssembly = GetTestAssembly(testRunner, testDirectory);
            if (testAssembly == null)
            {
                return;
            }

            testRunner.RunTests(testAssembly);
        }

        [CanBeNull]
        private Assembly GetTestAssembly([Diagnostics.NotNull] ITestRunner testRunner, [NotNull] string testDirectory)
        {
            var fileNames = Directory.GetFiles(testDirectory, "*.cs", SearchOption.AllDirectories).ToList();

            if (_testAssembly != null)
            {
                if (fileNames.All(f => File.GetLastWriteTimeUtc(f) < _lastCompile))
                {
                    return _testAssembly;
                }
            }

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
