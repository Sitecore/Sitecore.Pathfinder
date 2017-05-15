// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Xml.Linq;
using Sitecore.Extensions.XElementExtensions;

namespace Sitecore.Pathfinder.Emitting.Parsing
{
    public class Field
    {
        [NotNull]
        public string Blob { get; private set; }

        [NotNull]
        public string BlobExtension { get; private set; }

        public Guid Id { get; private set; }

        [NotNull]
        public string Language { get; private set; }

        [NotNull]
        public string Name { get; private set; }

        [NotNull]
        public string Value { get; private set; }

        public int Version { get; private set; }

        public static Field Parse(XElement fieldElement)
        {
            var field = new Field
            {
                Id = Guid.Parse(fieldElement.GetAttributeValue("id")),
                Value = fieldElement.Value,
                Name = fieldElement.GetAttributeValue("name"),
                Language = fieldElement.GetAttributeValue("language"),
                Blob = fieldElement.GetAttributeValue("blob"),
                BlobExtension = fieldElement.GetAttributeValue("blobExtension")
            };

            int version;
            if (int.TryParse(fieldElement.GetAttributeValue("version"), out version))
            {
                field.Version = version;
            }

            return field;
        }
    }
}
