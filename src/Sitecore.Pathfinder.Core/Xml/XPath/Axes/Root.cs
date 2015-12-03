// © 2015 Sitecore Corporation A/S. All rights reserved.

namespace Sitecore.Pathfinder.Xml.XPath.Axes
{
    public class Root : StepBase
    {
        public override object Evaluate(XPathExpression xpath, object context)
        {
            object result = null;

            Process(xpath, context, null, NextStep, ref result);

            return result;
        }
    }
}
