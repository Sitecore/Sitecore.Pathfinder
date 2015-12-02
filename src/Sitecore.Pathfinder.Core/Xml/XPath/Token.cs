using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Xml.XPath
{
    public struct Token
    {
        public int Index;

        public int NumberValue;

        public int Type;

        [NotNull]
        public string Value;

        [NotNull]
        public string Whitespace;

        internal bool Empty;
    }
}