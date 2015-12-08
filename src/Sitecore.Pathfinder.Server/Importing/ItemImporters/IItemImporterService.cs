// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Languages;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Items;

namespace Sitecore.Pathfinder.Importing.ItemImporters
{
    public interface IItemImporterService
    {
        [Diagnostics.NotNull]
        Item ImportItem([Diagnostics.NotNull] IProject project, [Diagnostics.NotNull] Data.Items.Item item, [Diagnostics.NotNull] ILanguage language, [Diagnostics.NotNull, ItemNotNull] IEnumerable<string> excludedFields);
    }
}
