// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Pathfinder.Languages;

namespace Sitecore.Pathfinder.Importing
{
    public class LinkFieldValueImporter : FieldValueImporterBase
    {
        public override bool CanImport(Field field, Item item, ILanguage language, string value)
        {
            return string.Equals(field.Type, "link", StringComparison.OrdinalIgnoreCase) || string.Equals(field.Type, "general link", StringComparison.OrdinalIgnoreCase);
        }

        public override string Import(Field field, Item item, ILanguage language, string value)
        {
            var linkField = new LinkField(field);
            return linkField.TargetItem?.Paths.Path ?? value;
        }
    }
}
