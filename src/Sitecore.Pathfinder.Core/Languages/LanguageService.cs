// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Languages
{
    public class LanguageService : ILanguageService
    {
        [ImportingConstructor]
        public LanguageService([NotNull] [ItemNotNull] [ImportMany] IEnumerable<ILanguage> languages)
        {
            Languages = languages;
        }

        [NotNull]
        [ItemNotNull]
        protected IEnumerable<ILanguage> Languages { get; }

        public ILanguage GetLanguageByExtension(string extension)
        {
            return Languages.FirstOrDefault(l => l.CanHandleExtension(extension));
        }
    }
}
                