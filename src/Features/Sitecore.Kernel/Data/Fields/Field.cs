

// © 2015 Sitecore Corporation A/S. All rights reserved.
namespace Sitecore.Data.Fields
{
    public class Field
    {
        [NotNull]
        public string Name { get; private set; }

        [NotNull]
        public string Value { get; private set; }
    }
}