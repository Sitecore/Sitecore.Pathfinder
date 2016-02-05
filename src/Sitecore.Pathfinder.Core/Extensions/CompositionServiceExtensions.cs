// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Extensions
{
    public static class CompositionServiceExtensions
    {
        [NotNull]
        public static T Resolve<T>([NotNull] this ICompositionService compositionService)
        {
            var compositionContainer = (CompositionContainer)compositionService;

            try
            {
                var exportedValue = compositionContainer.GetExportedValue<T>();
                return exportedValue;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                if (ex is ReflectionTypeLoadException)
                {
                    var typeLoadException = ex as ReflectionTypeLoadException;
                    var loaderExceptions = typeLoadException.LoaderExceptions;

                    foreach (var loaderException in loaderExceptions)
                    {
                        Console.WriteLine(loaderException.Message);
                    }
                }

                throw;
            }
        }

        public static void Set<T>([NotNull] this ICompositionService compositionService, [NotNull] T value)
        {
            var compositionContainer = (CompositionContainer)compositionService;

            compositionContainer.ComposeExportedValue(value);
        }
    }
}
