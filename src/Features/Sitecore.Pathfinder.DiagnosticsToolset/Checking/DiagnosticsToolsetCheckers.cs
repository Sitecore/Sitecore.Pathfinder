// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Security.Principal;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.DiagnosticsToolset.Core.Categories;
using Sitecore.DiagnosticsToolset.DataProvider.LocalServer;
using Sitecore.DiagnosticsToolset.TestRunner;
using Sitecore.DiagnosticsToolset.TestRunner.Base;
using Sitecore.DiagnosticsToolset.TestRunner.Reporting;
using Sitecore.Pathfinder.Checking;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.DiagnosticsToolset.Checking
{
    public class DiagnosticsToolsetCheckers : Checker
    {
        [ImportingConstructor]
        public DiagnosticsToolsetCheckers([NotNull] IConfiguration configuration)
        {
            Configuration = configuration;
        }

        [NotNull]
        protected IConfiguration Configuration { get; }

        protected virtual bool IsElevated
        {
            get
            {
                var identity = WindowsIdentity.GetCurrent();
                return new WindowsPrincipal(identity).IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

        [NotNull, ItemNotNull, Export("Check")]
        public IEnumerable<Diagnostic> DiagnosticsToolsetChecker([NotNull] ICheckerContext context)
        {
            if (!IsElevated)
            {
                context.Trace.TraceWarning(Msg.C1127, Texts.The_DiagnosticsToolset_checker_requires_elevated_rights_to_run);
                yield break;
            }

            var results = new List<ITestReport>();

            RunTests(context, results);

            foreach (var result in results)
            {
                var checkerName = GetCheckerName(result);

                foreach (var testResult in result.Results.All)
                {
                    switch (testResult.State)
                    {
                        case TestResultState.Error:
                            yield return Error(Msg.C1128, testResult.Message, string.Empty, checkerName);
                            break;

                        case TestResultState.Warning:
                            yield return Warning(Msg.C1128, testResult.Message, string.Empty, checkerName);
                            break;

                        case TestResultState.CannotRun:
                        case TestResultState.Unidentified:
                            yield return Information(Msg.C1128, testResult.Message, string.Empty, checkerName);
                            break;
                    }

                }
            }
        }

        [NotNull]
        protected virtual string GetCheckerName([NotNull] ITestReport result)
        {
            var names = new List<string>
            {
                nameof(DiagnosticsToolsetChecker),
                result.Owner.Name
            };

            names.AddRange(result.Categories.Select(c => c.Name));

            return string.Join(", ", names);
        }

        [NotNull]
        protected virtual string GetSiteName()
        {
            var siteName = Configuration.GetString(Constants.Configuration.HostName);
            var n = siteName.IndexOf("://", StringComparison.Ordinal);
            if (n >= 0)
            {
                siteName = siteName.Mid(n + 3);
            }

            return siteName;
        }

        protected virtual void RunTests([NotNull] ICheckerContext context, [NotNull, ItemNotNull] List<ITestReport> results)
        {
            var siteName = GetSiteName();
            var categories = Category.GetAll();
            var tests = TestManager.GetTests().ToArray();

            using (var dataProvider = new LocalWebServerDataProvider(siteName, categories))
            {
                var testRunner = new TestRunner() as ITestRunner;
                var testContext = testRunner.CreateContext(dataProvider);

                foreach (var test in tests)
                {
                    var severity = CheckerSeverity.Default;
                    CheckerSeverity s;

                    if (context.Checkers.TryGetValue(test.Name, out s))
                    {
                        severity = s;
                    }

                    foreach (var category in test.Categories)
                    {
                        // todo: last category wins, which is not correct
                        if (context.Checkers.TryGetValue(category.Name, out s))
                        {
                            severity = s;
                        }
                    }

                    if (severity == CheckerSeverity.Disabled)
                    {
                        continue;
                    }

                    var result = testRunner.RunTest(test, categories, testContext);

                    results.Add(result);

                    testContext.Reset();
                }
            }
        }
    }
}
