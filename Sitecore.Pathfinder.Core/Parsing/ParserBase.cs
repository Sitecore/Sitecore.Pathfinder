// © 2015 Sitecore Corporation A/S. All rights reserved.

namespace Sitecore.Pathfinder.Parsing
{
    public abstract class ParserBase : IParser
    {
        protected ParserBase(double sortorder)
        {
            Sortorder = sortorder;
        }

        public double Sortorder { get; }

        public abstract bool CanParse(IParseContext context);

        public abstract void Parse(IParseContext context);
    }
}
