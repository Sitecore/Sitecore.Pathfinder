// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Composition;
using System.Composition.Hosting;
using System.Reflection;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Extensibility
{
    [Export(typeof(ICompositionService)), Shared]
    public class CompositionService : ICompositionService
    {
        [NotNull]
        protected CompositionHost CompositionHost { get; private set; }

        public T Resolve<T>()
        {
            try
            {
                return CompositionHost.GetExport<T>();
            }
            catch (Exception ex)
            {
                HandleException(ex);
                throw;
            }
        }

        public T Resolve<T>(string contractName)
        {
            try
            {
                return CompositionHost.GetExport<T>(contractName);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }

            return default(T);
        }

        public IEnumerable<T> ResolveMany<T>()
        {
            try
            {
                return CompositionHost.GetExports<T>();
            }
            catch (Exception ex)
            {
                HandleException(ex);
                throw;
            }
        }

        [NotNull]
        public CompositionService With([NotNull] CompositionHost compositionHost)
        {
            CompositionHost = compositionHost;
            return this;
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
