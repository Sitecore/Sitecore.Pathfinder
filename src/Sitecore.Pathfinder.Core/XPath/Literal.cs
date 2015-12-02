// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.XPath
{
    public class Literal : Opcode
    {
        public Literal([NotNull] string text)
        {
            if (text.StartsWith("\"", StringComparison.OrdinalIgnoreCase) && text.EndsWith("\"", StringComparison.OrdinalIgnoreCase))
            {
                text = text.Substring(1, text.Length - 2);
            }

            if (text.StartsWith("'", StringComparison.OrdinalIgnoreCase) && text.EndsWith("'", StringComparison.OrdinalIgnoreCase))
            {
                text = text.Substring(1, text.Length - 2);
            }

            Text = text;
        }

        [NotNull]
        public string Text { get; }

        public override object Evaluate(Query query, object context)
        {
            return Text;
        }
    }
}
