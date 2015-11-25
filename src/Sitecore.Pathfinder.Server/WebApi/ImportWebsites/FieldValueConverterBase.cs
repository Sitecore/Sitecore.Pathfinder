// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Pathfinder.Languages;

namespace Sitecore.Pathfinder.WebApi.ImportWebsites
{
    public abstract class FieldValueConverterBase : IFieldValueConverter
    {
        public abstract bool CanConvert(Field field, Item item, ILanguage language, string value);

        public abstract string Convert(Field field, Item item, ILanguage language, string value);
    }
}
