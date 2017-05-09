// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Sitecore.Extensions.XElementExtensions;

namespace Sitecore.Pathfinder.Emitting.Parsing
{
    public class Item
    {
        [NotNull]
        public List<Field> Fields = new List<Field>();

        [NotNull]
        public string Database { get; private set; }

        [NotNull]
        public string Icon { get; private set; }

        public Guid Id { get; private set; }

        [NotNull]
        public string Name { get; private set; }

        [NotNull]
        public string Path { get; set; }

        public int Sortorder { get; private set; }

        [NotNull]
        public string Template { get; private set; }

        public static Item Parse(XElement itemElement)
        {
            var item = new Item
            {
                Id = Guid.Parse(itemElement.GetAttributeValue("id")),
                Database = itemElement.GetAttributeValue("database"),
                Name = itemElement.GetAttributeValue("name"),
                Path = itemElement.GetAttributeValue("path"),
                Icon = itemElement.GetAttributeValue("icon"),
                Template = itemElement.GetAttributeValue("template"),
                Sortorder = int.Parse(itemElement.GetAttributeValue("sortorder"))
            };

            foreach (var fieldElement in itemElement.Elements())
            {
                item.Fields.Add(Field.Parse(fieldElement));
            }

            return item;
        }
    }
}
