// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Data.Items;
using Sitecore.Pathfinder.Emitters.Writers;
using Sitecore.Pathfinder.Emitting;

namespace Sitecore.Pathfinder.Emitters.ThreeWayMerge
{
    public interface ICanSetFieldValue 
    {
        [NotNull]
        IEmitContext WithBaseDirectory([NotNull] string baseDirectory);

        bool CanSetFieldValue([NotNull] Item item, [NotNull] FieldWriter fieldWriter, [NotNull] string fieldValue);
    }
}
