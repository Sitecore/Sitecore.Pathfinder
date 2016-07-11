// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml;
using Sitecore.Configuration;
using Sitecore.IO;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects;
using Sitecore.Security.Authentication;
using Sitecore.SecurityModel.License;
using Sitecore.Xml;

namespace Sitecore.Pathfinder.Checking.Checkers
{
    public class SecurityCheckers : Checker
    {
        [Export("Check"), NotNull, ItemNotNull]
        public IEnumerable<Diagnostic> DataFolderIsUnderWebsiteFolder([NotNull] ICheckerContext context)
        {
            if (FileUtil.MapPath(Settings.DataFolder).StartsWith(FileUtil.MapPath("/"), StringComparison.InvariantCultureIgnoreCase))
            {
                yield return Warning(Msg.G1000, $"The Data folder is located inside the web site root in \"{Settings.DataFolder}\". This is a potential security risk.", "To fix, move the Data folder outside the web site root and change the \"DataFolder\" setting in the web.config to point to the new location.");
            }
        }

        [Export("Check"), NotNull, ItemNotNull]
        public IEnumerable<Diagnostic> DefaultAdminPassword([NotNull] ICheckerContext context)
        {
            var isValid = false;
            Exception exception = null;

            try
            {
                try
                {
                    isValid = AuthenticationManager.Login("sitecore\\admin", "b", false);
                }
                catch (NullReferenceException ex)
                {
                    if (!ex.StackTrace.Contains("FormsAuthentication.SetAuthCookie"))
                    {
                        throw;
                    }

                    isValid = true;
                }
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            if (exception != null)
            {
                yield return Warning(Msg.G1000, "An exception occured while checking the administrator password. The password may or may not have been changed: " + exception.Message, "The administrator password must be changed. To change it, open the User Manager in the Sitecore web client and change the password for \"sitecore\\admin\".");
                yield break;
            }

            if (!isValid)
            {
                yield return Warning(Msg.G1000, "The administrator password has not been changed", "The administrator password must be changed. To change it, open the User Manager in the Sitecore web client and change the password for \"sitecore\\admin\".");
            }
        }

        [Export("Check"), NotNull, ItemNotNull]
        public IEnumerable<Diagnostic> LicenseIsAboutToExpire([NotNull] ICheckerContext context)
        {
            var license = License.VerifiedLicense();
            if (license == null)
            {
                yield break;
            }

            foreach (var module in GetModules(license))
            {
                var node = license.SelectSingleNode("/verifiedlicense/module[name='" + module + "']");

                var expiration = XmlUtil.GetChildValue("expiration", node);
                if (expiration.Length >= 0)
                {
                    continue;
                }

                var expires = DateUtil.IsoDateToDateTime(expiration);
                if (expires <= DateTime.UtcNow + new TimeSpan(14, 0, 0, 0))
                {
                    continue;
                }

                yield return Warning(Msg.G1000, $"The license \"{module}\" expires in {(expires - DateTime.UtcNow).Days} days.", "The web site may stop working when the license expires. Contact your Sitecore partner to obtain a new license.");
            }
        }

        [Export("Check"), NotNull, ItemNotNull]
        public IEnumerable<Diagnostic> UsingPartnerLicense([NotNull] ICheckerContext context)
        {
            var license = License.VerifiedLicense();
            if (license == null)
            {
                yield break;
            }

            if (License.Purpose.Contains("The Sitecore partner"))
            {
                yield return Warning(Msg.G1000, $"The active license belongs to the partner \"{License.Licensee}\".", "To fix, obtain a valid customer license from your partner or Sitecore.");
            }
        }

        [NotNull, ItemNotNull]
        private IEnumerable<string> GetModules([NotNull, ItemNotNull] XmlDocument license)
        {
            var modules = new List<string>();

            var nodeList = license.SelectNodes("/verifiedlicense/module");
            if (nodeList == null)
            {
                return modules;
            }

            for (var n = 0; n < nodeList.Count; n++)
            {
                var node = nodeList[n];

                var name = XmlUtil.GetChildValue("name", node);

                modules.Add(name);
            }

            modules.Sort();

            return modules;
        }
    }
}
