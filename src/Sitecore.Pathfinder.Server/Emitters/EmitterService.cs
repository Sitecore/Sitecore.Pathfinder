// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.Configuration;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.SearchTypes;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Pathfinder.Emitting;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects;
using Sitecore.SecurityModel;
using Sitecore.Text;

namespace Sitecore.Pathfinder.Emitters
{
    [Export(typeof(IEmitterService))]
    public class EmitterService : IEmitterService
    {
        [ImportingConstructor]
        public EmitterService([NotNull] IConfiguration configuration, [NotNull] ICompositionService compositionService, [NotNull] IProjectService projectService)
        {
            Configuration = configuration;
            CompositionService = compositionService;
            ProjectService = projectService;
        }

        [NotNull]
        protected string BaseDirectory { get; private set; }

        [NotNull]
        protected ICompositionService CompositionService { get; }

        [NotNull]
        protected IConfiguration Configuration { get; }

        [NotNull]
        protected IProjectService ProjectService { get; }

        public virtual int Start()
        {
            PrepareWebsiteForPathfinder();

            // todo: support installation without configuration files
            var project = ProjectService.LoadProjectFromConfiguration();

            IProjectEmitter projectEmitter;
            if (Configuration.GetBool(Constants.Configuration.InstallPackage.ThreeWayMerge))
            {
                projectEmitter = CompositionService.Resolve<ThreeWayMergeProjectEmitter>().WithBaseDirectory(BaseDirectory);
            }
            else
            {
                projectEmitter = CompositionService.Resolve<ProjectEmitter>();
            }

            projectEmitter.Emit(project);

            if (Configuration.GetBool(Constants.Configuration.InstallPackage.DeleteProjectItems))
            {
                DeleteProjectItems(project);
            }

            return 0;
        }

        public IEmitterService With(string baseDirectory)
        {
            BaseDirectory = baseDirectory;
            return this;
        }

        protected virtual void DeleteProjectItems([NotNull] IProjectBase project)
        {
            if (!Configuration.GetBool(Constants.Configuration.InstallPackage.MarkItemsWithPathfinderProjectUniqueId, true))
            {
                return;
            }

            var projectUniqueId = project.ProjectUniqueId;

            foreach (var database in Factory.GetDatabases())
            {
                if (database.ReadOnly)
                {
                    continue;
                }

                var indexName = "sitecore_" + database.Name.ToLowerInvariant() + "_index";
                ISearchIndex index;
                try
                {
                    index = ContentSearchManager.GetIndex(indexName);
                }
                catch
                {
                    continue;
                }

                if (index == null)
                {
                    continue;
                }

                using (var context = index.CreateSearchContext())
                {
                    var ids = context.GetQueryable<SearchResultItem>().Where(item => item[ServerConstants.FieldNames.PathfinderProjectUniqueIds].Contains(projectUniqueId)).ToList().Select(i => i.ItemId).Distinct().ToList();

                    foreach (var id in ids)
                    {
                        var projectItem = project.ProjectItems.FirstOrDefault(i => i.Uri.Guid == id.Guid);
                        if (projectItem != null)
                        {
                            var i = projectItem as Projects.Items.Item;
                            if (i == null)
                            {
                                continue;
                            }

                            if (string.Equals(i.DatabaseName, database.Name, StringComparison.OrdinalIgnoreCase))
                            {
                                continue;
                            }

                            // todo: check properly for publishing databases
                            if (string.Equals(i.DatabaseName, "master", StringComparison.OrdinalIgnoreCase) && string.Equals(database.Name, "web", StringComparison.OrdinalIgnoreCase))
                            {
                                continue;
                            }
                        }

                        var item = database.GetItem(id);
                        if (item == null)
                        {
                            continue;
                        }

                        var uniqueIds = item[ServerConstants.FieldNames.PathfinderProjectUniqueIds];
                        if (uniqueIds.IndexOf('|') < 0)
                        {
                            // item is only used by this project, so it can be deleted.
                            item.Delete();
                        }
                        else
                        {
                            // remove project unique id from the field
                            var list = new ListString(uniqueIds);
                            list.Remove(projectUniqueId);

                            using (new EditContext(item))
                            {
                                item[ServerConstants.FieldNames.PathfinderProjectUniqueIds] = list.ToString();
                            }
                        }
                    }
                }
            }
        }

        protected virtual void PrepareWebsiteForPathfinder()
        {
            if (!Configuration.GetBool(Constants.Configuration.InstallPackage.MarkItemsWithPathfinderProjectUniqueId, true))
            {
                return;
            }

            using (new SecurityDisabler())
            {
                // ensure __PathfinderProjectUniqueIds field is in the Advanced section of all databases.
                foreach (var database in Factory.GetDatabases())
                {
                    if (database.ReadOnly)
                    {
                        continue;
                    }

                    var item = database.GetItem(ServerConstants.ItemIds.PathfinderProjectUniqueId);
                    if (item != null)
                    {
                        continue;
                    }

                    var parent = database.GetItem("/sitecore/templates/System/Templates/Sections/Advanced/Advanced");
                    if (parent == null)
                    {
                        continue;
                    }

                    var projectUniqueIdTemplateField = parent.Database.AddFromTemplateSynchronized(ServerConstants.FieldNames.PathfinderProjectUniqueIds, TemplateIDs.TemplateField, parent, ServerConstants.ItemIds.PathfinderProjectUniqueId);
                    using (new EditContext(projectUniqueIdTemplateField))
                    {
                        projectUniqueIdTemplateField["Shared"] = "1";
                    }
                }
            }
        }
    }
}
