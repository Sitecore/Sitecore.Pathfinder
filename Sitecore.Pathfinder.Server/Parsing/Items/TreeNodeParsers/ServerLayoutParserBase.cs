namespace Sitecore.Pathfinder.Parsing.Items.TreeNodeParsers
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel.Composition;
  using System.IO;
  using System.Linq;
  using System.Xml;
  using Sitecore.Configuration;
  using Sitecore.Data;
  using Sitecore.Data.Items;
  using Sitecore.Data.Managers;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Documents;
  using Sitecore.Pathfinder.Extensions.StringExtensions;
  using Sitecore.SecurityModel;
  using Sitecore.Text;

  public abstract class ServerLayoutParserBase : LayoutParserBase
  {
    private const string RenderingIdsFastQuery = "@@templateid='{99F8905D-4A87-4EB8-9F8B-A9BEBFB3ADD6}' or @@templateid='{2A3E91A0-7987-44B5-AB34-35C2D9DE83B9}' or @@templateid='{86776923-ECA5-4310-8DC0-AE65FE88D078}' or @@templateid='{39587D7D-F06D-4CB4-A25E-AA7D847EDDD0}' or @@templateid='{0A98E368-CDB9-4E1E-927C-8E0C24A003FB}' or @@templateid='{83E993C5-C0FC-4472-86A9-2F6CFED694E4}' or @@templateid='{1DDE3F02-0BD7-4779-867A-DC578ADF91EA}' or @@templateid='{F1F1D639-4F54-40C2-8BE0-81266B392CEB}'";

    private static readonly List<string> IgnoreAttributes = new List<string>
    {
      "Cacheable", 
      "DataSource", 
      "Placeholder", 
      "RenderingName", 
      "VaryByData", 
      "VaryByDevice", 
      "VaryByLogin", 
      "VaryByParameters", 
      "VaryByQueryString", 
      "VaryByUser", 
    };

    [NotNull]
    protected virtual Item[] FindRenderingItems([NotNull] IEnumerable<Item> renderingItems, [NotNull] string renderingItemId)
    {
      var n = renderingItemId.LastIndexOf('.');
      if (n < 0)
      {
        return new Item[0];
      }

      renderingItemId = renderingItemId.Mid(n + 1);

      return renderingItems.Where(r => r.Name == renderingItemId).ToArray();
    }

    [NotNull]
    protected virtual string GetPlaceholders([NotNull] ITextNode renderingTextNode, [NotNull] Item renderingItem)
    {
      var id = renderingTextNode.GetAttributeValue("Id");
      var result = ",";

      var placeHolders = renderingItem["Place Holders"];
      foreach (var s in placeHolders.Split(','))
      {
        if (string.IsNullOrEmpty(s))
        {
          continue;
        }

        var placeholderName = s.Replace("$Id", id).Trim();

        result += placeholderName + ",";
      }

      return result;
    }

    protected override string GetValue(ItemParseContext context, ITextNode textNode)
    {
      var layoutTextNode = textNode.ChildNodes.FirstOrDefault();
      if (layoutTextNode == null)
      {
        return string.Empty;
      }

      var database = Factory.GetDatabase(context.ParseContext.DatabaseName);

      var writer = new StringWriter();
      var output = new XmlTextWriter(writer)
      {
        Formatting = Formatting.Indented
      };

      using (new SecurityDisabler())
      {
        this.WriteLayout(context, output, database, layoutTextNode);
      }

      return writer.ToString();
    }

    protected virtual bool IsContentProperty([NotNull] ITextNode renderingTextNode, [NotNull] ITextNode childTextNode)
    {
      return childTextNode.Name.StartsWith(renderingTextNode.Name + ".");
    }

    [NotNull]
    protected virtual Item[] ResolveRenderingItem([NotNull] IEnumerable<Item> renderingItems, [NotNull] string renderingItemId)
    {
      var path = "/" + renderingItemId.Replace(".", "/");

      return renderingItems.Where(r => r.Paths.Path.EndsWith(path, StringComparison.InvariantCultureIgnoreCase)).ToArray();
    }

    [NotNull]
    protected virtual Item[] ResolveRenderingItemId([NotNull] Database database, [NotNull] IEnumerable<Item> renderingItems, [NotNull] string renderingItemId)
    {
      var matches = renderingItems.Where(r => r.Name == renderingItemId).ToArray();

      if (matches.Length == 0)
      {
        matches = this.FindRenderingItems(renderingItems, renderingItemId);
      }

      if (matches.Length > 1)
      {
        matches = this.ResolveRenderingItem(matches, renderingItemId);
      }

      return matches;
    }

    protected virtual void WriteBool([NotNull] ItemParseContext context, [NotNull] XmlTextWriter output, [NotNull] ITextNode renderingTextNode, [NotNull] string id, [NotNull] string attributeName, [NotNull] string name, bool ignoreValue = false)
    {
      var value = renderingTextNode.GetAttributeValue(attributeName);
      if (string.IsNullOrEmpty(value))
      {
        return;
      }

      if (value != "True" && value != "False")
      {
        context.ParseContext.Trace.TraceError(id + Texts.__Boolean_parameter_must_have_value__True__or__False_, renderingTextNode, attributeName);
        value = "False";
      }

      var b = value == "True";
      if (b == ignoreValue)
      {
        return;
      }

      output.WriteAttributeString(name, b ? "1" : "0");
    }

    protected virtual void WriteDataSource([NotNull] XmlTextWriter output, [NotNull] ITextNode renderingTextNode, [NotNull] Database database)
    {
      var dataSource = renderingTextNode.GetAttributeValue("DataSource");
      if (string.IsNullOrEmpty(dataSource))
      {
        return;
      }

      var item = database.GetItem(dataSource);
      output.WriteAttributeString("ds", item?.ID.ToString() ?? dataSource);
    }

    protected virtual void WriteDevice([NotNull] ItemParseContext context, [NotNull] XmlTextWriter output, [NotNull] IEnumerable<Item> renderingItems, [NotNull] Database database, [NotNull] ITextNode deviceTextNode)
    {
      output.WriteStartElement("d");

      var deviceName = deviceTextNode.GetAttributeValue("Name");
      if (string.IsNullOrEmpty(deviceName))
      {
        context.ParseContext.Trace.TraceError(Texts.Device_element_is_missing__Name__attribute_, deviceTextNode);
      }
      else
      {
        var devices = database.GetItem(ItemIDs.DevicesRoot);
        if (devices == null)
        {
          context.ParseContext.Trace.TraceError(Texts.Devices_not_found_in_database_, deviceTextNode, context.ParseContext.DatabaseName);
        }
        else
        {
          var device = devices.Children[deviceName];
          if (device == null)
          {
            // todo: put into resources
            context.ParseContext.Trace.TraceError($"Device \"{deviceName}\" not found.", deviceTextNode);
          }
          else
          {
            output.WriteAttributeString("id", device.ID.ToString());
          }
        }
      }

      var layoutPlaceholders = string.Empty;
      var layoutPath = deviceTextNode.GetAttributeValue("Layout");
      if (!string.IsNullOrEmpty(layoutPath))
      {
        var l = database.GetItem(layoutPath);
        if (l == null)
        {
          throw new RetryableEmitException(Texts.Layout_not_found_, context.ParseContext.Snapshot.SourceFile, layoutPath);
        }

        output.WriteAttributeString("l", l.ID.ToString());
        layoutPlaceholders = this.GetPlaceholders(deviceTextNode, l);
      }

      foreach (var renderingTextNode in deviceTextNode.ChildNodes)
      {
        this.WriteRendering(context, output, renderingItems, database, renderingTextNode, layoutPlaceholders);
      }

      output.WriteEndElement();
    }

    protected virtual void WriteLayout([NotNull] ItemParseContext context, [NotNull] XmlTextWriter output, [NotNull] Database database, [NotNull] ITextNode layoutTextNode)
    {
      // todo: cache this in the build context
      // todo: use better search
      var renderingItems = database.SelectItems("fast://*[" + RenderingIdsFastQuery + "]").ToList();

      output.WriteStartElement("r");

      foreach (var deviceTextNode in layoutTextNode.ChildNodes)
      {
        this.WriteDevice(context, output, renderingItems, database, deviceTextNode);
      }

      output.WriteEndElement();
    }

    protected virtual void WritePlaceholder([NotNull] ItemParseContext context, [NotNull] XmlTextWriter output, [NotNull] ITextNode renderingTextNode, [NotNull] string id, [NotNull] string placeholders)
    {
      var placeholder = renderingTextNode.GetAttributeValue("Placeholder");

      if (string.IsNullOrEmpty(placeholder) && !string.IsNullOrEmpty(placeholders))
      {
        var n = placeholders.IndexOf(",", 1, StringComparison.InvariantCultureIgnoreCase);
        if (n >= 0)
        {
          placeholder = placeholders.Mid(1, n - 1);
        }
      }

      if (string.IsNullOrEmpty(placeholder))
      {
        return;
      }

      if (!string.IsNullOrEmpty(placeholders))
      {
        if (placeholders.IndexOf("," + placeholder + ",", StringComparison.InvariantCultureIgnoreCase) < 0)
        {
          context.ParseContext.Trace.TraceWarning(string.Format(Texts._2___Placeholder___0___is_not_defined_in_the_parent_rendering__Parent_rendering_has_these_placeholders___1__, placeholder, placeholders.Mid(1, placeholders.Length - 2), id), renderingTextNode, "Placeholder");
        }
      }

      output.WriteAttributeString("ph", placeholder);
    }

    protected virtual void WriteRendering([NotNull] ItemParseContext context, [NotNull] XmlTextWriter output, [NotNull] IEnumerable<Item> renderingItems, [NotNull] Database database, [NotNull] ITextNode renderingTextNode, [NotNull] string placeholders)
    {
      string renderingItemId;

      if (renderingTextNode.Name == "r")
      {
        renderingItemId = renderingTextNode.GetAttributeValue("id");
      }
      else if (renderingTextNode.Name == "Rendering")
      {
        renderingItemId = renderingTextNode.GetAttributeValue("RenderingName");
      }
      else
      {
        renderingItemId = renderingTextNode.Name;
      }

      var id = renderingTextNode.GetAttributeValue("Id");
      if (string.IsNullOrEmpty(id))
      {
        id = renderingItemId;
      }

      if (string.IsNullOrEmpty(renderingItemId))
      {
        context.ParseContext.Trace.TraceError($"Unknown element \"{id}\".", renderingTextNode);
        return;
      }

      Item renderingItem;
      if (ID.IsID(renderingItemId))
      {
        renderingItem = database.GetItem(renderingItemId);
      }
      else
      {
        var matches = this.ResolveRenderingItemId(database, renderingItems, renderingItemId);

        if (matches.Length == 0)
        {
          context.ParseContext.Trace.TraceError($"Rendering \"{renderingItemId}\" not found.", renderingTextNode);
          return;
        }

        if (matches.Length > 1)
        {
          context.ParseContext.Trace.TraceError($"Ambiguous rendering match. {matches.Length} renderings match \"{renderingItemId}\".", renderingTextNode);
          return;
        }

        renderingItem = matches[0];
      }

      if (renderingItem == null)
      {
        context.ParseContext.Trace.TraceError($"Rendering \"{renderingItemId}\" not found.", renderingTextNode);
        return;
      }

      output.WriteStartElement("r");

      this.WriteBool(context, output, renderingTextNode, id, "Cacheable", "cac");
      output.WriteAttributeString("id", renderingItem.ID.ToString());
      this.WriteDataSource(output, renderingTextNode, database);
      this.WriteParameters(context, output, renderingTextNode, renderingItem, id);
      this.WritePlaceholder(context, output, renderingTextNode, id, placeholders);

      // WriteAttributeStringNotEmpty(@"uid", this.UniqueId);
      this.WriteBool(context, output, renderingTextNode, id, "VaryByData", "vbd");
      this.WriteBool(context, output, renderingTextNode, id, "VaryByDevice", "vbdev");
      this.WriteBool(context, output, renderingTextNode, id, "VaryByLogin", "vbl");
      this.WriteBool(context, output, renderingTextNode, id, "VaryByParameters", "vbp");
      this.WriteBool(context, output, renderingTextNode, id, "VaryByQueryString", "vbqs");
      this.WriteBool(context, output, renderingTextNode, id, "VaryByUser", "vbu");

      output.WriteEndElement();

      if (renderingTextNode.ChildNodes.Any(child => !this.IsContentProperty(renderingTextNode, child)))
      {
        var placeHolders = renderingItem["Place Holders"];

        if (string.IsNullOrEmpty(placeHolders))
        {
          context.ParseContext.Trace.TraceError($"The \"{renderingTextNode.Name}\" element cannot have any child elements as it does not define any placeholders in its 'Place Holders' field.", renderingTextNode);
        }
        else if (placeHolders.IndexOf("$Id", StringComparison.InvariantCulture) >= 0 && string.IsNullOrEmpty(renderingTextNode.GetAttributeValue("Id")))
        {
          context.ParseContext.Trace.TraceError($"The \"{renderingTextNode.Name}\" element must have an ID as it has child elements.", renderingTextNode);
        }
      }

      foreach (var child in renderingTextNode.ChildNodes)
      {
        if (this.IsContentProperty(renderingTextNode, child))
        {
          continue;
        }

        this.WriteRendering(context, output, renderingItems, database, child, this.GetPlaceholders(renderingTextNode, renderingItem));
      }
    }

    private void WriteParameters([NotNull] ItemParseContext context, [NotNull] XmlTextWriter output, [NotNull] ITextNode renderingTextNode, [NotNull] Item renderingItem, [NotNull] string id)
    {
      var fields = new Dictionary<string, string>();

      var parametersTemplateItemId = renderingItem["Parameters Template"];
      var parametersTemplateItem = renderingItem.Database.GetItem(parametersTemplateItemId);
      if (parametersTemplateItem != null)
      {
        var template = TemplateManager.GetTemplate(parametersTemplateItem.ID, renderingItem.Database);
        foreach (var field in template.GetFields(true))
        {
          if (field.Template.BaseIDs.Length != 0)
          {
            fields[field.Name.ToLowerInvariant()] = field.Type;
          }
        }
      }

      var properties = new Dictionary<string, string>();

      foreach (var attribute in renderingTextNode.Attributes)
      {
        properties[attribute.Name] = attribute.Value;
      }

      foreach (var child in renderingTextNode.ChildNodes)
      {
        if (this.IsContentProperty(renderingTextNode, child))
        {
          var name = child.Name.Mid(renderingTextNode.Name.Length + 1);
          var value = string.Join(string.Empty, child.ChildNodes.Select(n => n.ToString()).ToArray());

          properties[name] = value;
        }
      }

      var par = new UrlString();
      foreach (var pair in properties)
      {
        var attributeName = pair.Key;
        if (IgnoreAttributes.Contains(attributeName))
        {
          continue;
        }

        var value = pair.Value;

        string type;
        if (fields.TryGetValue(attributeName.ToLowerInvariant(), out type))
        {
          switch (type.ToLowerInvariant())
          {
            case "checkbox":
              if (!value.StartsWith("{Binding") && !value.StartsWith("{@"))
              {
                if (value != "True" && value != "False")
                {
                  context.ParseContext.Trace.TraceError($"{id}: Boolean parameter must have value \"True\", \"False\", \"{{Binding ... }}\" or \"{{@ ... }}\".", renderingTextNode, attributeName);
                }

                value = MainUtil.GetBool(value, false) ? "1" : "0";
              }

              break;
          }
        }
        else
        {
          context.ParseContext.Trace.TraceWarning(string.Format(Texts._1___Parameter___0___is_not_defined_in_the_parameters_template_, attributeName, id), renderingTextNode, attributeName);
        }

        if (value.StartsWith("/sitecore", StringComparison.InvariantCultureIgnoreCase))
        {
          var item = renderingItem.Database.GetItem(value);
          if (item != null)
          {
            value = item.ID.ToString();
          }
        }

        par[attributeName] = value;
      }

      output.WriteAttributeString("par", par.ToString());
    }
  }
}
