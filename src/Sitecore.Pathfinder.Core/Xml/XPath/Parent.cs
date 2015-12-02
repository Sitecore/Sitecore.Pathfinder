// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Projects.Items;

namespace Sitecore.Pathfinder.Xml.XPath
{
    public class Parent : Step
    {
        public override object Evaluate(Query query, object context)
        {
            object result = null;

            var item = context as Item;
            if (item == null)
            {
                return null;
            }

            var parent = item.GetParent();
            if (parent == null)
            {
                return null;
            }

            Process(query, parent, null, NextStep, ref result);

            return result;
        }
    }
}
