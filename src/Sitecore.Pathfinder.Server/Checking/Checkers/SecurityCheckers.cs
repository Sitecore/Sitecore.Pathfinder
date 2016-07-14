// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml;
using Sitecore.Configuration;
using Sitecore.IO;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Security.AccessControl;
using Sitecore.Security.Accounts;
using Sitecore.Security.Authentication;
using Sitecore.SecurityModel.License;
using Sitecore.Xml;

namespace Sitecore.Pathfinder.Checking.Checkers
{
    public class SecurityCheckers : WebsiteChecker
    {
        [Export("Check"), NotNull, ItemNotNull]
        public IEnumerable<Diagnostic> AvoidDefaultAdminPassword([NotNull] ICheckerContext context)
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
        public IEnumerable<Diagnostic> AvoidExplicitSecurityDenies([NotNull] ICheckerContext context) => ForEachItem(context, AvoidExplicitSecurityDenies);

        [NotNull, ItemNotNull]
        public IEnumerable<Diagnostic> AvoidExplicitSecurityDenies([NotNull] ICheckerContext context, [NotNull] Item item, [NotNull] Data.Items.Item databaseItem)
        {
            var rules = databaseItem.Security.GetAccessRules();

            foreach (var rule in rules)
            {
                if (rule.SecurityPermission != SecurityPermission.DenyAccess)
                {
                    continue;
                }

                if (rule.AccessRight.IsFieldRight)
                {
                    continue;
                }

                yield return Warning(Msg.G1000, $"\"{rule.AccessRight.Title}\" is explicitly denied which is not recommended.", databaseItem, "To fix, use inheritance to restrict permission.");
            }
        }

        [Export("Check"), NotNull, ItemNotNull]
        public IEnumerable<Diagnostic> AvoidUsingPartnerLicense([NotNull] ICheckerContext context)
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

        [Export("Check"), NotNull, ItemNotNull]
        public IEnumerable<Diagnostic> DataFolderShouldNotBeUnderWebsiteFolder([NotNull] ICheckerContext context)
        {
            if (FileUtil.MapPath(Settings.DataFolder).StartsWith(FileUtil.MapPath("/"), StringComparison.InvariantCultureIgnoreCase))
            {
                yield return Warning(Msg.G1000, $"The Data folder is located inside the web site root in \"{Settings.DataFolder}\". This is a potential security risk.", "To fix, move the Data folder outside the web site root and change the \"DataFolder\" setting in the web.config to point to the new location.");
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
        public IEnumerable<Diagnostic> LockedByNonExistingUser([NotNull] ICheckerContext context) => ForEachItem(context, LockedByNonExistingUser);

        [NotNull, ItemNotNull]
        public IEnumerable<Diagnostic> LockedByNonExistingUser([NotNull] ICheckerContext context, [NotNull] Item item, [NotNull] Data.Items.Item databaseItem)
        {
            if (!databaseItem.Locking.IsLocked())
            {
                yield break;
            }

            var owner = databaseItem.Locking.GetOwner();
            if (string.IsNullOrEmpty(owner))
            {
                yield break;
            }

            if (User.Exists(owner))
            {
                yield break;
            }

            yield return Warning(Msg.G1000, "The item is locked by the non-existing user", databaseItem, $"{owner}. To fix, unlock the item.");
        }

        [Export("Check"), NotNull, ItemNotNull]
        public IEnumerable<Diagnostic> OwnedByNonExistingUser([NotNull] ICheckerContext context) => ForEachItem(context, OwnedByNonExistingUser);

        [NotNull, ItemNotNull]
        public IEnumerable<Diagnostic> OwnedByNonExistingUser([NotNull] ICheckerContext context, [NotNull] Item item, [NotNull] Data.Items.Item databaseItem)
        {
            if (!databaseItem.Locking.IsLocked())
            {
                yield break;
            }

            var owner = databaseItem[FieldIDs.Owner];
            if (string.IsNullOrEmpty(owner))
            {
                yield break;
            }

            if (User.Exists(owner))
            {
                yield break;
            }

            yield return Warning(Msg.G1000, "Item is owned by non-existing user", databaseItem, $"{owner}. To fix, remove the ownership");
        }

        [Export("Check"), NotNull, ItemNotNull]
        public IEnumerable<Diagnostic> SecurityOnUser([NotNull] ICheckerContext context) => ForEachItem(context, SecurityOnUser);

        [NotNull, ItemNotNull]
        public IEnumerable<Diagnostic> SecurityOnUser([NotNull] ICheckerContext context, [NotNull] Item item, [NotNull] Data.Items.Item databaseItem)
        {
            var rules = databaseItem.Security.GetAccessRules();

            foreach (var rule in rules)
            {
                if (rule.Account.AccountType != AccountType.User)
                {
                    continue;
                }

                if (rule.Account.Name == "sitecore\\Anonymous")
                {
                    continue;
                }

                if (rule.Account.Name == "$currentuser")
                {
                    continue;
                }

                yield return Warning(Msg.G1000, "Security assigned to user", databaseItem, $"\"{rule.AccessRight.Title}\" is assigned to the user \"{rule.Account.DisplayName}\". It is recommended to assign security to roles only. To fix, assign security to a role");
            }
        }

        [Export("Check"), NotNull, ItemNotNull]
        public IEnumerable<Diagnostic> SecurityOnNonExistingRole([NotNull] ICheckerContext context) => ForEachItem(context, SecurityOnNonExistingRole);

        [NotNull, ItemNotNull]
        public IEnumerable<Diagnostic> SecurityOnNonExistingRole([NotNull] ICheckerContext context, [NotNull] Item item, [NotNull] Data.Items.Item databaseItem)
        {
            var rules = databaseItem.Security.GetAccessRules();

            foreach (var rule in rules)
            {
                if (rule.Account.AccountType != AccountType.Role)
                {
                    continue;
                }

                if (Role.Exists(rule.Account.Name))
                {
                    continue;
                }

                var role = Role.FromName(rule.Account.Name);
                if (role != null)
                {
                    if (role.IsEveryone)
                    {
                        continue;
                    }

                    if (role.IsGlobal)
                    {
                        continue;
                    }
                }

                yield return Warning(Msg.G1000, "Security set on non-existing role", databaseItem, $"\"{rule.AccessRight.Title}\" is assigned to the non-existing role: {rule.Account.DisplayName}. To fix, remove the security assignment.");
            }
        }

        [Export("Check"), NotNull, ItemNotNull]
        public IEnumerable<Diagnostic> SecurityOnNonExistingUser([NotNull] ICheckerContext context) => ForEachItem(context, SecurityOnNonExistingUser);

        [NotNull, ItemNotNull]
        public IEnumerable<Diagnostic> SecurityOnNonExistingUser([NotNull] ICheckerContext context, [NotNull] Item item, [NotNull] Data.Items.Item databaseItem)
        {
            var rules = databaseItem.Security.GetAccessRules();

            foreach (var rule in rules)
            {
                if (rule.Account.AccountType != AccountType.User)
                {
                    continue;
                }

                if (User.Exists(rule.Account.Name))
                {
                    continue;
                }

                if (rule.Account.Name == "sitecore\\Anonymous")
                {
                    continue;
                }

                if (rule.Account.Name == "$currentuser")
                {
                    continue;
                }

                yield return Warning(Msg.G1000, "Security set on non-existing user", databaseItem, $"\"{rule.AccessRight.Title}\" assigned to the non-existing user \"{rule.Account.DisplayName}\". To fix, remove the security assignment.");
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
