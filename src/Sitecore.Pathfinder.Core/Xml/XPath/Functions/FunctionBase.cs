// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Xml.XPath.Functions
{
    [InheritedExport(typeof(IFunction))]
    public abstract class FunctionBase : IFunction
    {
        protected FunctionBase([NotNull] string name)
        {
            Name = name;
        }

        public string Name { get; }

        public abstract object Evaluate(FunctionArgs args);
    }
}
