// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Data.Items;
using Sitecore.Jobs;
using Sitecore.Security.Accounts;

namespace Sitecore
{
    public static class Context
    {
        public static bool IsLoggedIn
        {
            get { throw new NotImplementedException(); }
        }

        [NotNull]
        public static Item Item
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        [CanBeNull]
        public static Job Job
        {
            get { throw new NotImplementedException(); }
        }

        [NotNull]
        public static User User
        {
            get { throw new NotImplementedException(); }
        }

        public static void Logout()
        {
            throw new NotImplementedException();
        }

        public static void SetActiveSite([NotNull] string siteName)
        {
            throw new NotImplementedException();
        }
    }
}
