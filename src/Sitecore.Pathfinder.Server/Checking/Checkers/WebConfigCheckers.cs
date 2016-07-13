// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml;
using Sitecore.Configuration;
using Sitecore.Data.Managers;
using Sitecore.IO;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects;
using Sitecore.Reflection;
using Sitecore.Sites;
using Sitecore.Xml;

namespace Sitecore.Pathfinder.Checking.Checkers
{
    public class WebConfigCheckers : Checker
    {
        [Export("Check"), NotNull, ItemNotNull]
        public IEnumerable<Diagnostic> CachingIsDisabled([NotNull] ICheckerContext context)
        {
            if (!Settings.Caching.Enabled)
            {
                yield return Warning(Msg.G1000, "Caching is disabled", "To fix, Set the setting \"Caching.Enabled\" to true in the web.config.");
            }
        }

        [Export("Check"), NotNull, ItemNotNull]
        public IEnumerable<Diagnostic> ContentEditorStylesheetIsMissing([NotNull] ICheckerContext context)
        {
            if (string.IsNullOrEmpty(Settings.WebEdit.ContentEditorStylesheet))
            {
                yield break;
            }

            if (FileUtil.Exists(Settings.WebEdit.ContentEditorStylesheet))
            {
                yield break;
            }

            yield return Warning(Msg.G1000, $"The \"WebEdit.ContentEditorStylesheet\" setting in the web.config points to the non-existing file: \"{Settings.WebEdit.ContentEditorStylesheet}\"", "To fix, either create the file or set the setting \"WebEdit.ContentEditorStylesheet\" to blank.");
        }

        [Export("Check"), NotNull, ItemNotNull]
        public IEnumerable<Diagnostic> CustomErrorsAreEnabled([NotNull] ICheckerContext context)
        {
            var severity = 0;
            var problem = string.Empty;

            var doc = new XmlDocument();
            doc.Load(FileUtil.MapPath("/web.config"));

            var node = doc.SelectSingleNode("/configuration/system.web/customErrors");
            if (node == null)
            {
                severity = 1;
                problem = "The configuration node \"/configuration/system.web/customErrors\" was not found in the web.config file.";
            }
            else
            {
                var value = XmlUtil.GetAttribute("enabled", node);

                if (value == "Off")
                {
                    severity = 2;
                    problem = "The \"mode\" attribute of the \"/configuration/system.web/customErrors\" element is set to \"Off\". This will show exceptions to end-users and is a potential security risk since a stack-trace is shown.";
                }
            }

            if (severity > 0)
            {
                yield return Warning(Msg.G1000, problem, "To fix, Set \"mode\" attribute of the \"/configuration/system.web/customErrors\" element to \"RemoteOnly\" or \"On\".");
            }
        }

        [Export("Check"), NotNull, ItemNotNull]
        public IEnumerable<Diagnostic> CustomWebsiteIsAfterWebsite([NotNull] ICheckerContext context)
        {
            var xmlDocument = Factory.GetConfiguration();

            var node = xmlDocument.SelectSingleNode("/sitecore/sites");
            if (node == null)
            {
                yield break;
            }

            var foundWebSite = false;
            foreach (XmlNode childNode in node.ChildNodes)
            {
                var siteName = XmlUtil.GetAttribute("name", childNode);
                if (siteName == "website")
                {
                    foundWebSite = true;
                    continue;
                }

                if (!foundWebSite)
                {
                    continue;
                }

                switch (siteName.ToLowerInvariant())
                {
                    case "shell":
                    case "login":
                    case "admin":
                    case "service":
                    case "modules_shell":
                    case "modules_website":
                    case "scheduler":
                    case "system":
                    case "testing":
                    case "publisher":
                        continue;
                }

                yield return Warning(Msg.G1000, $"The web site \"{siteName}\" is defined after the \"website\" site. This breaks the Site Resolver", $"To fix, move the \"{siteName}\" web site before the \"website\" site in the web.config");
            }
        }

        [Export("Check"), NotNull, ItemNotNull]
        public IEnumerable<Diagnostic> DebugIsEnabled([NotNull] ICheckerContext context)
        {
            var severity = 0;
            var problem = string.Empty;

            var doc = new XmlDocument();
            doc.Load(FileUtil.MapPath("/web.config"));

            var node = doc.SelectSingleNode("/configuration/system.web/compilation");
            if (node == null)
            {
                severity = 1;
                problem = "The configuration node \"/configuration/system.web/compilation\" was not found in the web.config file.";
            }
            else
            {
                var value = XmlUtil.GetAttribute("debug", node);

                if (value == "true")
                {
                    severity = 2;
                    problem = "The \"debug\" attribute of the compilation element is set to \"true\". This setting may decrease performance.";
                }
            }

            if (severity > 0)
            {
                yield return Warning(severity, "Disable debugging in web.config", problem + " To fix, it is recommended to set the \"debug\" attribute to \"false\".");
            }
        }

        [Export("Check"), NotNull, ItemNotNull]
        public IEnumerable<Diagnostic> DuplicateSiteHostName([NotNull] ICheckerContext context)
        {
            var hostNames = new Dictionary<string, SiteContext>();

            foreach (var siteName in Factory.GetSiteNames())
            {
                var site = Factory.GetSite(siteName);

                SiteContext s;

                if (!hostNames.TryGetValue(site.Name, out s))
                {
                    hostNames[site.Name] = site;
                    continue;
                }

                yield return Error(Msg.G1000, $"The host name \"{site.HostName}\" is used by both the \"{site.Name}\" and the \"{s.Name}\" site.", $"To fix, set the \"hostName\" attribute in the web.config on the \"{site.Name}\" or the \"{s.Name}\" site to a different host name.");
            }
        }

        [Export("Check"), NotNull, ItemNotNull]
        public IEnumerable<Diagnostic> HtmlCacheIsDisabled([NotNull] ICheckerContext context)
        {
            foreach (var siteName in Factory.GetSiteNames())
            {
                switch (siteName.ToLowerInvariant())
                {
                    case "shell":
                    case "login":
                    case "admin":
                    case "service":
                    case "modules_shell":
                    case "modules_website":
                    case "scheduler":
                    case "system":
                    case "testing":
                    case "publisher":
                        continue;
                }

                var site = Factory.GetSite(siteName);
                if (site == null)
                {
                    continue;
                }

                if (!site.CacheHtml)
                {
                    yield return Warning(Msg.G1000, $"Html caching is disabled for the site \"{siteName}\". Performance may suffer", $"To fix, set the \"cacheHtml\" setting to \"true\" for the site \"{siteName}\" in the web.config.");
                }
            }
        }

        [Export("Check"), NotNull, ItemNotNull]
        public IEnumerable<Diagnostic> SheerUiProfillingIsEnabled([NotNull] ICheckerContext context)
        {
            if (!Settings.Profiling.SheerUI)
            {
                yield return Warning(Msg.G1000, "Sheer UI profiling is enabled", "It is recommended that Sheer UI Profiling is disabled. To disable Sheer UI Profiling, open the web.config and set the setting \"Profiling.SheerUI\" to \"false\".");
            }
        }

        [Export("Check"), NotNull, ItemNotNull]
        public IEnumerable<Diagnostic> SiteContentLanguageIsUndefined([NotNull] ICheckerContext context)
        {
            foreach (var siteName in Factory.GetSiteNames())
            {
                var site = Factory.GetSite(siteName);

                if (site.ContentDatabase == null || site.ContentLanguage == null)
                {
                    continue;
                }

                if (LanguageManager.IsLanguageNameDefined(site.ContentDatabase, site.ContentLanguage.Name))
                {
                    continue;
                }

                yield return Error(Msg.G1000, $"The content language \"{site.ContentLanguage.Name}\" is specified on the site \"{site.Name}\" but is not defined in the site content database \"{site.ContentDatabase.Name}\"", $"To fix, set the \"ContentLanguage\" attribute on the site \"{site.Name}\" to an existing language.");
            }
        }

        [Export("Check"), NotNull, ItemNotNull]
        public IEnumerable<Diagnostic> SiteLanguageIsUndefined([NotNull] ICheckerContext context)
        {
            foreach (var siteName in Factory.GetSiteNames())
            {
                var site = Factory.GetSite(siteName);

                if (site.Database == null || string.IsNullOrEmpty(site.Language))
                {
                    continue;
                }

                if (LanguageManager.IsLanguageNameDefined(site.Database, site.Language))
                {
                    continue;
                }

                yield return Error(Msg.G1000, $"The language \"{site.Language}\" is specified on the site \"{site.Name}\" but is not defined in the site database \"{site.Database.Name}\"", $"To fix, Set the \"Language\" attribute on the site \"{site.Name}\" to an existing language.");
            }
        }

        [Export("Check"), NotNull, ItemNotNull]
        public IEnumerable<Diagnostic> TraceIsEnabled([NotNull] ICheckerContext context)
        {
            var severity = 0;
            var problem = string.Empty;

            var doc = new XmlDocument();
            doc.Load(FileUtil.MapPath("/web.config"));

            var node = doc.SelectSingleNode("/configuration/system.web/trace");
            if (node == null)
            {
                severity = 1;
                problem = "The configuration node \"/configuration/system.web/trace\" was not found in the web.config file.";
            }
            else
            {
                var value = XmlUtil.GetAttribute("enabled", node);

                if (value == "true")
                {
                    severity = 2;
                    problem = "The \"enabled\" attribute of \"/configuration/system.web/trace\" is set to \"true\". Tracing should only be enabled when debugging.";
                }
            }

            if (severity != 0)
            {
                yield return Warning(Msg.G1000, "Disable tracing in web.config", problem + "To fix, set the \"enabled\" attribute of \"/configuration/system.web/trace\" to \"false\" in the web.config.");
            }
        }

        [Export("Check"), NotNull, ItemNotNull]
        public IEnumerable<Diagnostic> TypeIsMissing([NotNull] ICheckerContext context)
        {
            var doc = Factory.GetConfiguration();

            var list = doc.SelectNodes("//*[@type]");
            if (list == null)
            {
                yield break;
            }

            foreach (XmlNode node in list)
            {
                var typeString = XmlUtil.GetAttribute("type", node);
                if (string.IsNullOrEmpty(typeString))
                {
                    continue;
                }

                if (typeString == "both")
                {
                    continue;
                }

                typeString = typeString.Replace(", mscorlib", string.Empty);
                typeString = typeString.Replace(",mscorlib", string.Empty);

                Type type;
                try
                {
                    type = ReflectionUtil.GetTypeInfo(typeString);
                }
                catch
                {
                    type = null;
                }

                if (type != null)
                {
                    continue;
                }

                var path = XmlUtil.GetPath(node);
                yield return Error(Msg.G1000, $"The referenced type \"{typeString}\" does not exist. It is referenced from {path}", "To fix, either correct the reference or remove it.");
            }
        }

        [Export("Check"), NotNull, ItemNotNull]
        public IEnumerable<Diagnostic> WebStylesheetIsMissing([NotNull] ICheckerContext context)
        {
            if (string.IsNullOrEmpty(Settings.WebStylesheet))
            {
                yield break;
            }

            if (FileUtil.Exists(Settings.WebStylesheet))
            {
                yield break;
            }

            yield return Warning(Msg.G1000, $"The \"WebStylesheet\" setting in the web.config points to the non-existing file: {Settings.WebStylesheet}", "To fix, either create the file or set the setting \"WebStylesheet\" to blank.");
        }
    }
}
