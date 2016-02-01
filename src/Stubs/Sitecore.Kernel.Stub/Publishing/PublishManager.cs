// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Collections;
using Sitecore.Data;
using Sitecore.Globalization;

namespace Sitecore.Publishing
{
    public static class PublishManager
    {
        [NotNull]
        public static ItemList GetPublishingTargets([NotNull] Database database)
        {
            throw new NotImplementedException();
        }

        public static Handle RebuildDatabase([NotNull] Database database, [NotNull] Database[] targetDatabases)
        {
            throw new NotImplementedException();
        }

        public static Handle PublishIncremental([NotNull] Database database, [NotNull] Database[] targetDatabases, [NotNull] Language[] languages)
        {
            throw new NotImplementedException();
        }

        public static Handle PublishSmart([NotNull] Database database, [NotNull] Database[] targetDatabases, [NotNull] Language[] languages)
        {
            throw new NotImplementedException();
        }

        public static Handle Republish([NotNull] Database database, [NotNull] Database[] targetDatabases, [NotNull] Language[] languages)
        {
            throw new NotImplementedException();
        }
    }
}
