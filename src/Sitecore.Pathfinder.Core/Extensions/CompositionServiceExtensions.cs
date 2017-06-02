// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Composition;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Extensions
{
    public static class CompositionServiceExtensions
    {
        [NotNull]
        public static T New<T>([NotNull] this ExportFactory<T> factory) where T : class => factory.CreateExport().Value;
    }
}
