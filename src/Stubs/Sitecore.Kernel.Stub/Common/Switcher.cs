// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;

namespace Sitecore.Common
{
    public class Switcher<T> : Switcher<T, T>
    {
        protected Switcher()
        {
            throw new NotImplementedException();
        }

        public Switcher([NotNull] T objectToSwitchTo) : base(objectToSwitchTo)
        {
        }
    }

    public class Switcher<TValue, TSwitchType> : IDisposable
    {
        public Switcher([NotNull] TValue objectToSwitchTo)
        {
            throw new NotImplementedException();
        }

        protected Switcher()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
