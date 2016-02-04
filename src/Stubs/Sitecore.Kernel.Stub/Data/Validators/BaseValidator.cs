// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;

namespace Sitecore.Data.Validators
{
    public abstract class BaseValidator
    {
        public ValidatorResult Result
        {
            get { throw new NotImplementedException(); }
        }

        [NotNull]
        public string Text
        {
            get { throw new NotImplementedException(); }
        }

        [NotNull]
        public abstract string GetFieldDisplayName();
    }
}
