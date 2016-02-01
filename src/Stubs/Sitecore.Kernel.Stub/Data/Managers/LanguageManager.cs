// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using Sitecore.Globalization;

namespace Sitecore.Data.Managers
{
    public class LanguageManager
    {
        [NotNull]
        public static Language DefaultLanguage { get; private set; }

        [CanBeNull]
        public static Language GetLanguage([NotNull] string language, [NotNull] Database database)
        {
            throw new NotImplementedException();
        }

        [NotNull, ItemNotNull]
        public static IEnumerable<Language> GetLanguages([NotNull] Database database)
        {
            throw new NotImplementedException();
        }
    }
}
