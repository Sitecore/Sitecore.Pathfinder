// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Emitting;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Serializing;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Emitters
{
    [Export]
    public class Emitter
    {
        [ImportingConstructor]
        public Emitter([NotNull] ICompositionService compositionService, [NotNull] ITraceService traceService, [NotNull] IProjectService projectService, [ImportMany] [NotNull] [ItemNotNull] IEnumerable<IEmitter> emitters)
        {
            CompositionService = compositionService;
            Trace = traceService;
            ProjectService = projectService;
            Emitters = emitters;
        }

        [NotNull]
        protected ICompositionService CompositionService { get; }

        [NotNull]
        [ItemNotNull]
        protected IEnumerable<IEmitter> Emitters { get; }

        [NotNull]
        protected IProjectService ProjectService { get; }

        [NotNull]
        protected ITraceService Trace { get; }

        public virtual int Start()
        {
            SerializingDataProviderService.Disabled = true;
            try
            {
                // todo: support installation without configuration files
                var project = ProjectService.LoadProjectFromConfiguration();

                Emit(project);
            }
            finally
            {
                SerializingDataProviderService.Disabled = false;
            }

            return 0;
        }

        protected virtual void Emit([NotNull] IProject project)
        {
            var context = CompositionService.Resolve<IEmitContext>().With(project);

            TraceProjectDiagnostics(context);

            var emitters = Emitters.OrderBy(e => e.Sortorder).ToList();
            var retries = new List<Tuple<IProjectItem, Exception>>();

            Emit(context, project, emitters, retries);
            EmitRetry(context, emitters, retries);
        }

        protected virtual void Emit([NotNull] IEmitContext context, [NotNull] IProject project, [NotNull] [ItemNotNull] List<IEmitter> emitters, [NotNull] [ItemNotNull] ICollection<Tuple<IProjectItem, Exception>> retries)
        {
            foreach (var projectItem in project.ProjectItems)
            {
                EmitProjectItem(context, projectItem, emitters, retries);
            }
        }

        protected virtual void EmitProjectItem([NotNull] IEmitContext context, [NotNull] IProjectItem projectItem, [NotNull] [ItemNotNull] List<IEmitter> emitters, [NotNull] [ItemNotNull] ICollection<Tuple<IProjectItem, Exception>> retries)
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

        protected virtual void EmitRetry([NotNull] IEmitContext context, [NotNull] [ItemNotNull] List<IEmitter> emitters, [NotNull] [ItemNotNull] ICollection<Tuple<IProjectItem, Exception>> retries)
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

        protected virtual void TraceProjectDiagnostics([NotNull] IEmitContext context)
        {
            foreach (var diagnostic in context.Project.Diagnostics)
            {
                switch (diagnostic.Severity)
                {
                    case Severity.Error:
                        context.Trace.TraceError(diagnostic.Msg, diagnostic.Text, diagnostic.FileName, diagnostic.Span);
                        break;
                    case Severity.Warning:
                        context.Trace.TraceWarning(diagnostic.Msg, diagnostic.Text, diagnostic.FileName, diagnostic.Span);
                        break;
                    default:
                        context.Trace.TraceInformation(diagnostic.Msg, diagnostic.Text, diagnostic.FileName, diagnostic.Span);
                        break;
                }
            }
        }
    }
}
