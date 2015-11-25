// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Pathfinder.Languages;

namespace Sitecore.Pathfinder.WebApi.ImportWebsites
{
    [InheritedExport]
    public interface IFieldValueConverter
    {
        bool CanConvert([Diagnostics.NotNull] Field field, [Diagnostics.NotNull] Item item, [Diagnostics.NotNull] ILanguage language, [Diagnostics.NotNull] string value);

        [Diagnostics.NotNull]
        string Convert([Diagnostics.NotNull] Field field, [Diagnostics.NotNull] Item item, [Diagnostics.NotNull] ILanguage language, [Diagnostics.NotNull] string value);
    }
}
