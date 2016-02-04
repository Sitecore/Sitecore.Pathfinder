// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;

namespace Sitecore.Data.Fields
{
    public abstract class CustomField
    {
        protected CustomField([NotNull] Field innerField)
        {
            throw new NotImplementedException();
        }

        [NotNull]
        public Field InnerField
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        [NotNull]
        public string Value
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }
    }
}
