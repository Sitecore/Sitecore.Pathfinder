// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Pathfinder.Languages;

namespace Sitecore.Pathfinder.Importing
{
    public class IdFieldValueImporter : FieldValueImporterBase
    {
        public override bool CanImport(Field field, Item item, ILanguage language, string value)
        {
            return ID.IsID(value);
        }

        public override string Import(Field field, Item item, ILanguage language, string value)
        {
            var i = item.Database.GetItem(value);
            if (i != null)
            {
                value = i.Paths.Path;
            }

            return value;
        }
    }
}
