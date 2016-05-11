// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Sitecore.Pathfinder.Checking;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Checkers
{
    public class SitecoreTemplateConventions : Checker
    {
        [Export("Check")]
        protected IEnumerable<Diagnostic> AvoidSettingSharedAndUnversionedInItems(ICheckerContext context)
        {
            return from item in context.Project.Items
                where item.TemplateName.Equals("Template field") && item["Shared"].Equals("True") && item["Unversioned"].Equals("True")
                select Warning(Msg.C1000, "In a template field, the 'Shared' field overrides the 'Unversioned' field. To fix, clear the 'Unversioned' field (the field remains shared)", TraceHelper.GetTextNode(item));
        }

        [Export("Check")]
        protected IEnumerable<Diagnostic> AvoidSettingSharedAndUnversionedInTemplates(ICheckerContext context)
        {
            return from template in context.Project.Templates
                from templateSection in template.Sections
                from templateField in templateSection.Fields
                where templateField.Shared && templateField.Unversioned
                select Warning(Msg.C1000, "In a template field, the 'Shared' field overrides the 'Unversioned' field. To fix, clear the 'Unversioned' field (the field remains shared)", TraceHelper.GetTextNode(templateField, template));
        }

        [Export("Check")]
        protected IEnumerable<Diagnostic> DefaultValueFieldIsObsolete(ICheckerContext context)
        {
            return from item in context.Project.Items
                where item.TemplateName.Equals("Template field")
                let defaultValueField = item.Fields["Default Value"]
                where defaultValueField != null && defaultValueField.Value != string.Empty
                select Warning(Msg.C1000, "In a template field, the 'Default value' field is no longer used. To fix, clear the 'Default value' field and set the value on the Standard Values item", TraceHelper.GetTextNode(defaultValueField, item));
        }

        [Export("Check")]
        protected IEnumerable<Diagnostic> TemplateIdOfStandardValuesShouldMatchParentId(ICheckerContext context)
        {
            return from item in context.Project.Items
                let parent = item.GetParent()
                where parent != null && item.ItemName.Equals("__Standard Values") && item.Template.Uri.Guid != parent.Uri.Guid
                select Warning(Msg.C1000, "The Template ID of a Standard Values item should be match the ID of the parent item. To fix, moved the Standard Values item under the correct template", TraceHelper.GetTextNode(item));
        }

        [Export("Check")]
        protected IEnumerable<Diagnostic> TemplateMustLocatedInTemplatesSection(ICheckerContext context)
        {
            return from template in context.Project.Templates
                where !template.ItemIdOrPath.StartsWith("/sitecore/templates/")
                select Warning(Msg.C1000, "All templates should be located in the '/sitecore/templates' section. To fix, move the template into the '/sitecore/templates' section", TraceHelper.GetTextNode(template));
        }

        [Export("Check")]
        protected IEnumerable<Diagnostic> TemplateNodeOrFolderShouldBeTemplateFolder(ICheckerContext context)
        {
            return from item in context.Project.Items
                where item.ItemIdOrPath.StartsWith("/sitecore/templates/") && (item.TemplateName.Equals("Folder") || item.TemplateName.Equals("Node"))
                select Warning(Msg.C1000, "In the '/sitecore/templates' section, folder items use the 'Template folder' template - not the 'Folder' or 'Node' template. To fix, change the template of the item to 'Template Folder", TraceHelper.GetTextNode(item), item.TemplateName);
        }

        [Export("Check")]
        protected IEnumerable<Diagnostic> TemplateSectionShouldOnlyContainTemplates(ICheckerContext context)
        {
            return from item in context.Project.Items
                where item.ItemIdOrPath.StartsWith("/sitecore/templates/") && item.ItemIdOrPath.IndexOf("Branches") < 0 && !item.ItemName.Equals("__Standard Values") && !item.TemplateName.Equals("Template") && !item.TemplateName.Equals("Template section") && !item.TemplateName.Equals("Template field") && !item.TemplateName.Equals("Template Folder") && !item.TemplateName.Equals("Node") && !item.TemplateName.Equals("Folder")
                select Warning(Msg.C1000, "The '/sitecore/templates' section should only contain item with template 'Template', 'Template section', 'Template field', 'Template folder' or standard values items. To fix, move the item outside the '/sitecore/templates' section", TraceHelper.GetTextNode(item), item.TemplateName);
        }

        [Export("Check")]
        protected IEnumerable<Diagnostic> TemplatesMustLocatedInTemplatesSection(ICheckerContext context)
        {
            return from item in context.Project.Items
                where !item.ItemIdOrPath.StartsWith("/sitecore/templates/") && (item.TemplateName.Equals("Template") || item.TemplateName.Equals("Template section") || item.TemplateName.Equals("Template Field") || item.TemplateName.Equals("Template Folder"))
                select Warning(Msg.C1000, "All items with template 'Template', 'Template section', 'Template field' and 'Template folder' should be located in the '/sitecore/templates' section. To fix, move the template into the '/sitecore/templates' section", TraceHelper.GetTextNode(item), item.TemplateName);
        }
    }
}
