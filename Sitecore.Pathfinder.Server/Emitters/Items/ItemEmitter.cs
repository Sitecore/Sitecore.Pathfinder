// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.ComponentModel.Composition;
using System.Linq;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Managers;
using Sitecore.Data.Templates;
using Sitecore.Pathfinder.Builders.Items;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Emitters.Items
{
    [Export(typeof(IEmitter))]
    public class ItemEmitter : EmitterBase
    {
        public ItemEmitter() : base(Constants.Emitters.Items)
        {
        }

        public override bool CanEmit(IEmitContext context, IProjectItem projectItem)
        {
            return projectItem is Item;
        }

        public override void Emit(IEmitContext context, IProjectItem projectItem)
        {
            var item = (Item)projectItem;
            if (!item.IsEmittable)
            {
                return;
            }

            var templateIdOrPath = ResolveTemplateIdOrPath(item);
            if (string.IsNullOrEmpty(templateIdOrPath))
            {
                throw new RetryableEmitException(Texts.Template_missing, item.TemplateIdOrPath.Source ?? item.ItemName.Source ?? TextNode.Empty, item.TemplateIdOrPath.Value);
            }

            var database = Factory.GetDatabase(item.DatabaseName);
            var templateItem = database.GetItem(templateIdOrPath);
            if (string.IsNullOrEmpty(templateIdOrPath))
            {
                throw new RetryableEmitException(Texts.Template_missing, item.TemplateIdOrPath.Source ?? TextNode.Empty, item.TemplateIdOrPath.Value);
            }

            var template = TemplateManager.GetTemplate(templateItem.ID, templateItem.Database);
            if (template == null)
            {
                throw new RetryableEmitException(Texts.Template_missing, item.TemplateIdOrPath.Source ?? TextNode.Empty, item.TemplateIdOrPath.Value);
            }

            ValidateFields(database, template, item);

            var itemBuilder = new ItemBuilder
            {
                Snapshot = item.Snapshots.First(),
                DatabaseName = item.DatabaseName,
                Guid = projectItem.Guid,
                ItemName = item.ItemName.Value,
                ItemIdOrPath = item.ItemIdOrPath,
                TemplateIdOrPath = templateIdOrPath
            };

            foreach (var field in item.Fields)
            {
                var value = field.Value.Value;
                var templateField = template.GetField(field.FieldName.Value);
                if (templateField == null)
                {
                    throw new RetryableEmitException(Texts.Template_field_missing, field.FieldName.Source ?? item.ItemName.Source ?? TextNode.Empty, field.FieldName.Value);
                }

                foreach (var fieldResolver in context.FieldResolvers.OrderBy(r => r.Priority))
                {
                    if (fieldResolver.CanResolve(context, templateField, field))
                    {
                        value = fieldResolver.Resolve(context, templateField, field);
                    }
                }

                var fieldBuilder = new FieldBuilder(field.FieldName, field.Language.Value, field.Version.Value, value);
                itemBuilder.Fields.Add(fieldBuilder);
            }

            itemBuilder.Build(context);
        }

        [Diagnostics.CanBeNull]
        protected string ResolveTemplateIdOrPath([Diagnostics.NotNull] Item item)
        {
            var templateIdOrPath = item.TemplateIdOrPath.Value;
            if (ID.IsID(templateIdOrPath) || templateIdOrPath.StartsWith("/sitecore", StringComparison.OrdinalIgnoreCase))
            {
                return templateIdOrPath;
            }

            var database = Factory.GetDatabase(item.DatabaseName);
            var templates = TemplateManager.GetTemplates(database).Values.ToList();

            // try matching by name only
            var template = templates.FirstOrDefault(t => string.Compare(t.Name, templateIdOrPath, StringComparison.OrdinalIgnoreCase) == 0);
            if (template != null)
            {
                return database.GetItem(template.ID)?.Paths.Path;
            }

            // try matching by Xml safe name
            template = templates.FirstOrDefault(t => string.Compare(t.Name.GetSafeXmlIdentifier(), templateIdOrPath, StringComparison.OrdinalIgnoreCase) == 0);
            if (template != null)
            {
                return database.GetItem(template.ID)?.Paths.Path;
            }

            // try to see if the item already exists - may have been created elsewhere
            var i = database.GetItem(item.ItemIdOrPath);
            if (i != null)
            {
                return database.GetItem(i.Template.ID)?.Paths.Path;
            }

            return null;
        }

        protected void ValidateFields([Diagnostics.NotNull] Database database, [Diagnostics.NotNull] Template template, [Diagnostics.NotNull] Item projectItem)
        {
            var templateFields = template.GetFields(true);

            foreach (var field in projectItem.Fields)
            {
                if (templateFields.All(f => string.Compare(f.Name, field.FieldName.Value, StringComparison.OrdinalIgnoreCase) != 0))
                {
                    throw new RetryableEmitException(Texts.Field_is_not_defined_in_the_template, field.FieldName.Source ?? TextNode.Empty, field.FieldName.Value);
                }

                if (!string.IsNullOrEmpty(field.Language.Value))
                {
                    var language = LanguageManager.GetLanguage(field.Language.Value, database);
                    if (language == null)
                    {
                        throw new RetryableEmitException(Texts.Language_not_found, field.Value.Source ?? TextNode.Empty, field.Language.Value);
                    }
                }
            }
        }
    }
}
