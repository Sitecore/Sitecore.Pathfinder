// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Emitting;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Templates;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Emitters
{
    [Export(typeof(ProjectEmitter))]
    public class ProjectEmitter : IProjectEmitter
    {
        [NotNull]
        private readonly object _syncObject = new object();

        [ImportingConstructor]
        public ProjectEmitter([NotNull] IConfiguration configuration, [NotNull] ICompositionService compositionService, [NotNull] ITraceService traceService, [ImportMany, NotNull, ItemNotNull] IEnumerable<IEmitter> emitters)
        {
            Configuration = configuration;
            CompositionService = compositionService;
            Trace = traceService;
            Emitters = emitters;
        }

        [NotNull]
        protected ICompositionService CompositionService { get; }

        [NotNull]
        protected IConfiguration Configuration { get; }

        [NotNull, ItemNotNull]
        protected IEnumerable<IEmitter> Emitters { get; }

        [NotNull]
        protected ITraceService Trace { get; }

        public virtual void Emit(IProject project)
        {
            var context = CompositionService.Resolve<IEmitContext>().With(this, project);

            if (Configuration.GetBool(Constants.Configuration.InstallPackage.ShowDiagnostics))
            {
                var treatWarningsAsErrors = Configuration.GetBool(Constants.Configuration.CheckProject.TreatWarningsAsErrors);
                context.Trace.TraceDiagnostics(context.Project.Diagnostics, treatWarningsAsErrors);
            }

            project.Lock(Locking.ReadOnly);

            var emitters = Emitters.OrderBy(e => e.Sortorder).ToList();
            var retries = new List<Tuple<IProjectItem, Exception>>();

            Emit(context, project, emitters, retries);
            EmitRetry(context, emitters, retries);

            project.Lock(Locking.ReadWrite);
        }

        protected virtual void Emit([NotNull] IEmitContext context, [NotNull] IProject project, [NotNull, ItemNotNull] List<IEmitter> emitters, [NotNull, ItemNotNull] ICollection<Tuple<IProjectItem, Exception>> retries)
        {
            EmitProjectItems(context, project.ProjectItems, emitters, retries);
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
                    lock (_syncObject)
                    {
                        retries.Add(new Tuple<IProjectItem, Exception>(projectItem, ex));
                    }
                }
                catch (EmitException ex)
                {
                    Trace.TraceError(ex.Msg, ex.Text, ex.FileName, ex.Span, ex.Details);
                }
                catch (Exception ex)
                {
                    lock (_syncObject)
                    {
                        retries.Add(new Tuple<IProjectItem, Exception>(projectItem, ex));
                    }
                }
            }
        }

        protected virtual void EmitProjectItems([NotNull] IEmitContext context, [NotNull, ItemNotNull] IEnumerable<IProjectItem> projectItems, [NotNull, ItemNotNull] List<IEmitter> emitters, [NotNull, ItemNotNull] ICollection<Tuple<IProjectItem, Exception>> retries)
        {
            // process templates before anything else
            var templates = projectItems.Where(p => p is Template).ToArray();
            var nonTemplates = projectItems.Where(p => !(p is Template)).ToArray();

            // always emit templates synchronized - Sitecore Template Engine does not perform well 
            foreach (var projectItem in templates)
            {
                EmitProjectItem(context, projectItem, emitters, retries);
            }

            var isMultiThreaded = context.Configuration.GetBool(Constants.Configuration.System.MultiThreaded);
            if (isMultiThreaded)
            {
                Parallel.ForEach(nonTemplates, projectItem => EmitProjectItem(context, projectItem, emitters, retries));
            }
            else
            {
                foreach (var projectItem in nonTemplates)
                {
                    EmitProjectItem(context, projectItem, emitters, retries);
                }
            }
        }

        protected virtual void EmitRetry([NotNull] IEmitContext context, [NotNull, ItemNotNull] List<IEmitter> emitters, [NotNull, ItemNotNull] ICollection<Tuple<IProjectItem, Exception>> retries)
        {
            while (true)
            {
                var retryAgain = new List<Tuple<IProjectItem, Exception>>();

                var projectItems = retries.Reverse().Select(retry => retry.Item1).ToArray();

                EmitProjectItems(context, projectItems, emitters, retryAgain);

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
                    Trace.TraceError(buildException.Msg, buildException.Text, buildException.FileName, buildException.Span, buildException.Details);
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
