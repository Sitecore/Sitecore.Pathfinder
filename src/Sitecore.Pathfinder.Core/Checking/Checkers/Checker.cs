// © 2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Checking.Checkers
{
    public class Checker : CheckerBase
    {
        [ImportingConstructor]
        public Checker([NotNull, ItemNotNull, ImportMany("Check")] IEnumerable<Func<ICheckerContext, IEnumerable<Diagnostic>>> checkers)
        {
            Checkers = checkers;
        }

        [NotNull, ItemNotNull]
        protected IEnumerable<Func<ICheckerContext, IEnumerable<Diagnostic>>> Checkers { get; }

        public override void Check(ICheckerContext context)
        {
            foreach (var checker in Checkers)
            {
                var checkerName = checker.Method.Name;
                var checkerCategory = checker.Method.DeclaringType?.Name;

                if (context.DisabledCheckers.Any(c => string.Equals(c, checkerName, StringComparison.OrdinalIgnoreCase)))
                {
                    continue;
                }

                if (context.DisabledCategories.Any(c => string.Equals(c, checkerCategory, StringComparison.OrdinalIgnoreCase)))
                {
                    continue;
                }

                foreach (var diagnostic in checker(context))
                {
                    switch (diagnostic.Severity)
                    {
                        case Severity.Error:
                            context.Trace.TraceError(diagnostic.Msg, diagnostic.Text, diagnostic.FileName, diagnostic.Span);
                            break;
                        case Severity.Warning:
                            context.Trace.TraceWarning(diagnostic.Msg, diagnostic.Text, diagnostic.FileName, diagnostic.Span);
                            break;
                        case Severity.Information:
                            context.Trace.TraceInformation(diagnostic.Msg, diagnostic.Text, diagnostic.FileName, diagnostic.Span);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }
    }
}
