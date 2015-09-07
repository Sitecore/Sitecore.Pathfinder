// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.ComponentModel.Composition;
using System.Linq;
using Sitecore.Pathfinder.Building.Querying;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Querying;

namespace Sitecore.Pathfinder.Building.Refactoring
{
    [Export(typeof(ITask))]
    public class Rename : QueryTaskBase
    {
        public Rename() : base("rename")
        {
        }

        public override void Run(IBuildContext context)
        {
            context.DisplayDoneMessage = false;

            var qualifiedName = context.Configuration.GetCommandLineArg(1).Trim();
            if (string.IsNullOrEmpty(qualifiedName))
            {
                qualifiedName = context.Configuration.GetString("name").Trim();
            }

            if (string.IsNullOrEmpty(qualifiedName))
            {
                context.Trace.Writeline(Texts.You_must_specific_the___name_argument);
                return;
            }

            var newShortName = context.Configuration.GetCommandLineArg(2).Trim();
            if (string.IsNullOrEmpty(newShortName))
            {
                newShortName = context.Configuration.GetString("to").Trim();
            }

            if (string.IsNullOrEmpty(newShortName))
            {
                context.Trace.Writeline(Texts.You_must_specific_the___to_argument);
                return;
            }

            var projectItem = context.Project.FindQualifiedItem(qualifiedName);
            if (projectItem == null)
            {
                context.Trace.Writeline(Texts.Item_not_found, qualifiedName);
                return;
            }

            var value = projectItem.QualifiedName;
            var n = value.LastIndexOf('/');
            value = n < 0 ? newShortName : value.Left(n + 1) + newShortName;

            var queryService = context.CompositionService.Resolve<IQueryService>();
            var references = queryService.FindUsages(context.Project, qualifiedName).ToList();

            foreach (var reference in references)
            {
                if (reference.SourceProperty != null)
                {
                    reference.SourceProperty.SetValue(value);
                }
            }

            var changedFileNames = context.Project.Items.SelectMany(i => i.Snapshots).Where(s => s.IsModified).Select(s => s.SourceFile.FileName).ToList();

            projectItem.Rename(newShortName);
            context.Project.SaveChanges();

            foreach (var fileName in changedFileNames)
            {
                context.Trace.TraceInformation(PathHelper.UnmapPath(context.SolutionDirectory, fileName));
            }

            context.Trace.TraceInformation($"Changed files: {changedFileNames.Count}");
        }

        public override void WriteHelp(HelpWriter helpWriter)
        {
            helpWriter.Summary.Write("Finds all project items that references the specified project item.");
            helpWriter.Remarks.Write("The project item must be fully qualified.");
            helpWriter.Parameters.WriteLine("name - The fully qualified project item to rename.");
            helpWriter.Parameters.WriteLine("to - The new name of the project item.");
            helpWriter.Examples.Write("scc rename /sitecore/content/Home Welcome");
        }
    }
}
