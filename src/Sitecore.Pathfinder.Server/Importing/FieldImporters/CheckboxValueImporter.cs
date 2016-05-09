using System;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Pathfinder.Languages;

namespace Sitecore.Pathfinder.Importing.FieldImporters
{
    public class CheckboxValueImporter : FieldValueImporterBase
    {
        public override bool CanImport(Field field, Item item, ILanguage language, string value)
        {
            return string.Equals(field.Type, "checkbox", StringComparison.OrdinalIgnoreCase);
        }

        public override string Import(Field field, Item item, ILanguage language, string value)
        {
            var result = value;

            if (result == "1" || string.Equals(result, "true", StringComparison.OrdinalIgnoreCase))
            {
                result = "True";
            }
            else if (result == "0" || string.Equals(result, "false", StringComparison.OrdinalIgnoreCase) || string.IsNullOrEmpty(result))
            {
                result = "False";
            }

            return result;
        }
    }
}