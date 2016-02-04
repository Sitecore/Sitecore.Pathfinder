// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;

namespace Sitecore.Configuration
{
    public static class Settings
    {
        [NotNull]
        public static string DataFolder
        {
            get { throw new NotImplementedException(); }
        }

        [NotNull]
        public static string GetSetting([NotNull] string name, [NotNull] string defaultValue)
        {
            throw new NotImplementedException();
        }

        public static string GetSetting([NotNull] string name)
        {
            throw new NotImplementedException();
        }
    }
}
