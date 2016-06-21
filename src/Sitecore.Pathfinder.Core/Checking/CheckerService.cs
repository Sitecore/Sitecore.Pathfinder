// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Checking
{
    [Export(typeof(ICheckerService))]
    public class CheckerService : ICheckerService
    {
        public enum CheckerSeverity
        {
            Disabled,

            Default,

            Information,

            Warning,

            Error
        }

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

        public virtual void CheckProject(IProject project)
        {
            var context = CompositionService.Resolve<ICheckerContext>().With(project);
            var treatWarningsAsErrors = Configuration.GetBool(Constants.Configuration.CheckProject.TreatWarningsAsErrors);
            var isMultiThreaded = Configuration.GetBool(Constants.Configuration.System.MultiThreaded);

            var checkers = GetCheckers().ToArray();
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

        private void EnableCheckers([NotNull, ItemNotNull] IEnumerable<CheckerInfo> checkers, [NotNull] string configurationKey)
        {
            foreach (var pair in Configuration.GetSubKeys(configurationKey))
            {
                if (pair.Key == BasedOn)
                {
                    var baseName = Constants.Configuration.ProjectRoleCheckers + ":" + Configuration.GetString(configurationKey + ":" + pair.Key);
                    EnableCheckers(checkers, baseName);
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

                    continue;
                }

                if (!checkers.Any(c => c.Name == key || c.Category == key))
                {
                    throw new ConfigurationException("Checker not found: " + key);
                }

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
        private IEnumerable<CheckerInfo> GetCheckers()
        {
            var checkers = Checkers.Select(c => new CheckerInfo(c)).ToList();

            // apply project roles
            var projectRoles = Configuration.GetStringList(Constants.Configuration.ProjectRole);
            foreach (var projectRole in projectRoles)
            {
                EnableCheckers(checkers, Constants.Configuration.ProjectRoleCheckers + ":" + projectRole);
            }

            // apply check-project:checkers
            EnableCheckers(checkers, Constants.Configuration.CheckProject.Checkers);

            // apply checkers
            EnableCheckers(checkers, Constants.Configuration.Checkers);

            return checkers.Where(c => c.Severity != CheckerSeverity.Disabled).ToArray();
        }

        protected class CheckerInfo
        {
            public CheckerInfo([NotNull] Func<ICheckerContext, IEnumerable<Diagnostic>> checker)
            {
                Checker = checker;

                Name = checker.Method.Name;
                Category = checker.Method.DeclaringType?.Name ?? string.Empty;

                if (Category.EndsWith("Checker"))
                {
                    Category = Category.Left(Category.Length - 7);
                }

                if (Category.EndsWith("Conventions"))
                {
                    Category = Category.Left(Category.Length - 11);
                }
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
