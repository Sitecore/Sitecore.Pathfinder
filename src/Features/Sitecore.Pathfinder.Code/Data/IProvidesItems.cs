// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;

namespace Sitecore.Pathfinder.Code.Data
{
    public interface IProvidesItems
    {
        IEnumerable<Item> ProvideItems();
    }
}
