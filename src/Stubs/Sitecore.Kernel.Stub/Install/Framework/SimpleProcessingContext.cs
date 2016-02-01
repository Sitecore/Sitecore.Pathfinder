// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;

namespace Sitecore.Install.Framework
{
    public class SimpleProcessingContext : IProcessingContext
    {
        public void AddAspect<T>([NotNull] T instance) where T : class
        {
            throw new NotImplementedException();
        }
    }
}
