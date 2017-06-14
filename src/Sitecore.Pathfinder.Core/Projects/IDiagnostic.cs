using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Projects
{
    public interface IDiagnostic
    {
        [NotNull]
        string FileName { get; }

        int Msg { get; }

        Severity Severity { get; }

        TextSpan Span { get; }

        [NotNull]
        string Text { get; }
    }
}