// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;

namespace Sitecore.Pathfinder.Projects.Items
{
    public class ItemStatistics
    {
        [NotNull]
        private readonly Item _item;

        public ItemStatistics([NotNull] Item item)
        {
            _item = item;
        }

        public DateTime Created => _item[Constants.Fields.Created].FromIsoToDateTime();

        [CanBeNull]
        public string CreatedBy => _item[Constants.Fields.CreatedBy];

        public DateTime Updated => _item[Constants.Fields.Updated].FromIsoToDateTime();

        [NotNull]
        public string UpdatedBy => _item[Constants.Fields.UpdatedBy];
    }
}
