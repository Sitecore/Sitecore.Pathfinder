// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Checking.Conventions
{
    public class SitecoreTemplateConventions : ConventionsBase
    {
        public SitecoreTemplateConventions()
        {
            ConventionCount = 9;
        }

        protected override IEnumerable<IEnumerable<Diagnostic>> Check()
        {
            yield return    
                from item in Items
                where 
                    !item.ItemIdOrPath.StartsWithIgnoreCase("/sitecore/templates/") &&
                    (
                        item.TemplateName.EqualsIgnoreCase("Template") ||
                        item.TemplateName.EqualsIgnoreCase("Template Section") ||
                        item.TemplateName.EqualsIgnoreCase("Template Field") ||
                        item.TemplateName.EqualsIgnoreCase("Template Folder")
                    )
                select Warning("All items with template 'Template', 'Template section', 'Template field' and 'Template folder' should be located in the '/sitecore/templates' section. To fix, move the template into the '/sitecore/templates' section", item);

            yield return 
                from template in Templates
                where 
                    !template.ItemIdOrPath.StartsWithIgnoreCase("/sitecore/templates/")
                select Warning("All templates should be located in the '/sitecore/templates' section. To fix, move the template into the '/sitecore/templates' section", template);

            yield return
                from item in Items
                where
                    item.ItemName.EqualsIgnoreCase("__Standard Values") &&
                    item.Template.Uri.Guid != item.GetParent()?.Uri.Guid
                select Warning("The Template ID of a Standard Values item should be match the ID of the parent item. To fix, moved the Standard Values item under the correct template", item);

            yield return
                from item in Items
                where
                    item.ItemIdOrPath.StartsWith("/sitecore/templates/") &&
                    (
                        item.TemplateName.EqualsIgnoreCase("Folder") ||
                        item.TemplateName.EqualsIgnoreCase("Node") 
                    )
                select Warning("In the '/sitecore/templates' section, folder items use the 'Template folder' template - not the 'Folder' or 'Node' template. To fix, change the template of the item to 'Template Folder", item);

            yield return               
                from item in Items
                where
                    item.ItemIdOrPath.StartsWithIgnoreCase("/sitecore/templates/") &&
                    !item.ItemName.EqualsIgnoreCase("__Standard Values") &&
                    !item.TemplateName.EqualsIgnoreCase("Template")  &&
                    !item.TemplateName.EqualsIgnoreCase("Template section") &&
                    !item.TemplateName.EqualsIgnoreCase("Template field") &&
                    !item.TemplateName.EqualsIgnoreCase("Template folder")
                select Warning("The '/sitecore/templates' section should only contain item with template 'Template', 'Template section', 'Template field', 'Template folder' or standard values items. To fix, move the item outside the '/sitecore/templates' section", item);

            yield return
                from item in Items
                where
                    item.TemplateName.EqualsIgnoreCase("Template field") &&
                    item["Shared"].EqualsIgnoreCase("True") &&
                    item["Unversioned"].EqualsIgnoreCase("True")
                select Warning("In a template field, the 'Shared' field overrides the 'Unversioned' field. To fix, clear the 'Unversioned' field (the field remains shared)", item);

            yield return
                from template in Templates
                from templateSection in template.Sections
                from templateField in templateSection.Fields
                where
                    templateField.Shared && 
                    templateField.Unversioned
                select Warning("In a template field, the 'Shared' field overrides the 'Unversioned' field. To fix, clear the 'Unversioned' field (the field remains shared)", templateField, template);

            yield return
                from item in Items
                where
                    item.TemplateName.EqualsIgnoreCase("Template field")
                let defaultValueField = item.Fields["Default Value"]
                where
                    defaultValueField != null &&
                    defaultValueField.Value != string.Empty
                select Warning("In a template field, the 'Default value' field is no longer used. To fix, clear the 'Default value' field and set the value on the Standard Values item", defaultValueField, item);

            yield return
                from item in Items
                where
                    item.TemplateName.EqualsIgnoreCase("Template field")
                let validation = item.Fields["Validation"]
                let validationText = item.Fields["ValidationText"]
                where
                    validation != null && validation.Value != "" &&
                    validationText != null && validationText.Value == ""
                select Warning("In a template field, the 'Default value' field is no longer used. To fix, clear the 'Default value' field and set the value on the Standard Values item", validation, item);
       }
    }
}