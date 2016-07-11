// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Sitecore.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Checking.Checkers
{
    public class WorkflowCheckers : Checker
    {
        [Export("Check"), NotNull, ItemNotNull]
        public IEnumerable<Diagnostic> WorkflowHaNoInitialState([NotNull] ICheckerContext context)
        {
            var worflowTemplateGuid = TemplateIDs.Workflow.Guid;

            return from item in context.Project.Items
                where item.Template.Uri.Guid == worflowTemplateGuid && item.ItemName != "$name"
                let initialStateItem = context.Project.GetByQualifiedName<Item>(item["Initial state"])
                where initialStateItem == null
                select Error(Msg.G1000, $"The workflow \"{item.ItemName}\" has no initial state", TraceHelper.GetTextNode(item), "To fix, open the workflow and set an initial state.");
        }

        [Export("Check"), NotNull, ItemNotNull]
        public IEnumerable<Diagnostic> WorkflowHasFinalState([NotNull] ICheckerContext context)
        {
            var database = Factory.GetDatabase("master");

            var workflowProvider = database.WorkflowProvider;
            if (workflowProvider == null)
            {
                yield break;
            }

            var workflows = workflowProvider.GetWorkflows();
            if (workflows == null)
            {
                yield break;
            }

            foreach (var workflow in workflows)
            {
                var hasFinalState = false;

                foreach (var workflowState in workflow.GetStates())
                {
                    if (workflowState.FinalState)
                    {
                        hasFinalState = true;
                        break;
                    }
                }

                if (!hasFinalState)
                {
                    yield return Warning(Msg.G1000, $"The workflow \"{workflow.Appearance.DisplayName}\" has no final state", "One or more of the workflow state should be a final state. To fix, open the final state item and check the \"Final State\" checkbox.");
                }
            }
        }
    }
}
