// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Files;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Projects.Templates;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Checking.Conventions
{
    [InheritedExport, PartCreationPolicy(CreationPolicy.NonShared)]
    public abstract class ConventionsBase
    {
        public int ConventionCount { get; protected set; } = 1;

        [NotNull]
        protected ICheckerContext Context { get; private set; }

        [NotNull, ItemNotNull]
        protected IEnumerable<File> Files => Context.Project.Files;

        [NotNull, ItemNotNull]
        protected IEnumerable<Item> Items => Context.Project.Items;

        [NotNull, ItemNotNull]
        protected IEnumerable<IProject> Project
        {
            get { yield return Context.Project; }
        }

        [NotNull, ItemNotNull]
        protected IEnumerable<IProjectItem> ProjectItems => Context.Project.ProjectItems;

        [NotNull, ItemNotNull]
        protected IEnumerable<Template> Templates => Context.Project.Templates;

        [NotNull, ItemCanBeNull]
        protected abstract IEnumerable<IEnumerable<Diagnostic>> Check();

        [NotNull, ItemNotNull]
        protected virtual IEnumerable<Diagnostic> CheckDiagnostics()
        {
            return Check().SelectMany(i => i != null ? i.ToList() : Enumerable.Empty<Diagnostic>()).ToList();
        }

        [NotNull]
        protected Diagnostic Error([NotNull] string text)
        {
            return new Diagnostic(0, string.Empty, TextSpan.Empty, Severity.Error, text);
        }

        [NotNull]
        protected Diagnostic Error([NotNull] string text, [NotNull, ItemNotNull] params IHasSourceTextNodes[] textNodes)
        {
            var textNode = TraceHelper.GetTextNode(textNodes);
            return new Diagnostic(0, textNode.Snapshot.SourceFile.ProjectFileName, textNode.TextSpan, Severity.Error, text);
        }

        [NotNull]
        protected Diagnostic Information([NotNull] string text)
        {
            return new Diagnostic(0, string.Empty, TextSpan.Empty, Severity.Information, text);
        }

        [NotNull]
        protected Diagnostic Information([NotNull] string text, [NotNull, ItemNotNull] params IHasSourceTextNodes[] textNodes)
        {
            var textNode = TraceHelper.GetTextNode(textNodes);
            return new Diagnostic(0, textNode.Snapshot.SourceFile.ProjectFileName, textNode.TextSpan, Severity.Information, text);
        }

        [NotNull]
        protected Diagnostic Warning([NotNull] string text, [NotNull, ItemNotNull] params IHasSourceTextNodes[] textNodes)
        {
            var textNode = TraceHelper.GetTextNode(textNodes);
            return new Diagnostic(0, textNode.Snapshot.SourceFile.ProjectFileName, textNode.TextSpan, Severity.Warning, text);
        }

        [NotNull]
        protected Diagnostic Warning([NotNull] string text)
        {
            return new Diagnostic(0, string.Empty, TextSpan.Empty, Severity.Warning, text);
        }

        internal void Check([NotNull] ICheckerContext context)
        {
            Context = context;

            var diagnostics = CheckDiagnostics();

            foreach (var diagnostic in diagnostics)
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
