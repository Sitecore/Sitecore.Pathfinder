// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;

namespace Sitecore.Diagnostics
{
    public static class Log
    {
        public static void Audit([NotNull] object o, [NotNull] string text, [NotNull] string details)
        {
        }

        public static void Audit([NotNull] object o, [NotNull] string text)
        {
        }

        public static void Error([NotNull] string message, [NotNull] Exception exception, [NotNull] Type owner)
        {
        }
    }
}
