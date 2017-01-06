// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Configuration.ConfigurationModel
{
    public interface IConfigurationSource
    {
        void Load();

        [ItemNotNull, NotNull]
        IEnumerable<string> ProduceSubKeys([ItemNotNull, NotNull] IEnumerable<string> earlierKeys, [NotNull] string prefix, [NotNull] string delimiter);

        void Set([NotNull] string key, [NotNull] string value);

        bool TryGet([NotNull] string key, [CanBeNull] out string value);
    }
}
