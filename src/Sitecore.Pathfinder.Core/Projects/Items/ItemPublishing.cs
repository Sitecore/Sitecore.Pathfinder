// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;

namespace Sitecore.Pathfinder.Projects.Items
{
    public class ItemPublishing
    {
        [NotNull]
        private readonly Item _item;

        public ItemPublishing([NotNull] Item item)
        {
            _item = item;
        }

        public bool NeverPublish => string.Equals(_item[Constants.Fields.NeverPublish], "1", StringComparison.Ordinal);

        public DateTime PublishDate => _item[Constants.Fields.PublishDate].FromIsoToDateTime();

        public DateTime UnpublishDate => _item[Constants.Fields.UnpublishDate].FromIsoToDateTime();

        public DateTime ValidFrom => _item[Constants.Fields.ValidFrom].FromIsoToDateTime();

        public DateTime ValidTo => _item[Constants.Fields.ValidTo].FromIsoToDateTime();
    }
}
