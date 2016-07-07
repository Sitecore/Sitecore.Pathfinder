// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Xml;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Validators;
using Sitecore.Data.Validators.FieldValidators;
using Sitecore.Data.Validators.ItemValidators;
using Sitecore.IO;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Snapshots;
using Sitecore.Reflection;
using Sitecore.Xml;

namespace Sitecore.Pathfinder.Checking.Checkers
{
    public class WebsiteCheckers : Checker
    {
        [Export("Check"), NotNull, ItemNotNull]
        public IEnumerable<Diagnostic> CachingIsDisabled([NotNull] ICheckerContext context)
        {
            if (!Settings.Caching.Enabled)
            {
                yield return Warning(Msg.G1000, "Enable caching", "Caching is disabled. To fix, Set the setting \"Caching.Enabled\" to true in the web.config.");
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
                yield return Warning(severity, "Disable debugging in web.config", problem + "To fix, it is recommended to set the \"debug\" attribute to \"false\".");
            }
        }

        [Export("Check"), NotNull, ItemNotNull]
        public IEnumerable<Diagnostic> FolderIsReadOnly([NotNull] ICheckerContext context)
        {
            return CheckWritableWebFolder().Where(d => d != null);
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
                    yield return Warning(Msg.G1000, "Enable Html caching", string.Format("Html caching is disabled for the site \"{0}\". Performance may suffer. To fix, Set the \"cacheHtml\" setting to \"true\" for the site \"{0}\" in the web.config.", siteName));
                }
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
                yield return Warning(Msg.G1000, "Disable tracing web.config", problem + "To fix, set the \"enabled\" attribute of \"/configuration/system.web/trace\" to \"false\" in the web.config.");
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
                yield return Error(Msg.G1000, "Referenced types in web.config", $"The referenced type \"{typeString}\" does not exist. It is referenced from {path}. To fix, either correct the reference or remove it.");
            }
        }

        [Export("Check"), NotNull, ItemNotNull]
        public IEnumerable<Diagnostic> Validators([NotNull] ICheckerContext context)
        {
            foreach (var item in context.Project.Items)
            {
                var database = Factory.GetDatabase(item.DatabaseName);
                var databaseItem = database.GetItem(new Data.ID(item.Uri.Guid));
                if (databaseItem == null)
                {
                    continue;
                }

                if (StandardValuesManager.IsStandardValuesHolder(databaseItem))
                {
                    continue;
                }

                foreach (var diagnostic in Validate(item, databaseItem))
                {
                    yield return diagnostic;
                }
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

            yield return Warning(Msg.G1000, "Valid web stylesheet file", $"The \"WebStylesheet\" setting in the web.config points to the non-existing file: {Settings.WebStylesheet}. To fix, either create the file or set the setting \"WebStylesheet\" to blank.");
        }

        [CanBeNull]
        protected virtual Diagnostic CheckWritableDataFolder([NotNull] string folder)
        {
            folder = Path.Combine(FileUtil.MapPath(Settings.DataFolder), folder);

            return CheckWriteableFolder(folder);
        }

        [ItemCanBeNull, Diagnostics.NotNull]
        protected IEnumerable<Diagnostic> CheckWritableWebFolder()
        {
            yield return CheckWritableWebFolder("/upload");
            yield return CheckWritableWebFolder("/temp");
            yield return CheckWritableWebFolder("/sitecore/shell/Applications/debug");
            yield return CheckWritableWebFolder("/sitecore/shell/Controls/debug");
            yield return CheckWritableWebFolder("/layouts");
            yield return CheckWritableWebFolder("/xsl");
            yield return CheckWritableWebFolder("/App_Data");
            yield return CheckWritableWebFolder("/App_Data/MediaCache");
            yield return CheckWritableWebFolder("/indexes");

            yield return CheckWritableDataFolder("/audit");
            yield return CheckWritableDataFolder("/logs");
            yield return CheckWritableDataFolder("/viewstate");
            yield return CheckWritableDataFolder("/diagnostics");
        }

        [CanBeNull]
        protected virtual Diagnostic CheckWritableWebFolder([NotNull] string folder)
        {
            folder = FileUtil.MapPath(folder);

            return CheckWriteableFolder(folder);
        }

        [CanBeNull]
        protected virtual Diagnostic CheckWriteableFolder([NotNull] string folder)
        {
            if (!Directory.Exists(folder))
            {
                return null;
            }

            var fileName = Path.Combine(folder, "file.tmp");
            var n = 0;
            while (File.Exists(fileName))
            {
                fileName = Path.Combine(folder, $"file{n}.tmp");
                n++;
            }

            try
            {
                FileUtil.WriteToFile(fileName, "write test");
                FileUtil.Delete(fileName);
            }
            catch (Exception ex)
            {
                return Error(Msg.G1000, "Folder without required write permission", $"The folder \"{FileUtil.UnmapPath(folder, false)}\" is not writable by the ASP.NET user: {ex.Message}. To fix, Ensure that the ASP.NET user has write permission to the folder");
            }

            return null;
        }

        [ItemNotNull, NotNull]
        protected virtual IEnumerable<Diagnostic> Validate([NotNull] Item item, [NotNull] Data.Items.Item databaseItem)
        {
            var validatorCollection = new ValidatorCollection();
            foreach (BaseValidator validator in ValidatorManager.BuildValidators(ValidatorsMode.ValidatorBar, databaseItem))
            {
                // remove slow and obsolete validators
                if (validator is FullPageXHtmlValidator)
                {
                    continue;
                }

                if (validator is XhtmlValidator)
                {
                    continue;
                }

                if (validator is W3CXhtmlValidator)
                {
                    continue;
                }

                validatorCollection.Add(validator);
            }

            ValidatorManager.Validate(validatorCollection, new ValidatorOptions(true));

            foreach (BaseValidator validator in validatorCollection)
            {
                var text = validator.Text.TrimEnd('.').Replace('\r', ' ').Replace('\n', ' ');

                var details = validator.GetFieldDisplayName() ?? string.Empty;
                if (details == "[unknown]")
                {
                    details = string.Empty;
                }

                switch (validator.Result)
                {
                    case ValidatorResult.Suggestion:
                    case ValidatorResult.Warning:
                        yield return Warning(Msg.G1017, text, databaseItem.Paths.Path, TextSpan.Empty, details);
                        break;

                    case ValidatorResult.Error:
                    case ValidatorResult.CriticalError:
                    case ValidatorResult.FatalError:
                        yield return Error(Msg.G1017, text, databaseItem.Paths.Path, TextSpan.Empty, details);
                        break;
                }
            }
        }
    }
}
