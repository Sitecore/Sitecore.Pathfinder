// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Composition;
using System.Linq;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Emitting;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks.Commands
{
    [Export(typeof(ITask)), Shared]
    public class PublishProject : BuildTaskBase
    {
        [ImportingConstructor]
        public PublishProject([NotNull] IFactory factory, [ItemNotNull, NotNull, ImportMany] IEnumerable<IProjectEmitter> projectEmitters) : base("publish-project", "publish")
        {
            Factory = factory;
            ProjectEmitters = projectEmitters;

            Shortcut = "p";
        }

        [NotNull, Option("format", Alias = "f", IsRequired = true, PromptText = "Select output format", HelpText = "Output format", PositionalArg = 1, HasOptions = true, DefaultValue = "package")]
        public string Format { get; set; } = "package";

        [NotNull]
        protected IFactory Factory { get; }

        [ItemNotNull, NotNull]
        protected IEnumerable<IProjectEmitter> ProjectEmitters { get; }

        public override void Run(IBuildContext context)
        {
            var project = context.LoadProject();

            context.Trace.TraceInformation(Msg.D1029, "Publishing project...");

            if (project.Diagnostics.Any(d => d.Severity == Severity.Error))
            {
                context.Trace.TraceError(Msg.E1048, "Project contains errors and will not be published");
                return;
            }

            var format = Format;
            if (string.IsNullOrEmpty(format))
            {
                format = context.Configuration.GetString(Constants.Configuration.Output.Format, "package");
            }

            var projectEmitters = ProjectEmitters.Where(p => p.CanEmit(format)).ToArray();
            if (!projectEmitters.Any())
            {
                context.Trace.TraceError(Msg.E1043, "No project emitters found");
                return;
            }

            foreach (var projectEmitter in projectEmitters)
            {
                var emitContext = Factory.EmitContext(projectEmitter, project);

                projectEmitter.Emit(emitContext, project);

                context.OutputFiles.AddRange(emitContext.OutputFiles);
            }
        }

        [NotNull, OptionValues("Format")]
        protected IEnumerable<(string Name, string Value)> GetFormatOptions([NotNull] ITaskContext context)
        {
            // remember to update ExtractItems.cs as well
            yield return ("Package", "package");
            yield return ("Nuget", "nuget");
            yield return ("Unicorn", "unicorn");
            yield return ("Update", "update");
            yield return ("Yaml", "yaml");
            yield return ("Json", "json");
            yield return ("Xml", "xml");
            yield return ("Serialization", "serialization");
        }
    }
}
