namespace Sitecore.Pathfinder.Builders.FieldResolvers
{
  using System;
  using System.ComponentModel.Composition;
  using Sitecore.Data.Templates;
  using Sitecore.Pathfinder.Builders.FieldResolvers.Layouts;
  using Sitecore.Pathfinder.Documents;
  using Sitecore.Pathfinder.Documents.Xml;
  using Sitecore.Pathfinder.Emitters;
  using Sitecore.Pathfinder.Projects.Items;

  [Export(typeof(IFieldResolver))]
  public class LayoutFieldResolver : FieldResolverBase
  {
    public override bool CanResolve(IEmitContext context, TemplateField templateField, Field field)
    {
      return string.Compare(templateField.Type, "layout", StringComparison.OrdinalIgnoreCase) == 0;
    }

    public override string Resolve(IEmitContext context, TemplateField templateField, Field field)
    {
      var layoutResolveContext = new LayoutResolveContext(context, (ITextSnapshot)field.NameProperty.TextNode.Snapshot, field.Item.DatabaseName);

      var textNode = field.NameProperty.TextNode;
      var resolver = textNode is XmlTextNode ? (LayoutResolverBase)new XmlLayoutResolver() : new JsonLayoutResolver();

      return resolver.Resolve(layoutResolveContext, textNode);
    }
  }
}
