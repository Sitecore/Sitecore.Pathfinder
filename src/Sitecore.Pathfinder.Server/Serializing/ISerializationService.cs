// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Data;

namespace Sitecore.Pathfinder.Serializing
{
    public interface ISerializationService
    {
        void SerializeItem([Diagnostics.NotNull] Database database, [Diagnostics.NotNull] ID itemId);
    }
}
