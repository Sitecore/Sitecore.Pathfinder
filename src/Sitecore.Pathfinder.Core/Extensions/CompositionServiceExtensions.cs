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
        public static void Dispose([NotNull] this ICompositionService compositionService)
        {
            var compositionContainer = (CompositionContainer)compositionService;

            compositionContainer.Dispose();
        }

        [NotNull]
        public static T New<T>([NotNull] this ExportFactory<T> factory) where T : class
        {
            return factory.CreateExport().Value;
        }

        [NotNull]
        public static T Resolve<T>([NotNull] this ICompositionService compositionService)
        {
            var compositionContainer = (CompositionContainer)compositionService;

            try
            {
                return compositionContainer.GetExportedValue<T>();
            }
            catch (Exception ex)
            {
                HandleException(ex);
                throw;
            }
        }

        [CanBeNull]
        public static T Resolve<T>([NotNull] this ICompositionService compositionService, [NotNull] string contractName)
        {
            var compositionContainer = (CompositionContainer)compositionService;

            try
            {
                return compositionContainer.GetExportedValue<T>(contractName);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }

            return default(T);
        }

        [NotNull, ItemNotNull]
        public static IEnumerable<T> ResolveMany<T>([NotNull] this ICompositionService compositionService)
        {
            var compositionContainer = (CompositionContainer)compositionService;

            try
            {
                return compositionContainer.GetExportedValues<T>();
            }
            catch (Exception ex)
            {
                HandleException(ex);
                throw;
            }
        }

        public static void Set<T>([NotNull] this ICompositionService compositionService, [NotNull] T value)
        {
            var compositionContainer = (CompositionContainer)compositionService;

            compositionContainer.ComposeExportedValue(value);
        }

        private static void HandleException([NotNull] Exception ex)
        {
            Console.WriteLine(ex.Message);

            var typeLoadException = ex as ReflectionTypeLoadException;
            if (typeLoadException == null)
            {
                return;
            }

            foreach (var loaderException in typeLoadException.LoaderExceptions)
            {
                Console.WriteLine($"    LoaderException: {loaderException.Message}");
            }
        }
    }
}
