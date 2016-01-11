using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Building.DTO.WriteJsonProjectDiagnostic
{
    public class ProjectDiagnostic
    {
        [NotNull]
        public string ProjectId { get; set; }

        [NotNull]
        public string ProjectName { get; set; }

        [NotNull]
        public string ProjectDirectory { get; set; }

        [NotNull]
        [ItemNotNull]
        public IList<Diagnostic> Diagnostics { get; set; }
    }
}