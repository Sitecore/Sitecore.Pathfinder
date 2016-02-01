// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Collections;
using Sitecore.Globalization;

namespace Sitecore.Data.Managers
{
    public class LanguageManager
    {
        [NotNull]
        public static Language DefaultLanguage
        {
            get { throw new System.NotImplementedException(); }
        }

        [CanBeNull]
        public static Language GetLanguage([NotNull] string language, [NotNull] Database database)
        {
            throw new NotImplementedException();
        }

        [NotNull]
        public static LanguageCollection GetLanguages([NotNull] Database database)
        {
            throw new NotImplementedException();
        }
    }
}
