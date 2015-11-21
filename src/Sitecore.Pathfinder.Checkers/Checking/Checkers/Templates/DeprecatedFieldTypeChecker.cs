// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Checking.Checkers.Templates
{
    public class DeprecatedFieldTypeChecker : CheckerBase
    {
        public override void Check(ICheckerContext context)
        {
            foreach (var template in context.Project.Templates)
            {
                foreach (var field in template.Fields)
                {
                    var type = field.Type;
                    string newType = null;

                    switch (type.ToLowerInvariant())
                    {
                        case "text":
                            newType = "Single-Line Text";
                            break;
                        case "html":
                            newType = "Rich Text";
                            break;
                        case "link":
                            newType = "General Link";
                            break;
                        case "lookup":
                            newType = "Droplink";
                            break;
                        case "memo":
                            newType = "Multi-Line Text";
                            break;
                        case "reference":
                            newType = "Droptree";
                            break;
                        case "server file":
                            newType = "Single-Line Text";
                            break;
                        case "tree":
                            newType = "Droptree";
                            break;
                        case "treelist":
                            newType = "TreelistEx";
                            break;
                        case "valuelookup":
                            newType = "Droplist";
                            break;
                    }

                    if (!string.IsNullOrEmpty(newType))
                    {
                        context.Trace.TraceWarning("Deprecated template field type", TraceHelper.GetTextNode(field.TypeProperty, field), $"The template field type \"{type}\" is deprecated. Use the \"{newType}\" field type.");
                    }
                }
            }
        }
    }
}
