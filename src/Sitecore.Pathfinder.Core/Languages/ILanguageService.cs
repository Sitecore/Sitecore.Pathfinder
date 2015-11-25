// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Languages
{
    [InheritedExport]
    public interface ILanguageService
    {
        [CanBeNull]
        ILanguage GetLanguageByExtension([NotNull] string extension);
    }
}
