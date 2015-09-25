// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.ComponentModel.Composition;
using System.Linq;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Managers;
using Sitecore.Data.Templates;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Emitters.Writers.Items;
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
            if (!item.IsEmittable || item.IsExternalReference)
            {
                return;
            }

            var templateIdOrPath = ResolveTemplateIdOrPath(item);
            if (string.IsNullOrEmpty(templateIdOrPath))
            {
                throw new RetryableEmitException(Texts.Template_missing, TraceHelper.GetTextNode(item.TemplateIdOrPathProperty, item.ItemNameProperty), item.TemplateIdOrPath);
            }

            var database = Factory.GetDatabase(item.DatabaseName);
            var templateItem = database.GetItem(templateIdOrPath);
            if (string.IsNullOrEmpty(templateIdOrPath))
            {
                throw new RetryableEmitException(Texts.Template_missing, TraceHelper.GetTextNode(item.TemplateIdOrPathProperty), item.TemplateIdOrPath);
            }

            var template = TemplateManager.GetTemplate(templateItem.ID, templateItem.Database);
            if (template == null)
            {
                throw new RetryableEmitException(Texts.Template_missing, TraceHelper.GetTextNode(item.TemplateIdOrPathProperty), item.TemplateIdOrPath);
            }

            ValidateFields(database, template, item);

            var itemWriter = new ItemWriter
            {
                Snapshot = item.Snapshots.First(),
                DatabaseName = item.DatabaseName,
                Guid = projectItem.Uri.Guid,
                ItemName = item.ItemName,
                ItemIdOrPath = item.ItemIdOrPath,
                TemplateIdOrPath = templateIdOrPath
            };

            foreach (var field in item.Fields)
            {
                var templateField = template.GetField(field.FieldName);
                if (templateField == null)
                {
                    throw new RetryableEmitException(Texts.Template_field_missing, TraceHelper.GetTextNode(field.FieldNameProperty, item.ItemNameProperty), field.FieldName);
                }

                var fieldWriter = new FieldWriter(field.FieldNameProperty, field.Language, field.Version, field.CompiledValue);
                itemWriter.Fields.Add(fieldWriter);
            }

            itemWriter.Write(context);
        }

        [Diagnostics.CanBeNull]
        protected string ResolveTemplateIdOrPath([Diagnostics.NotNull] Item item)
        {
            var templateIdOrPath = item.TemplateIdOrPath;
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
                if (templateFields.All(f => string.Compare(f.Name, field.FieldName, StringComparison.OrdinalIgnoreCase) != 0))
                {
                    throw new RetryableEmitException(Texts.Field_is_not_defined_in_the_template, TraceHelper.GetTextNode(field.FieldNameProperty), field.FieldName);
                }

                if (!string.IsNullOrEmpty(field.Language))
                {
                    var language = LanguageManager.GetLanguage(field.Language, database);
                    if (language == null)
                    {
                        throw new RetryableEmitException(Texts.Language_not_found, TraceHelper.GetTextNode(field.ValueProperty), field.Language);
                    }
                }
            }
        }
    }
}
