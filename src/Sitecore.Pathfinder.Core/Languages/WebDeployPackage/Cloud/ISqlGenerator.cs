// © 2015-2017 by Jakob Christensen. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects.Items;

namespace Sitecore.Pathfinder.Languages.Webdeploy.Cloud
{
    public interface ISqlGenerator
    {
        [NotNull]
        string GenerateAddBlobStatements([NotNull] Blob blob);

        [NotNull]
        string GenerateAddItemStatements([NotNull] VersionedItem any);

        [NotNull]
        string GenerateAddRoleStatements([NotNull] Role role);

        [NotNull]
        string GenerateAppendStatements();

        [NotNull]
        string GeneratePrependStatements();
    }
}
