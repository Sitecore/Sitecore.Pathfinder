// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;

namespace Sitecore.Security.Accounts
{
    public class User
    {
        public bool IsAdministrator
        {
            get { throw new NotImplementedException(); }
        }

        [NotNull]
        public string Name
        {
            get { throw new NotImplementedException(); }
        }

        [NotNull]
        public static User FromName([NotNull] string userName, bool b)
        {
            throw new NotImplementedException();
        }

        public bool IsInRole([NotNull] Role role)
        {
            throw new NotImplementedException();
        }
    }
}
