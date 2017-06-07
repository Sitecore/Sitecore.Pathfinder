// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Sitecore.Pathfinder.Checking.Checkers;
using Sitecore.Pathfinder.Configuration.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensibility;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Checking
{
    public enum CheckerSeverity
    {
        Disabled,

        Default,

        Information,

        Warning,

        Error
    }

    [Export(typeof(ICheckerService)), Shared]
    public class CheckerService : ICheckerService
    {
        private const string BasedOn = "based-on";

        [ImportingConstructor]
        public CheckerService([NotNull] IConfiguration configuration, [NotNull] ICompositionService compositionService, [NotNull, ItemNotNull, ImportMany] IEnumerable<IChecker> checkers)
        {
            Configuration = configuration;
            CompositionService = compositionService;

            var list = new List<CheckerInfo>();

            foreach (var checker in checkers)
            {
                foreach (var method in checker.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (method.GetCustomAttribute<CheckAttribute>() != null)
                    {
                        var checkerInfo = new CheckerInfo(method.DeclaringType.Name, method.Name, context => method.Invoke(checker, new object[]
                        {
                            context
                        }) as IEnumerable<Diagnostic>);
                        list.Add(checkerInfo);
                    }
                }
            }

            Checkers = list;
        }

        public IEnumerable<CheckerInfo> Checkers { get; }

        public int EnabledCheckersCount { get; protected set; }

        [NotNull]
        protected ICompositionService CompositionService { get; }

        [NotNull]
        protected IConfiguration Configuration { get; }

        public virtual void CheckProject(IProjectBase project)
        {
            var context = CompositionService.Resolve<ICheckerContext>().With(project);

            var treatWarningsAsErrors = Configuration.GetBool(Constants.Configuration.CheckProject.TreatWarningsAsErrors);
            var isMultiThreaded = Configuration.GetBool(Constants.Configuration.System.MultiThreaded, true);

            var checkers = GetCheckers(context).ToArray();

            CheckProject(context, checkers, isMultiThreaded, treatWarningsAsErrors);
        }

        public void CheckProject(IProjectBase project, IEnumerable<string> checkerNames)
        {
            var context = CompositionService.Resolve<ICheckerContext>().With(project);
            var treatWarningsAsErrors = Configuration.GetBool(Constants.Configuration.CheckProject.TreatWarningsAsErrors);
            var isMultiThreaded = Configuration.GetBool(Constants.Configuration.System.MultiThreaded, true);

            var checkers = Checkers.Where(c => checkerNames.Contains(c.Name)).ToArray();

            CheckProject(context, checkers, isMultiThreaded, treatWarningsAsErrors);
        }

        public virtual IEnumerable<CheckerInfo> GetEnabledCheckers()
        {
            var context = CompositionService.Resolve<ICheckerContext>();
            return GetCheckers(context);
        }

        protected virtual void CheckProject([NotNull] ICheckerContext context, [NotNull, ItemNotNull] CheckerInfo[] checkers, bool isMultiThreaded, bool treatWarningsAsErrors)
        {
            EnabledCheckersCount = checkers.Length;

            if (isMultiThreaded)
            {
                Parallel.ForEach(checkers, checker =>
                {
                    var diagnostics = checker.Checker(context).ToArray();
                    TraceDiagnostics(context, checker, diagnostics, treatWarningsAsErrors);
                });
            }
            else
            {
                foreach (var checker in checkers)
                {
                    var diagnostics = checker.Checker(context).ToArray();
                    TraceDiagnostics(context, checker, diagnostics, treatWarningsAsErrors);
                }
            }
        }

        protected virtual void EnableCheckers([NotNull] ICheckerContext context, [NotNull, ItemNotNull] IEnumerable<CheckerInfo> checkers, [NotNull] string configurationKey)
        {
            foreach (var pair in Configuration.GetSubKeys(configurationKey))
            {
                if (pair.Key == BasedOn)
                {
                    var baseName = Constants.Configuration.ProjectRoleCheckers + ":" + Configuration.GetString(configurationKey + ":" + pair.Key);
                    EnableCheckers(context, checkers, baseName);
                }
            }

            foreach (var pair in Configuration.GetSubKeys(configurationKey))
            {
                var key = pair.Key;
                if (key == BasedOn)
                {
                    continue;
                }

                var severity = CheckerSeverity.Default;
                switch (Configuration.GetString(configurationKey + ":" + key).ToLowerInvariant())
                {
                    case "disabled":
                        severity = CheckerSeverity.Disabled;
                        break;
                    case "enabled":
                        severity = CheckerSeverity.Default;
                        break;
                    case "information":
                        severity = CheckerSeverity.Information;
                        break;
                    case "warning":
                        severity = CheckerSeverity.Warning;
                        break;
                    case "error":
                        severity = CheckerSeverity.Error;
                        break;
                }

                if (key == "*")
                {
                    foreach (var c in checkers)
                    {
                        c.Severity = severity;
                    }

                    foreach (var c in context.Checkers)
                    {
                        context.Checkers[c.Key] = severity;
                    }

                    continue;
                }

                context.Checkers[key] = severity;

                foreach (var checker in checkers)
                {
                    if (checker.Name == key || checker.Category == key)
                    {
                        checker.Severity = severity;
                    }
                }
            }
        }

        [NotNull, ItemNotNull]
        protected virtual IEnumerable<CheckerInfo> GetCheckers([NotNull] ICheckerContext context)
        {
            foreach (var checker in Checkers)
            {
                context.Checkers[checker.Name] = CheckerSeverity.Default;
                context.Checkers[checker.Category] = CheckerSeverity.Default;
            }

            // apply project roles
            var projectRoles = Configuration.GetArray(Constants.Configuration.ProjectRole);
            foreach (var projectRole in projectRoles)
            {
                EnableCheckers(context, Checkers, Constants.Configuration.ProjectRoleCheckers + ":" + projectRole);
            }

            // apply checkers
            EnableCheckers(context, Checkers, Constants.Configuration.Checkers);

            return Checkers.Where(c => c.Severity != CheckerSeverity.Disabled).ToArray();
        }

        protected virtual void TraceDiagnostics([NotNull] ICheckerContext context, [NotNull] CheckerInfo checker, [NotNull, ItemNotNull] Diagnostic[] diagnostics, bool treatWarningsAsErrors)
        {
            if (checker.Severity == CheckerSeverity.Default)
            {
                context.Trace.TraceDiagnostics(diagnostics, treatWarningsAsErrors);
                return;
            }

            var severity = Severity.Information;
            switch (checker.Severity)
            {
                case CheckerSeverity.Information:
                    severity = Severity.Information;
                    break;
                case CheckerSeverity.Warning:
                    severity = Severity.Warning;
                    break;
                case CheckerSeverity.Error:
                    severity = Severity.Error;
                    break;
            }

            context.Trace.TraceDiagnostics(diagnostics, severity, treatWarningsAsErrors);
        }
    }
}
