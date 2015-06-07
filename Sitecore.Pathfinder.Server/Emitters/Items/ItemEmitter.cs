namespace Sitecore.Pathfinder.Emitters.Items
{
  using System;
  using System.ComponentModel.Composition;
  using System.Linq;
  using Sitecore.Configuration;
  using Sitecore.Data;
  using Sitecore.Data.Managers;
  using Sitecore.Pathfinder.Builders.Items;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Extensions;
  using Sitecore.Pathfinder.Projects;
  using Sitecore.Pathfinder.Projects.Items;

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

      var templateIdOrPath = this.ResolveTemplateIdOrPath(item);
      if (string.IsNullOrEmpty(templateIdOrPath))
      {
        throw new RetryableEmitException(Texts.Template_missing, item.Snapshot, item.TemplateIdOrPath);
      }

      var database = Factory.GetDatabase(item.DatabaseName);
      var templateItem = database.GetItem(templateIdOrPath);
      if (string.IsNullOrEmpty(templateIdOrPath))
      {
        throw new RetryableEmitException(Texts.Template_missing, item.Snapshot, item.TemplateIdOrPath);
      }

      var template = TemplateManager.GetTemplate(templateItem.ID, templateItem.Database);
      if (template == null)
      {
        throw new RetryableEmitException(Texts.Template_missing, item.Snapshot, item.TemplateIdOrPath);
      }

      this.ValidateFields(database, template, item);

      var itemBuilder = new ItemBuilder
      {
        Snapshot = item.Snapshot,
        DatabaseName = item.DatabaseName,
        Guid = projectItem.Guid,
        ItemName = item.ItemName,
        ItemIdOrPath = item.ItemIdOrPath,
        TemplateIdOrPath = templateIdOrPath
      };

      foreach (var field in item.Fields)
      {
        var value = field.Value;
        var templateField = template.GetField(field.FieldName);
        if (templateField == null)
        {
          throw new RetryableEmitException("Template field missing", item.Snapshot, field.FieldName);
        }

        foreach (var fieldResolver in context.FieldResolvers)
        {
          if (fieldResolver.CanResolve(context, templateField, field))
          {
            value = fieldResolver.Resolve(context, templateField, field);
          }
        }

        var fieldBuilder = new FieldBuilder(field.FieldName, field.Language, field.Version, value);
        itemBuilder.Fields.Add(fieldBuilder);
      }

      itemBuilder.Build(context);
    }

    [CanBeNull]
    protected string ResolveTemplateIdOrPath([NotNull] Item item)
    {
      if (ID.IsID(item.TemplateIdOrPath) || item.TemplateIdOrPath.StartsWith("/sitecore", StringComparison.OrdinalIgnoreCase))
      {
        return item.TemplateIdOrPath;
      }

      var database = Factory.GetDatabase(item.DatabaseName);
      var templates = TemplateManager.GetTemplates(database).Values.ToList();

      // try matching by name only
      var template = templates.FirstOrDefault(t => string.Compare(t.Name, item.TemplateIdOrPath, StringComparison.OrdinalIgnoreCase) == 0);
      if (template != null)
      {
        return database.GetItem(template.ID)?.Paths.Path;
      }

      // try matching by Xml safe name
      template = templates.FirstOrDefault(t => string.Compare(t.Name, item.TemplateIdOrPath.GetSafeXmlIdentifier(), StringComparison.OrdinalIgnoreCase) == 0);
      if (template != null)
      {
        return database.GetItem(template.ID)?.Paths.Path;
      }

      return null;
    }

    protected void ValidateFields([NotNull] Database database, [NotNull] Sitecore.Data.Templates.Template template, [NotNull] Item projectItem)
    {
      var templateFields = template.GetFields(true);

      foreach (var field in projectItem.Fields)
      {
        if (templateFields.All(f => string.Compare(f.Name, field.FieldName, StringComparison.OrdinalIgnoreCase) != 0))
        {
          throw new RetryableEmitException(Texts.Field_is_not_defined_in_the_template, field.NameProperty.TextNode, field.FieldName);
        }

        if (!string.IsNullOrEmpty(field.Language))
        {
          var language = LanguageManager.GetLanguage(field.Language, database);
          if (language == null)
          {
            throw new RetryableEmitException(Texts.Language_not_found, field.ValueProperty.TextNode, field.Language);
          }
        }
      }
    }
  }
}
