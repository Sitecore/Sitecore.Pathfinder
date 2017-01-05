// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;

namespace Sitecore.Pathfinder.Configuration.ConfigurationModel
{
    public interface IConfigurationSourceRoot : IConfiguration
    {
        IEnumerable<IConfigurationSource> Sources { get; }

        IConfigurationSourceRoot Add(IConfigurationSource configurationSource);

        void Reload();
    }
}
