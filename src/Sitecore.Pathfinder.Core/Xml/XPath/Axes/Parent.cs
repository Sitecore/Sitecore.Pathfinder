// © 2015 Sitecore Corporation A/S. All rights reserved.

namespace Sitecore.Pathfinder.Xml.XPath.Axes
{
    public class Parent : StepBase
    {
        public override object Evaluate(XPathExpression xpath, object context)
        {
            object result = null;

            var item = context as IXPathItem;
            if (item == null)
            {
                return null;
            }

            var parent = item.GetParent();
            if (parent == null)
            {
                return null;
            }

            Process(xpath, parent, null, NextStep, ref result);

            return result;
        }
    }
}
