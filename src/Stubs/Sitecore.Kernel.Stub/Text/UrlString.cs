// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Specialized;

namespace Sitecore.Text
{
    public class UrlString
    {
        public UrlString([NotNull] string source)
        {
            throw new NotImplementedException();
        }

        [CanBeNull]
        public string this[[NotNull] string parameterName]
        {
            get { throw new NotImplementedException(); }
        }

        [NotNull]
        public NameValueCollection Parameters
        {
            get { throw new NotImplementedException(); }
        }
    }
}
