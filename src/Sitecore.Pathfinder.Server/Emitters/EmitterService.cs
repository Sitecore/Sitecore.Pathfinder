// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.Configuration;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.SearchTypes;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Emitters.Writers;
using Sitecore.Pathfinder.Emitting;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Snapshots;
using Sitecore.SecurityModel;
using Sitecore.Text;

namespace Sitecore.Pathfinder.Emitters
{
    [Export(typeof(IEmitterService))]
    public class EmitterService : IEmitterService
    {
        [ImportingConstructor]
        public EmitterService([NotNull] IConfiguration configuration, [NotNull] ICompositionService compositionService, [NotNull] ITraceService traceService, [NotNull] IProjectService projectService, [ImportMany, NotNull, ItemNotNull] IEnumerable<IEmitter> emitters)
        {
            Configuration = configuration;
            CompositionService = compositionService;
            Trace = traceService;
            ProjectService = projectService;
            Emitters = emitters;
        }

        [NotNull]
        protected IConfiguration Configuration { get; }

        [NotNull]
        protected ICompositionService CompositionService { get; }

        [NotNull, ItemNotNull]
        protected IEnumerable<IEmitter> Emitters { get; }

        [NotNull]
        protected IProjectService ProjectService { get; }

        [NotNull]
        protected ITraceService Trace { get; }

        public virtual int Start()
        {
            PrepareWebsiteForPathfinder();

            // todo: support installation without configuration files
            var project = ProjectService.LoadProjectFromConfiguration();

            Emit(project);

            DeleteProjectItems(project);

            return 0;
        }

        public IEmitterService With(string baseDirectory)
        {
            BaseDirectory = baseDirectory;
            return this;
        }

        [NotNull]
        protected string BaseDirectory { get; private set; }

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

                    var projectUniqueIdTemplateField = ItemManager.AddFromTemplate(ServerConstants.FieldNames.PathfinderProjectUniqueIds, TemplateIDs.TemplateField, parent, ServerConstants.ItemIds.PathfinderProjectUniqueId);
                    using (new EditContext(projectUniqueIdTemplateField))
                    {
                        projectUniqueIdTemplateField["Shared"] = "1";
                    }
                }
            }
        }

        protected virtual void DeleteProjectItems([NotNull] IProject project)
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

        protected virtual void Emit([NotNull] IProject project)
        {
            var context = CompositionService.Resolve<IEmitContext>().With(project);

            if (context.Configuration.GetBool(Constants.Configuration.InstallPackage.ShowDiagnostics))
            {
                var treatWarningsAsErrors = context.Configuration.GetBool(Constants.Configuration.CheckProject.TreatWarningsAsErrors);
                context.Trace.TraceDiagnostics(context.Project.Diagnostics, treatWarningsAsErrors);
            }

            var threeWayMerge = context.Configuration.GetBool(Constants.Configuration.InstallPackage.ThreeWayMerge);
            if (threeWayMerge)
            {
                LoadBaseProject(context);
            }

            var emitters = Emitters.OrderBy(e => e.Sortorder).ToList();
            var retries = new List<Tuple<IProjectItem, Exception>>();

            Emit(context, project, emitters, retries);
            EmitRetry(context, emitters, retries);

            if (retries.Count == 0 && threeWayMerge)
            {
                UpdateBaseProject(context);
            }
        }

        private void UpdateBaseProject([NotNull] IEmitContext context)
        {
            context.FileSystem.CreateDirectory(BaseDirectory);
            context.FileSystem.Mirror(context.Project.ProjectDirectory, BaseDirectory);
        }

        protected virtual void LoadBaseProject([NotNull] IEmitContext context)
        {
            if (string.IsNullOrEmpty(BaseDirectory))
            {
                return;
            }

            if (!context.FileSystem.DirectoryExists(BaseDirectory))
            {
                return;
            }

            var baseProject = ProjectService.LoadProjectFromNewHost(BaseDirectory);
            if (baseProject == null)
            {
                return;
            }

            Func<Data.Items.Item, FieldWriter, bool> threeWayMerge = delegate(Data.Items.Item item, FieldWriter fieldWriter)
            {
                var uri = new ProjectItemUri(fieldWriter.ItemWriter.DatabaseName, fieldWriter.ItemWriter.Guid);

                var baseItem = baseProject.FindQualifiedItem<Projects.Items.Item>(uri);
                if (baseItem == null)
                {
                    return true;
                }

                var projectItem = context.Project.FindQualifiedItem<Projects.Items.Item>(uri);
                if (projectItem == null)
                {
                    throw new InvalidOperationException("Oh no, something went really wrong");
                }

                var projectField = projectItem.Fields[fieldWriter.FieldName];
                if (projectField == null)
                {
                    throw new InvalidOperationException("Oh no, something went really wrong");
                }

                var templateField = projectField.TemplateField;

                Field baseField;
                if (templateField.Shared)
                {
                    baseField = baseItem.Fields.FirstOrDefault(f => f.FieldId == fieldWriter.FieldId && f.Language == string.Empty && f.Version == 0);
                }
                else if (templateField.Unversioned)
                {
                    baseField = baseItem.Fields.FirstOrDefault(f => f.FieldId == fieldWriter.FieldId && f.Language == fieldWriter.Language && f.Version == 0);
                }
                else 
                {
                    baseField = baseItem.Fields.FirstOrDefault(f => f.FieldId == fieldWriter.FieldId && f.Language == fieldWriter.Language && f.Version == fieldWriter.Version);
                }

                if (baseField == null)
                {
                    return true;
                }

                var databaseValue = item[fieldWriter.FieldName];

                return baseField.CompiledValue == databaseValue;
            };
        }

        protected virtual void Emit([NotNull] IEmitContext context, [NotNull] IProject project, [NotNull, ItemNotNull] List<IEmitter> emitters, [NotNull, ItemNotNull] ICollection<Tuple<IProjectItem, Exception>> retries)
        {
            foreach (var projectItem in project.ProjectItems)
            {
                EmitProjectItem(context, projectItem, emitters, retries);
            }
        }

        protected virtual void EmitProjectItem([NotNull] IEmitContext context, [NotNull] IProjectItem projectItem, [NotNull, ItemNotNull] List<IEmitter> emitters, [NotNull, ItemNotNull] ICollection<Tuple<IProjectItem, Exception>> retries)
        {
            foreach (var emitter in emitters)
            {
                if (!emitter.CanEmit(context, projectItem))
                {
                    continue;
                }

                try
                {
                    emitter.Emit(context, projectItem);
                }
                catch (RetryableEmitException ex)
                {
                    retries.Add(new Tuple<IProjectItem, Exception>(projectItem, ex));
                }
                catch (EmitException ex)
                {
                    Trace.TraceError(Msg.E1000, ex.Text, ex.FileName, ex.Span, ex.Details);
                }
                catch (Exception ex)
                {
                    retries.Add(new Tuple<IProjectItem, Exception>(projectItem, ex));
                }
            }
        }

        protected virtual void EmitRetry([NotNull] IEmitContext context, [NotNull, ItemNotNull] List<IEmitter> emitters, [NotNull, ItemNotNull] ICollection<Tuple<IProjectItem, Exception>> retries)
        {
            while (true)
            {
                var retryAgain = new List<Tuple<IProjectItem, Exception>>();
                foreach (var projectItem in retries.Reverse().Select(retry => retry.Item1))
                {
                    try
                    {
                        EmitProjectItem(context, projectItem, emitters, retryAgain);
                    }
                    catch (Exception ex)
                    {
                        retries.Add(new Tuple<IProjectItem, Exception>(projectItem, ex));
                    }
                }

                if (retryAgain.Count >= retries.Count)
                {
                    // did not succeed to install any items
                    retries = retryAgain;
                    break;
                }

                retries = retryAgain;
            }

            foreach (var retry in retries)
            {
                var projectItem = retry.Item1;
                var exception = retry.Item2;

                var buildException = exception as EmitException;
                if (buildException != null)
                {
                    Trace.TraceError(Msg.E1001, buildException.Text, buildException.FileName, buildException.Span, buildException.Details);
                }
                else if (exception != null)
                {
                    Trace.TraceError(Msg.E1002, exception.Message, projectItem.Snapshots.First().SourceFile.AbsoluteFileName, TextSpan.Empty);
                }
                else
                {
                    Trace.TraceError(Msg.E1003, Texts.An_error_occured, projectItem.Snapshots.First().SourceFile.AbsoluteFileName, TextSpan.Empty);
                }
            }
        }
    }
}
