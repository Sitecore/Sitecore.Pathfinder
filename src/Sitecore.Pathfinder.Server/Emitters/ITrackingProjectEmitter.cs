// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Data.Items;
using Sitecore.Pathfinder.Emitters.Writers;

namespace Sitecore.Pathfinder.Emitters
{
    public interface ITrackingProjectEmitter
    {
        bool CanSetFieldValue([NotNull] Item item, [NotNull] FieldWriter fieldWriter, [NotNull] string fieldValue);
    }
}
