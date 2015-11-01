// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Web.Mvc;
using System.Web.Security;
using Sitecore.Security.Accounts;
using Sitecore.Security.Authentication;
using Sitecore.SecurityModel.License;
using Sitecore.Web;

namespace Sitecore.Pathfinder.Extensions
{
    public static class ControllerExtensions
    {
        [Diagnostics.CanBeNull]
        public static ActionResult AuthenticateUser([Diagnostics.NotNull] this Controller controller)
        {
            var userName = WebUtil.GetQueryString("u");
            var password = WebUtil.GetQueryString("p");

            if (Context.IsLoggedIn)
            {
                if (Context.User.Name.Equals(userName, StringComparison.OrdinalIgnoreCase))
                {
                    return null;
                }

                Context.Logout();
            }

            if (!LicenseManager.HasContentManager && !LicenseManager.HasExpress)
            {
                return new HttpUnauthorizedResult("A required license is missing");
            }

            var validated = Membership.ValidateUser(userName, password);
            if (!validated)
            {
                return new HttpUnauthorizedResult("Unknown username or password");
            }

            var user = User.FromName(userName, true);
            if (!user.IsAdministrator && !user.IsInRole(Role.FromName("sitecore\\Sitecore Client Developing")))
            {
                return new HttpUnauthorizedResult("User is not an Administrator or a member of the sitecore\\Sitecore Client Developing role");
            }

            AuthenticationManager.Login(userName, password, true);
            return null;
        }
    }
}
