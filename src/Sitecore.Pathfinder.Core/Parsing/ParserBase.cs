// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

namespace Sitecore.Pathfinder.Parsing
{
    public abstract class ParserBase : IParser
    {
        protected ParserBase(double priority)
        {
            Priority = priority;
        }

        public double Priority { get; }

        public abstract bool CanParse(IParseContext context);

        public abstract void Parse(IParseContext context);
    }
}
