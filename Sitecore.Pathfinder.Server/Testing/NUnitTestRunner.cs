// © 2015 Sitecore Corporation A/S. All rights reserved.
namespace Sitecore.Pathfinder.Testing
{
    using System;
    using System.Reflection;
    using NUnit.Core;
    using NUnit.Core.Filters;

    public class NUnitTestRunner : EventListener
    {
        public void Run()
        {
            CoreExtensions.Host.InitializeService();

            var package = new TestPackage(Assembly.GetExecutingAssembly().Location);
            var testSuite = new TestSuiteBuilder().Build(package);
            var testName = TestName.Parse();
            testName.
            ITestFilter filter = new NameFilter(testName);
            var testResult = testSuite.Run(this, filter);
            testResult.
        }

        public void RunStarted(string name, int testCount)
        {
        }

        public void RunFinished(TestResult result)
        {
        }

        public void RunFinished(Exception exception)
        {
        }

        public void TestStarted(TestName testName)
        {
        }

        public void TestFinished(TestResult result)
        {
            result.
        }

        public void SuiteStarted(TestName testName)
        {
        }

        public void SuiteFinished(TestResult result)
        {
        }

        public void UnhandledException(Exception exception)
        {
        }

        public void TestOutput(TestOutput testOutput)
        {
        }
    }
}