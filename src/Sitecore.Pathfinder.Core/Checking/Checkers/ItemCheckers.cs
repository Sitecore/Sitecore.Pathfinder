// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Projects.Templates;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Checking.Checkers
{
    [Export(typeof(IChecker)), Shared]
    public class ItemCheckers : Checker
    {
        [Check]
        public IEnumerable<Diagnostic> AvoidManyChildren(ICheckerContext context)
        {
            return from item in context.Project.Items
                let count = item.GetChildren().Count()
                where count > 100
                select Warning(Msg.C1009, "Avoid items with many children", TraceHelper.GetTextNode(item), $"The item has {count} children. Items with more than 100 children decrease performance. Change the structure of the tree to reduce the number of children");
        }

        [Check]
        public IEnumerable<Diagnostic> AvoidManyVersions(ICheckerContext context)
        {
            return from item in context.Project.Items
                from language in item.GetLanguages()
                let count = item.GetVersions(language).Count()
                where count >= 10
                select Warning(Msg.C1010, "Avoid items with many version", TraceHelper.GetTextNode(item), $"The item has {count} versions in the {language} language. Items with more than 10 version decrease performance. Remove some of the older versions.");
        }

        [Check]
        public IEnumerable<Diagnostic> AvoidSpacesInItemNames(ICheckerContext context)
        {
            return from item in context.Project.Items
                where item.ItemName.IndexOf(' ') >= 0 && item.ItemName != "__Standard Values"
                select Warning(Msg.C1003, "Avoid spaces in item names. Use a display name instead", TraceHelper.GetTextNode(item.ItemNameProperty), item.ItemName);
        }

        [Check]
        public IEnumerable<Diagnostic> FieldIsNotDefinedInTemplate(ICheckerContext context)
        {
            return from item in context.Project.Items
                where item.Template != Template.Empty
                let templateFields = item.Template.GetAllFields().ToList()
                from field in item.Fields
                let templateField = templateFields.FirstOrDefault(f => string.Equals(f.FieldName, field.FieldName, StringComparison.OrdinalIgnoreCase))
                where templateField == null
                select Error(Msg.C1005, "Field is not defined in the template", TraceHelper.GetTextNode(field.FieldNameProperty, field, field.Item), "field: " + field.FieldName + ", template: " + item.TemplateName);
        }

        [Check]
        public IEnumerable<Diagnostic> ItemsWithSameDisplayName(ICheckerContext context)
        {
            var parents = new HashSet<Item>();

            foreach (var item in context.Project.Items)
            {
                var parent = item.GetParent();
                if (parent == null)
                {
                    continue;
                }

                if (parents.Contains(parent))
                {
                    continue;
                }

                parents.Add(parent);

                var children = parent.GetChildren().ToArray();
                for (var i0 = 0; i0 < children.Length - 2; i0++)
                {
                    var child0 = children[i0];

                    for (var i1 = i0 + 1; i1 < children.Length - 1; i1++)
                    {
                        var child1 = children[i1];

                        var languages = child0.GetLanguages().Intersect(child1.GetLanguages());

                        foreach (var language in languages)
                        {
                            var displayNames0 = child0.Fields.Where(f => string.Equals(f.FieldName, "__Display Name", StringComparison.OrdinalIgnoreCase) && f.Language == language).Select(f => f.Value);
                            var displayNames1 = child1.Fields.Where(f => string.Equals(f.FieldName, "__Display Name", StringComparison.OrdinalIgnoreCase) && f.Language == language).Select(f => f.Value);

                            var same = displayNames0.Intersect(displayNames1);

                            if (same.Any())
                            {
                                yield return Error(Msg.C1006, "Items with same display name on same level", TraceHelper.GetTextNode(child0, child1), $"Two or more items have the same display name \"{displayNames0.First()}\" on the same level. Change the display name of one or more of the items.");
                            }
                        }
                    }
                }
            }
        }

        [Check]
        public IEnumerable<Diagnostic> ItemsWithSameName(ICheckerContext context)
        {
            var parents = new HashSet<Item>();

            foreach (var item in context.Project.Items)
            {
                var parent = item.GetParent();
                if (parent == null)
                {
                    continue;
                }

                if (parents.Contains(parent))
                {
                    continue;
                }

                parents.Add(parent);

                var children = parent.GetChildren().ToArray();
                for (var i0 = 0; i0 < children.Length - 2; i0++)
                {
                    var child0 = children[i0];

                    for (var i1 = i0 + 1; i1 < children.Length - 1; i1++)
                    {
                        var child1 = children[i1];

                        if (string.Equals(child0.ItemName, child1.ItemName, StringComparison.OrdinalIgnoreCase))
                        {
                            yield return Error(Msg.C1007, "Items with same name on same level", TraceHelper.GetTextNode(child0.ItemNameProperty, child1.ItemNameProperty, child0, child1), $"Two or more items have the same name \"{child0.ItemName}\" on the same level. Change the name of one or more of the items.");
                        }
                    }
                }
            }
        }

        [Check]
        public IEnumerable<Diagnostic> ItemTemplateNotFound(ICheckerContext context)
        {
            return from item in context.Project.Items
                where item.Template == Template.Empty
                select Error(Msg.C1004, "Template not found", TraceHelper.GetTextNode(item.TemplateIdOrPathProperty, item, item.ItemNameProperty), item.TemplateIdOrPath);
        }

        [Check]
        public IEnumerable<Diagnostic> ReminderDateIsAfterArchiveDate(ICheckerContext context)
        {
            return from item in context.Project.Items
                let archiveDate = item[Constants.Fields.ArchiveDate].FromIsoToDateTime()
                let reminderDate = item[Constants.Fields.ReminderDate].FromIsoToDateTime()
                where reminderDate != DateTime.MinValue && archiveDate != DateTime.MinValue && reminderDate > archiveDate
                select Warning(Msg.C1002, "The Reminder date is after the Archive date", TraceHelper.GetTextNode(item.Fields[Constants.Fields.ArchiveDate], item.Fields[Constants.Fields.ReminderDate], item), "Change either the Reminder date or the Archive date.");
        }

        [Check]
        public IEnumerable<Diagnostic> UnpublishDateIsBeforePublishDate(ICheckerContext context)
        {
            return from item in context.Project.Items
                where !item.Publishing.NeverPublish
                let publishDate = item.Publishing.PublishDate
                let unpublishDate = item.Publishing.PublishDate
                where publishDate != DateTime.MinValue && unpublishDate != DateTime.MinValue && publishDate > unpublishDate
                select Warning(Msg.C1011, "The Publish date is after the Unpublish date", TraceHelper.GetTextNode(item.Fields[Constants.Fields.PublishDate], item.Fields[Constants.Fields.UnpublishDate], item), "Change either the Publish date or the Unpublish date");
        }

        [Check]
        public IEnumerable<Diagnostic> ValidToDateIsBeforeValidFromDate(ICheckerContext context)
        {
            return from item in context.Project.Items
                where !item.Publishing.NeverPublish
                let validFrom = item.Publishing.ValidFrom
                let validTo = item.Publishing.ValidTo
                where validFrom != DateTime.MinValue && validTo != DateTime.MinValue && validFrom > validTo
                select Warning(Msg.C1021, "The Valid From date is after the Valid To date", TraceHelper.GetTextNode(item.Fields[Constants.Fields.ValidFrom], item.Fields[Constants.Fields.ValidTo], item), "Change either the Valid From date or the Valid To date");
        }
    }
}
