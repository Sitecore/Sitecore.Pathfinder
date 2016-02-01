// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;

namespace Sitecore.Mvc.Extensions
{
    public static class ObjectExtensions
    {
        public static TResult ValueOrDefault<T, TResult>([CanBeNull] this T source, [NotNull] Func<T, TResult> resultGetter) where T : class
        {
            throw new NotImplementedException();
        }
    }
}
