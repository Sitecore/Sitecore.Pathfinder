// � 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;

namespace Sitecore.Pathfinder.Building.Tasks
{
    public class WriteJsonProjectDiagnostics : BuildTaskBase
    {
        public WriteJsonProjectDiagnostics() : base("write-json-project-diagnostics")
        {
            CanRunWithoutConfig = true;
        }

        public override void Run(IBuildContext context)
        {
            context.Trace.TraceInformation(Msg.C1041, Texts.Checking___);

            WriteJsonDiagnostics(context);
        }

        public override void WriteHelp(HelpWriter helpWriter)
        {
            helpWriter.Summary.Write("Checks the project for warnings and errors.");
            helpWriter.Remarks.Write("SETTINGS:" + Environment.NewLine);
            helpWriter.Remarks.Write("  write-json-project-diagnostics:output-path - /op = Insert path for outputting the json file). " + Environment.NewLine);
            helpWriter.Remarks.Write("  write-json-project-diagnostics:disabled-categories - Disables checker categories (Items, Fields, Templates, TemplateFields, Media)." + Environment.NewLine);
            helpWriter.Remarks.Write("  write-json-project-diagnostics:disabled-checkers - Disables specific checkers." + Environment.NewLine);
        }

        protected virtual void WriteJsonDiagnostics([NotNull] IBuildContext context)
        {
            JsonSerializer serializer = new JsonSerializer();

            SolutionDiagnostic solutionDiagnostic = new SolutionDiagnostic
            {
                SolutionName = "HARDCODED - PROJECT NAME",
                SolutionLocation = "Hardcoded - C:\\Projects\\MY PROJECT",
                ProjectDiagnostics = new List<ProjectDiagnostic>()
            };

            ProjectDiagnostic projectDiagnostic = new ProjectDiagnostic
            {
                ProjectId = context.Project.ProjectUniqueId,
                ProjectDirectory = context.ProjectDirectory,
                ProjectName = context.Project.Name,
                Diagnostics = new List<Diagnostic>()
            };

            foreach (var diagnostic in context.Project.Diagnostics)
            {
                projectDiagnostic.Diagnostics.Add(new Diagnostic()
                {
                    Severity = diagnostic.Severity,
                    Code = diagnostic.Msg,
                    FileName = diagnostic.FileName,
                    TextSpan = diagnostic.Span.ToString(),
                    Text = diagnostic.Text
                });
            }

            solutionDiagnostic.ProjectDiagnostics.Add(projectDiagnostic);

            var outputPath = GetOutputPath(context);

            using (StreamWriter sw = new StreamWriter(string.Concat(outputPath, context.Project.Name, ".json")))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, solutionDiagnostic);
            }
        }

        [NotNull]
        protected virtual string GetOutputPath([NotNull]IBuildContext context)
        {
            string path = context.Configuration.GetString("op");
            return path ?? string.Empty;
        }
    }

    internal class SolutionDiagnostic
    {
        [NotNull]
        public string SolutionName { get; set; }
        [NotNull]
        public string SolutionLocation { get; set; }
        [NotNull]
        [ItemNotNull]
        public IList<ProjectDiagnostic> ProjectDiagnostics { get; set; }
    }

    internal class ProjectDiagnostic
    {
        [NotNull]
        public string ProjectId { get; set; }
        [NotNull]
        public string ProjectName { get; set; }
        [NotNull]
        public string ProjectDirectory { get; set; }
        [NotNull]
        [ItemNotNull]
        public IList<Diagnostic> Diagnostics { get; set; }
    }

    internal class Diagnostic
    {
        public Severity Severity { get; set; }
        public int Code { get; set; }
        [NotNull]
        public string FileName { get; set; }
        [NotNull]
        public string TextSpan { get; set; }
        [NotNull]
        public string Text { get; set; }
    }

}
