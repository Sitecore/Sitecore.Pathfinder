// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using NuGet;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Emitters
{
    [Export]
    public class Emitter
    {
        [ImportingConstructor]
        public Emitter([Diagnostics.NotNull] ICompositionService compositionService, [Diagnostics.NotNull] IConfigurationService configurationService, [Diagnostics.NotNull] ITraceService traceService, [Diagnostics.NotNull] IProjectService projectService)
        {
            CompositionService = compositionService;
            ConfigurationService = configurationService;
            Trace = traceService;
            ProjectService = projectService;
        }

        [Diagnostics.NotNull]
        protected ICompositionService CompositionService { get; }

        [Diagnostics.NotNull]
        protected IConfigurationService ConfigurationService { get; set; }

        [Diagnostics.NotNull]
        [ItemNotNull]
        [ImportMany]
        protected IEnumerable<IEmitter> Emitters { get; private set; }

        [Diagnostics.NotNull]
        protected IProjectService ProjectService { get; }

        [Diagnostics.NotNull]
        protected ITraceService Trace { get; }

        public virtual void Start()
        {
            // todo: support installation without configuration files
            ConfigurationService.Load(LoadConfigurationOptions.None);

            var project = ProjectService.LoadProjectFromConfiguration();

            Emit(project);
        }

        protected virtual void BuildNupkgFile([Diagnostics.NotNull] IEmitContext context, [Diagnostics.NotNull] string nuspecFileName)
        {
            var nupkgFileName = Path.ChangeExtension(nuspecFileName, "nupkg");

            try
            {
                context.FileSystem.DeleteFile(nupkgFileName);

                var packageBuilder = new PackageBuilder(nuspecFileName, Path.GetDirectoryName(nupkgFileName), NullPropertyProvider.Instance, false);

                using (var nupkg = new FileStream(nupkgFileName, FileMode.Create))
                {
                    packageBuilder.Save(nupkg);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
            }
        }

        protected virtual void Emit([Diagnostics.NotNull] IProject project)
        {
            var context = CompositionService.Resolve<IEmitContext>().With(project);

            TraceProjectDiagnostics(context);

            var emitters = Emitters.OrderBy(e => e.Sortorder).ToList();
            var retries = new List<Tuple<IProjectItem, Exception>>();

            Emit(context, project, emitters, retries);
            EmitRetry(context, emitters, retries);
        }

        protected virtual void Emit([Diagnostics.NotNull] IEmitContext context, [Diagnostics.NotNull] IProject project, [Diagnostics.NotNull][ItemNotNull] List<IEmitter> emitters, [Diagnostics.NotNull][ItemNotNull] ICollection<Tuple<IProjectItem, Exception>> retries)
        {
            foreach (var projectItem in project.Items)
            {
                EmitProjectItem(context, projectItem, emitters, retries);
            }
        }

        protected virtual void EmitProjectItem([Diagnostics.NotNull] IEmitContext context, [Diagnostics.NotNull] IProjectItem projectItem, [Diagnostics.NotNull][ItemNotNull] List<IEmitter> emitters, [Diagnostics.NotNull][ItemNotNull] ICollection<Tuple<IProjectItem, Exception>> retries)
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
                    Trace.TraceError(ex.Text, ex.FileName, ex.Span, ex.Details);
                }
                catch (Exception ex)
                {
                    retries.Add(new Tuple<IProjectItem, Exception>(projectItem, ex));
                }
            }
        }

        protected virtual void EmitRetry([Diagnostics.NotNull] IEmitContext context, [Diagnostics.NotNull][ItemNotNull] List<IEmitter> emitters, [Diagnostics.NotNull][ItemNotNull] ICollection<Tuple<IProjectItem, Exception>> retries)
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
                    Trace.TraceError(buildException.Text, buildException.FileName, buildException.Span, buildException.Details);
                }
                else if (exception != null)
                {
                    Trace.TraceError(exception.Message, projectItem.Snapshots.First().SourceFile.AbsoluteFileName, TextSpan.Empty);
                }
                else
                {
                    Trace.TraceError(Texts.An_error_occured, projectItem.Snapshots.First().SourceFile.AbsoluteFileName, TextSpan.Empty);
                }
            }
        }

        protected virtual void TraceProjectDiagnostics([Diagnostics.NotNull] IEmitContext context)
        {
            foreach (var diagnostic in context.Project.Diagnostics)
            {
                switch (diagnostic.Severity)
                {
                    case Severity.Error:
                        context.Trace.TraceError(diagnostic.Text, diagnostic.FileName, diagnostic.Span);
                        break;
                    case Severity.Warning:
                        context.Trace.TraceWarning(diagnostic.Text, diagnostic.FileName, diagnostic.Span);
                        break;
                    default:
                        context.Trace.TraceInformation(diagnostic.Text, diagnostic.FileName, diagnostic.Span);
                        break;
                }
            }
        }
    }
}
