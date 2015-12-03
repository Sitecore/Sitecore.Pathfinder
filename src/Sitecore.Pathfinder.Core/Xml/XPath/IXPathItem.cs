// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Xml.XPath
{
    public interface IXPathItem
    {
        [NotNull]
        string this[[NotNull] string name] { get; }

        [NotNull]
        string ItemId { get; }

        [NotNull]
        string ItemName { get; }

        [NotNull]
        string TemplateId { get; }

        [NotNull]
        string TemplateName { get; }

        [NotNull]
        string ItemPath { get; }

        [NotNull, ItemNotNull]
        IEnumerable<IXPathItem> GetChildren();

        [CanBeNull]
        IXPathItem GetParent();
    }
}
