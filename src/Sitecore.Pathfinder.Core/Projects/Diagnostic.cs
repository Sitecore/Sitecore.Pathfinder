// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Diagnostics;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Projects
{
    [DebuggerDisplay("{GetType().Name,nq}: {Text}, {FileName}")]
    public class Diagnostic : IDiagnostic
    {
        [FactoryConstructor(typeof(IDiagnostic))]
        public Diagnostic(int msg, [NotNull] string fileName, TextSpan span, Severity severity, [NotNull] string text)
        {
            Msg = msg;
            FileName = fileName;
            Span = span;
            Severity = severity;
            Text = text;
        }

        public string FileName { get; }

        public int Msg { get; }

        public Severity Severity { get; }

        public TextSpan Span { get; }

        public string Text { get; }
    }
}
