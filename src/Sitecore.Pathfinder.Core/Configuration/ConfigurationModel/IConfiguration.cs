// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Configuration.ConfigurationModel
{
    public interface IConfiguration
    {
        [CanBeNull]
        string this[[NotNull] string key] { get; set; }

        [CanBeNull]
        string Get([NotNull] string key);

        [CanBeNull]
        IConfiguration GetSubKey([NotNull] string key);

        [NotNull]
        IEnumerable<KeyValuePair<string, IConfiguration>> GetSubKeys();

        [NotNull]
        IEnumerable<KeyValuePair<string, IConfiguration>> GetSubKeys([NotNull] string key);

        void Set([NotNull] string key, [NotNull] string value);

        bool TryGet([NotNull] string key, [CanBeNull] out string value);
    }
}
