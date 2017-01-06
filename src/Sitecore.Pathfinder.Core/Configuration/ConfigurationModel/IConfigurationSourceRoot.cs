// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Configuration.ConfigurationModel
{
    public interface IConfigurationSourceRoot : IConfiguration
    {
        [ItemNotNull, NotNull]
        IEnumerable<IConfigurationSource> Sources { get; }

        [NotNull]
        IConfigurationSourceRoot Add([NotNull] IConfigurationSource configurationSource);

        void Reload();
    }
}
