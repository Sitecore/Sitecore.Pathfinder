// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;

namespace Sitecore.Pathfinder.Code.Data
{
    public class Item : IEnumerable
    {
        public Item(string name, string path, string templateIdOrPath) 
        {
        }

        public Item(string name)
        {
        }

        public ICollection<Item> Children { get; } = new List<Item>();

        public Guid ID { get; set; } = Guid.Empty;

        public string Name { get; set; } = string.Empty;

        public Guid TemplateID { get; set; } = Guid.Empty;

        public IEnumerator GetEnumerator()
        {
            return null;
        }

        public void Add(string fieldName, string fieldValue)
        {
        }
        public void Add(Item item)
        {
        }
    }
}
