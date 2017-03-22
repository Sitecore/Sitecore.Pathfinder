// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using Sitecore.Pathfinder.Configuration.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensibility;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Emitting
{
    public abstract class ProjectEmitterBase : IProjectEmitter
    {
        [NotNull]
        private readonly object _syncObject = new object();

        [ImportingConstructor]
        protected ProjectEmitterBase([NotNull] IConfiguration configuration, [NotNull] ICompositionService compositionService, [NotNull] ITraceService traceService, [ImportMany, NotNull, ItemNotNull] IEnumerable<IEmitter> emitters)
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

        public abstract bool CanEmit(string format);

        public virtual void Emit(IProject project)
        {
            var context = CompositionService.Resolve<IEmitContext>().With(this, project);

            if (Configuration.GetBool(Constants.Configuration.InstallPackage.ShowDiagnostics))
            {
                var treatWarningsAsErrors = Configuration.GetBool(Constants.Configuration.CheckProject.TreatWarningsAsErrors);
                context.Trace.TraceDiagnostics(project.Diagnostics, treatWarningsAsErrors);
            }

            project.Lock(Locking.ReadOnly);

            var emitters = Emitters.OrderBy(e => e.Sortorder).ToList();
            var retries = new List<Tuple<IProjectItem, Exception>>();

            Emit(context, project, emitters, retries);
            EmitRetry(context, emitters, retries);

            project.Lock(Locking.ReadWrite);
        }

        protected virtual void Emit([NotNull] IEmitContext context, [NotNull] IProjectBase project, [NotNull, ItemNotNull] List<IEmitter> emitters, [NotNull, ItemNotNull] ICollection<Tuple<IProjectItem, Exception>> retries)
        {
            EmitProjectItems(context, project.ProjectItems, emitters, retries);
        }

        protected virtual void EmitProjectItem([NotNull] IEmitContext context, [NotNull] IEmitter emitter, [NotNull] IProjectItem projectItem, [NotNull, ItemNotNull] ICollection<Tuple<IProjectItem, Exception>> retries)
        {
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

        protected virtual void EmitProjectItems([NotNull] IEmitContext context, [NotNull, ItemNotNull] IEnumerable<IProjectItem> projectItems, [NotNull, ItemNotNull] List<IEmitter> emitters, [NotNull, ItemNotNull] ICollection<Tuple<IProjectItem, Exception>> retries)
        {
            var unemitted = new List<IProjectItem>(projectItems);

            foreach (var emitter in emitters.OrderBy(e => e.Sortorder))
            {
                foreach (var projectItem in projectItems)
                {
                    if (!emitter.CanEmit(context, projectItem))
                    {
                        continue;
                    }

                    EmitProjectItem(context, emitter, projectItem, retries);
                    unemitted.Remove(projectItem);
                }
            }

            foreach (var projectItem in unemitted)
            {
                Trace.TraceWarning(Msg.E1039, "No emitter found", projectItem.Snapshot.SourceFile, projectItem.QualifiedName);
            }

            /*
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
            */
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
                    Trace.TraceError(Msg.E1002, exception.Message, projectItem.Snapshot.SourceFile.AbsoluteFileName, TextSpan.Empty);
                    Trace.WriteLine(exception.StackTrace);
                }
                else
                {
                    Trace.TraceError(Msg.E1003, Texts.An_error_occured, projectItem.Snapshot.SourceFile.AbsoluteFileName, TextSpan.Empty);
                }
            }
        }
    }
}
