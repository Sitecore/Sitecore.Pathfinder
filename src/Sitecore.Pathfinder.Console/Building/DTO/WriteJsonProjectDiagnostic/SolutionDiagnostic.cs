using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Building.DTO.WriteJsonProjectDiagnostic
{
    public class SolutionDiagnostic
    {
        [NotNull]
        public string SolutionName { get; set; }

        [NotNull]
        public string SolutionLocation { get; set; }

        [NotNull]
        [ItemNotNull]
        public IList<ProjectDiagnostic> ProjectDiagnostics { get; set; }
    }
}