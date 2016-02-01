// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Globalization;

namespace Sitecore.Publishing
{
    public static class PublishManager
    {
        [NotNull, ItemNotNull]
        public static IEnumerable<Item> GetPublishingTargets([NotNull] Database database)
        {
            throw new NotImplementedException();
        }

        public static void RebuildDatabase([NotNull] Database database, [NotNull, ItemNotNull] Database[] targetDatabases)
        {
            throw new NotImplementedException();
        }

        public static void PublishIncremental([NotNull] Database database, [NotNull, ItemNotNull] Database[] targetDatabases, [NotNull, ItemNotNull] Language[] languages)
        {
            throw new NotImplementedException();
        }

        public static void PublishSmart([NotNull] Database database, [NotNull, ItemNotNull] Database[] targetDatabases, [NotNull, ItemNotNull] Language[] languages)
        {
            throw new NotImplementedException();
        }

        public static void Republish([NotNull] Database database, [NotNull, ItemNotNull] Database[] targetDatabases, [NotNull, ItemNotNull] Language[] languages)
        {
            throw new NotImplementedException();
        }
    }
}
