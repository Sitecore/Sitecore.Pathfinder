// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Sitecore.Pathfinder.Configuration.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
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

    [Export(typeof(ICheckerService))]
    public class CheckerService : ICheckerService
    {
        private const string BasedOn = "based-on";

        [ImportingConstructor]
        public CheckerService([NotNull] IConfiguration configuration, [NotNull] ICompositionService compositionService, [NotNull, ItemNotNull, ImportMany("Check")] IEnumerable<Func<ICheckerContext, IEnumerable<Diagnostic>>> checkers)
        {
            Configuration = configuration;
            CompositionService = compositionService;
            Checkers = checkers;
        }

        public IEnumerable<Func<ICheckerContext, IEnumerable<Diagnostic>>> Checkers { get; }

        public int EnabledCheckersCount { get; protected set; }

        [NotNull]
        protected ICompositionService CompositionService { get; }

        [NotNull]
        protected IConfiguration Configuration { get; }

        public virtual void CheckProject(IProjectBase project, IDiagnosticCollector diagnosticCollector)
        {
            var context = CompositionService.Resolve<ICheckerContext>().With(project, diagnosticCollector);

            var treatWarningsAsErrors = Configuration.GetBool(Constants.Configuration.CheckProject.TreatWarningsAsErrors);
            var isMultiThreaded = Configuration.GetBool(Constants.Configuration.System.MultiThreaded, true);

            var checkers = GetCheckers(context).ToArray();

            CheckProject(context, checkers, isMultiThreaded, treatWarningsAsErrors);
        }

        public void CheckProject(IProjectBase project, IDiagnosticCollector diagnosticCollector, IEnumerable<string> checkerNames)
        {
            var context = CompositionService.Resolve<ICheckerContext>().With(project, diagnosticCollector);
            var treatWarningsAsErrors = Configuration.GetBool(Constants.Configuration.CheckProject.TreatWarningsAsErrors);
            var isMultiThreaded = Configuration.GetBool(Constants.Configuration.System.MultiThreaded, true);

            var checkers = Checkers.Where(c => checkerNames.Contains(c.Method.Name)).Select(c => new CheckerInfo(c)).ToArray();

            CheckProject(context, checkers, isMultiThreaded, treatWarningsAsErrors);
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

        public virtual IEnumerable<Func<ICheckerContext, IEnumerable<Diagnostic>>> GetEnabledCheckers()
        {
            var context = CompositionService.Resolve<ICheckerContext>();
            return GetCheckers(context).Select(c => c.Checker);
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
            var checkers = Checkers.Select(c => new CheckerInfo(c)).ToList();

            foreach (var checker in checkers)
            {
                context.Checkers[checker.Name] = CheckerSeverity.Default;
                context.Checkers[checker.Category] = CheckerSeverity.Default;
            }

            // apply project roles
            var projectRoles = Configuration.GetStringList(Constants.Configuration.ProjectRole);
            foreach (var projectRole in projectRoles)
            {
                EnableCheckers(context, checkers, Constants.Configuration.ProjectRoleCheckers + ":" + projectRole);
            }

            // apply check-project:checkers
            EnableCheckers(context, checkers, Constants.Configuration.CheckProject.Checkers);

            // apply checkers
            EnableCheckers(context, checkers, Constants.Configuration.Checkers);

            return checkers.Where(c => c.Severity != CheckerSeverity.Disabled).ToArray();
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

        [DebuggerDisplay("{GetType().Name,nq}: {Name}, {Category}")]
        protected class CheckerInfo
        {
            public CheckerInfo([NotNull] Func<ICheckerContext, IEnumerable<Diagnostic>> checker)
            {
                Checker = checker;

                Name = checker.Method.Name;
                Category = checker.Method.DeclaringType?.Name ?? string.Empty;
            }

            [NotNull]
            public string Category { get; }

            [NotNull]
            public Func<ICheckerContext, IEnumerable<Diagnostic>> Checker { get; }

            [NotNull]
            public string Name { get; }

            public CheckerSeverity Severity { get; set; } = CheckerSeverity.Default;
        }
    }
}
