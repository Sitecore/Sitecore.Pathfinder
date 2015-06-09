// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel;
using System.Linq;
using System.Xml.Linq;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Extensions
{
    public static class XElementExtensions
    {
        #region Public Methods and Operators

        [CanBeNull]
        public static XElement Element([NotNull] this XElement element, int index)
        {
            var elements = element.Elements();

            var count = 0;
            foreach (var e in elements)
            {
                if (count == index)
                {
                    return e;
                }

                count++;
            }

            return null;
        }

        [NotNull]
        public static string GetElementValue([NotNull] this XElement element, [NotNull] [Localizable(false)] string elementName)
        {
            return GetElementValue(element, elementName, string.Empty);
        }

        [NotNull]
        public static string GetAttributeValue([NotNull] this XElement element, [NotNull] [Localizable(false)] string attributeName)
        {
            if (!element.HasAttributes)
            {
                return string.Empty;
            }

            var attribute = element.Attribute(attributeName);

            return attribute?.Value ?? string.Empty;
        }

        [CanBeNull]
        public static string GetAttributeValue([NotNull] this XElement element, [NotNull] [Localizable(false)] string attributeName, [CanBeNull] string defaultValue)
        {
            if (!element.HasAttributes)
            {
                return defaultValue;
            }

            var attribute = element.Attribute(attributeName);

            return attribute?.Value ?? defaultValue;
        }

        [NotNull]
        public static string GetElementValue([NotNull] this XElement element, [NotNull] [Localizable(false)] string elementName, [NotNull] string defaultValue)
        {
            var e = element.Element(elementName);

            return e?.Value ?? defaultValue;
        }

        public static int GetElementValueInt([NotNull] this XElement element, [NotNull] [Localizable(false)] string elementName, int defaultValue = 0)
        {
            var e = element.Element(elementName);
            if (string.IsNullOrEmpty(e?.Value))
            {
                return defaultValue;
            }

            int result;
            if (int.TryParse(e.Value, out result))
            {
                return result;
            }

            return defaultValue;
        }

        public static bool HasAttribute([NotNull] this XElement element, [NotNull] [Localizable(false)] string attributeName)
        {
            if (!element.HasAttributes)
            {
                return false;
            }

            return element.Attribute(attributeName) != null;
        }

        [NotNull]
        public static string InnerText([NotNull] this XElement element)
        {
            return string.Join(string.Empty, element.Nodes().Select(n => n.ToString()).ToArray());
        }

        [CanBeNull]
        public static XElement ToXElement([NotNull] this string text, LoadOptions loadOptions = LoadOptions.None)
        {
            XDocument doc;
            try
            {
                doc = XDocument.Parse(text, loadOptions);
            }
            catch
            {
                return null;
            }

            return doc.Root;
        }

        #endregion
    }
}
