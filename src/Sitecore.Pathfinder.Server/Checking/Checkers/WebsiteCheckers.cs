// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
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

namespace Sitecore.Pathfinder.Checking.Checkers
{
    public class WebsiteCheckers : Checker
    {
        [Export("Check"), NotNull, ItemNotNull]
        public IEnumerable<Diagnostic> FolderIsReadOnly([NotNull] ICheckerContext context)
        {
            return CheckWritableWebFolder().Where(d => d != null);
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
                return Error(Msg.G1000, $"The folder \"{FileUtil.UnmapPath(folder, false)}\" is not writable by the ASP.NET user: {ex.Message}", " To fix, Ensure that the ASP.NET user has write permission to the folder");
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
