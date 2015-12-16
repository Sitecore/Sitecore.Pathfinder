// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Pathfinder.Languages;

namespace Sitecore.Pathfinder.Importing
{
    public abstract class FieldValueImporterBase : IFieldValueImporter
    {
        public abstract bool CanImport(Field field, Item item, ILanguage language, string value);

        public abstract string Import(Field field, Item item, ILanguage language, string value);
    }
}
