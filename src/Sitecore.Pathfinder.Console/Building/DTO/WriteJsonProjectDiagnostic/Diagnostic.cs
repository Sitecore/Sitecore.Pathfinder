using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Building.DTO.WriteJsonProjectDiagnostic
{
    public class Diagnostic
    {
        public Severity Severity { get; set; }
        public int Code { get; set; }

        [NotNull]
        public string FileName { get; set; }

        [NotNull]
        public string TextSpan { get; set; }

        [NotNull]
        public string Text { get; set; }
    }
}