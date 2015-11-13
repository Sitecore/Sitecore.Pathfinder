// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Reflection;
using System.Web;
using NUnit.Core;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensibility;

namespace Sitecore.Pathfinder.Testing
{
    [Export(typeof(ITestRunner))]
    public class NUnitTestRunner : ITestRunner, EventListener
    {
        private int _executed;

        private int _failed;

        private int _passed;

        public string Name { get; } = "NUnit";

        public Assembly CompileAssembly(ICollection<string> references, IEnumerable<string> fileNames)
        {
            var compiler = new CsharpCompilerService();

            references.Add(typeof(HttpUtility).Assembly.Location);

            return compiler.GetUnitTestAssembly(references, fileNames);
        }

        public void RunFinished([NotNull] TestResult result)
        {
        }

        public void RunFinished([NotNull] Exception exception)
        {
        }

        public void RunStarted([NotNull] string name, int testCount)
        {
        }

        public void RunTests(Assembly testAssembly)
        {
            CoreExtensions.Host.InitializeService();

            var package = new TestPackage(testAssembly.Location);

            var testSuite = new TestSuiteBuilder().Build(package);

            TestExecutionContext.CurrentContext.TestPackage = package;

            var result = testSuite.Run(this, TestFilter.Empty);

            Console.WriteLine("{0} out of {1} tests run in {2} seconds", _executed, result.Test.TestCount, result.Time.ToString("#,##0.00"));

            if (_failed > 0)
            {
                Console.WriteLine("{0} {1} failed", _failed, _failed == 1 ? "test" : "tests");
            }

            var skipped = result.Test.TestCount - _executed;
            if (skipped > 0)
            {
                Console.WriteLine("{0} {1} skipped", skipped, skipped == 1 ? "test" : "tests");
            }
        }

        public void SuiteFinished([NotNull] TestResult result)
        {
        }

        public void SuiteStarted([NotNull] TestName testName)
        {
        }

        public void TestFinished([NotNull] TestResult result)
        {
            if (result.IsSuccess && result.Executed)
            {
                Console.Write("scc.cmd(0,0): information SCC0000: Pass: ");
                _passed++;
            }

            if (result.IsFailure && result.Executed)
            {
                Console.Write("scc.cmd(0,0): error SCC0000: Fail: ");
                _failed++;
            }

            Console.WriteLine(result.Test.TestName.Name);
            Console.WriteLine(result.Message);

            if (result.IsFailure)
            {
                Console.WriteLine(result.StackTrace);
            }

            if (result.Executed)
            {
                _executed++;
            }
        }

        public void TestOutput([NotNull] TestOutput testOutput)
        {
        }

        public void TestStarted([NotNull] TestName testName)
        {
        }

        public void UnhandledException([NotNull] Exception exception)
        {
        }
    }
}
