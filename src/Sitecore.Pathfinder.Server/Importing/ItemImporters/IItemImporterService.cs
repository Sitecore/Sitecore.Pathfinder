// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Data.Items;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Languages;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Templates;

namespace Sitecore.Pathfinder.Importing.ItemImporters
{
    public interface IItemImporterService
    {
        [Diagnostics.NotNull]
        Template ImportTemplate([Diagnostics.NotNull] IProject project, [Diagnostics.NotNull] Item item);

        [Diagnostics.NotNull]
        Projects.Items.Item ImportItem([Diagnostics.NotNull] IProject project, [Diagnostics.NotNull] Item item, [Diagnostics.NotNull] ILanguage language, [Diagnostics.NotNull, ItemNotNull] IEnumerable<string> excludedFields);
    }
}
