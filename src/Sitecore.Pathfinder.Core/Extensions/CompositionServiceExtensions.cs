// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
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

                var typeLoadException = ex as ReflectionTypeLoadException;
                if (typeLoadException != null)
                {
                    foreach (var loaderException in typeLoadException.LoaderExceptions)
                    {
                        Console.WriteLine(loaderException.Message);
                    }
                }

                throw;
            }
        }

        [CanBeNull]
        public static T Resolve<T>([NotNull] this ICompositionService compositionService, [NotNull] string contractName)
        {
            var compositionContainer = (CompositionContainer)compositionService;

            try
            {
                var exportedValue = compositionContainer.GetExportedValue<T>(contractName);
                return exportedValue;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                var typeLoadException = ex as ReflectionTypeLoadException;
                if (typeLoadException != null)
                {
                    foreach (var loaderException in typeLoadException.LoaderExceptions)
                    {
                        Console.WriteLine(loaderException.Message);
                    }
                }
            }

            return default(T);
        }

        [NotNull, ItemNotNull]
        public static IEnumerable<T> ResolveMany<T>([NotNull] this ICompositionService compositionService)
        {
            var compositionContainer = (CompositionContainer)compositionService;

            try
            {
                var exportedValue = compositionContainer.GetExportedValues<T>();
                return exportedValue;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                var typeLoadException = ex as ReflectionTypeLoadException;
                if (typeLoadException != null)
                {
                    foreach (var loaderException in typeLoadException.LoaderExceptions)
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
