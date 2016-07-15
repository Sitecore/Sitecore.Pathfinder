// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Sitecore.Configuration;
using Sitecore.Data.Items;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Checking
{
    public abstract class WebsiteChecker : Checker
    {
        [NotNull]
        protected Diagnostic Error(int msg, [NotNull] string text, [NotNull] Item item, [NotNull] string details = "", [NotNull, CallerMemberName] string checkerName = "")
        {
            return Error(msg, text, item.Paths.Path, TextSpan.Empty, details, checkerName);
        }

        [ItemNotNull, Diagnostics.NotNull]
        protected IEnumerable<Diagnostic> ForEachItem([NotNull] ICheckerContext context, [NotNull] Func<ICheckerContext, Projects.Items.Item, Item, IEnumerable<Diagnostic>> checker)
        {
            var diagnostics = new SynchronizedCollection<Diagnostic>();

            Parallel.ForEach(context.Project.Items, item =>
            {
                var database = Factory.GetDatabase(item.DatabaseName);

                var databaseItem = database.GetItem(new Data.ID(item.Uri.Guid));
                if (databaseItem != null)
                {
                    diagnostics.AddRange(checker(context, item, databaseItem));
                }
            });

            foreach (var diagnostic in diagnostics)
            {
                yield return diagnostic;
            }
        }

        [ItemNotNull, Diagnostics.NotNull]
        protected IEnumerable<Diagnostic> ForEachItemVersion([NotNull] ICheckerContext context, [NotNull] Func<ICheckerContext, Projects.Items.Item, Item, IEnumerable<Diagnostic>> checker)
        {
            var diagnostics = new SynchronizedCollection<Diagnostic>();

            Parallel.ForEach(context.Project.Items, item =>
            {
                var database = Factory.GetDatabase(item.DatabaseName);

                var databaseItem = database.GetItem(new Data.ID(item.Uri.Guid));
                if (databaseItem == null)
                {
                    return;
                }

                Parallel.ForEach(databaseItem.Versions.GetVersions(true), versionedItem => diagnostics.AddRange(checker(context, item, versionedItem)));
            });

            foreach (var diagnostic in diagnostics)
            {
                yield return diagnostic;
            }
        }

        [NotNull]
        protected Diagnostic Warning(int msg, [NotNull] string text, [NotNull] Item item, [NotNull] string details = "", [NotNull, CallerMemberName] string checkerName = "")
        {
            return Warning(msg, text, item.Paths.Path, TextSpan.Empty, details, checkerName);
        }
    }
}
