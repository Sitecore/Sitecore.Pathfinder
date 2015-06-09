// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Extensions
{
    public static class CompositionServiceExtensions
    {
        public static T Resolve<T>([NotNull] this ICompositionService compositionService)
        {
            var compositionContainer = (CompositionContainer)compositionService;

            return compositionContainer.GetExportedValue<T>();
        }

        public static void Set<T>([NotNull] this ICompositionService compositionService, T value)
        {
            var compositionContainer = (CompositionContainer)compositionService;

            compositionContainer.ComposeExportedValue(value);
        }
    }
}
