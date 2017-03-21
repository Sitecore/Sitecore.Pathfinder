// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Composition;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Emitting;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks
{
    [Export(typeof(ITask)), Shared]
    public class PublishProject : BuildTaskBase
    {
        [ImportingConstructor]
        public PublishProject([ItemNotNull, NotNull, ImportMany] IEnumerable<IProjectEmitter> projectEmitters) : base("publish-project")
        {
            ProjectEmitters = projectEmitters;
            Alias = "publish";
            Shortcut = "p";
        }

        [NotNull, Option("format", Alias = "f", IsRequired = true, PromptText = "Select output format", HelpText = "Output format", PositionalArg = 1, HasOptions = true, DefaultValue = "default")]
        public string Format { get; set; } = "directory";

        [ItemNotNull, NotNull]
        protected IEnumerable<IProjectEmitter> ProjectEmitters { get; }

        public override void Run(IBuildContext context)
        {
            context.Trace.TraceInformation(Msg.D1029, "Publishing project...");

            var format = Format;
            if (format == "default")
            {
                format = context.Configuration.GetString(Constants.Configuration.Output.Format, "directory");
            }

            var projectEmitters = ProjectEmitters.Where(p => p.CanEmit(format)).ToArray();
            if (!projectEmitters.Any())
            {
                context.Trace.TraceError(Msg.E1043, "No project emitters found");
                return;
            }

            var project = context.LoadProject();

            foreach (var projectEmitter in projectEmitters)
            {
                projectEmitter.Emit(project);
            }
        }

        [NotNull, OptionValues("Format")]
        protected IEnumerable<(string Name, string Value)> GetFormatOptions([NotNull] ITaskContext context)
        {
            yield return ("Directory", "directory");
            yield return ("Package", "package");
        }
    }
}
