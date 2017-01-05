// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;

namespace Sitecore.Pathfinder.Configuration.ConfigurationModel
{
    public interface IConfigurationSource
    {
        void Load();

        IEnumerable<string> ProduceSubKeys(IEnumerable<string> earlierKeys, string prefix, string delimiter);

        void Set(string key, string value);

        bool TryGet(string key, out string value);
    }
}
