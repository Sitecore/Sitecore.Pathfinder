// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Languages
{
    public interface ILanguageService
    {
        [CanBeNull]
        ILanguage GetLanguageByExtension([NotNull] string extension);
    }
}
