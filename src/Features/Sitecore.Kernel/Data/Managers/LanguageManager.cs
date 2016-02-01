// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Globalization;

namespace Sitecore.Data.Managers
{
    public class LanguageManager
    {
        [NotNull, ItemNotNull]
        public static IEnumerable<Language> GetLanguages([NotNull] Database database)
        {
        }

        public static Language GetLanguage(string language, Database database)
        {
            return null;
        }

        public static Language DefaultLanguage { get; private set; }
    }
}
                                                               