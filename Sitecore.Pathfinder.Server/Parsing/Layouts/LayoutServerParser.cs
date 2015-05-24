namespace Sitecore.Pathfinder.Parsing.Layouts
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel.Composition;
  using System.IO;
  using System.Linq;
  using System.Xml;
  using System.Xml.Linq;
  using Sitecore.Configuration;
  using Sitecore.Data;
  using Sitecore.Data.Items;
  using Sitecore.Data.Managers;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Documents;
  using Sitecore.Pathfinder.Emitters;
  using Sitecore.Pathfinder.Extensions.StringExtensions;
  using Sitecore.Pathfinder.Extensions.XElementExtensions;
  using Sitecore.Pathfinder.Projects.Items;
  using Sitecore.Pathfinder.Projects.Layouts;
  using Sitecore.Text;

  [Export(typeof(IParser))]
  public class LayoutServerParser : ParserBase
  {
    private const string FileExtension = ".layout.xml";

    private const string RenderingIdsFastQuery = "@@templateid='{99F8905D-4A87-4EB8-9F8B-A9BEBFB3ADD6}' or " + "@@templateid='{2A3E91A0-7987-44B5-AB34-35C2D9DE83B9}' or " + "@@templateid='{86776923-ECA5-4310-8DC0-AE65FE88D078}' or " + "@@templateid='{39587D7D-F06D-4CB4-A25E-AA7D847EDDD0}' or " + "@@templateid='{0A98E368-CDB9-4E1E-927C-8E0C24A003FB}' or " + "@@templateid='{83E993C5-C0FC-4472-86A9-2F6CFED694E4}' or " + "@@templateid='{1DDE3F02-0BD7-4779-867A-DC578ADF91EA}' or " + "@@templateid='{F1F1D639-4F54-40C2-8BE0-81266B392CEB}'";

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

    public LayoutServerParser() : base(Constants.Parsers.Renderings)
    {
    }

    public override bool CanParse(IParseContext context)
    {
      return context.DocumentSnapshot.SourceFile.FileName.EndsWith(FileExtension, StringComparison.OrdinalIgnoreCase);
    }

    public override void Parse(IParseContext context)
    {
      var layout = this.ParseLayout(context);

      var item = new Projects.Items.Item(context.Project, context.ItemPath, context.DocumentSnapshot)
      {
        ItemName = context.ItemName, 
        DatabaseName = context.DatabaseName, 
        ItemIdOrPath = context.ItemPath,
        OverwriteWhenMerging = true
      };

      item.Fields.Add(new Field(item.TextNode, "__Renderings", layout));

      context.Project.AddOrMerge(item);
    }

    [Sitecore.NotNull]
    public Sitecore.Data.Items.Item[] ResolveRenderingItemId([Sitecore.NotNull] Database database, [Sitecore.NotNull] IEnumerable<Sitecore.Data.Items.Item> renderingItems, [Sitecore.NotNull] string renderingItemId)
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

    [NotNull]
    protected string ParseLayout([NotNull] IParseContext context)
    {
      var database = Factory.GetDatabase(context.DatabaseName);

      var errors = new List<Message>();
      var warnings = new List<Message>();

      var root = context.DocumentSnapshot.SourceFile.ReadAsXml();
      if (root == null)
      {
        throw new EmitException(Texts.Layout_file_is_not_valid, context.DocumentSnapshot.SourceFile);
      }

      var writer = new StringWriter();
      var output = new XmlTextWriter(writer)
      {
        Formatting = Formatting.Indented
      };

      this.WriteLayout(context, output, database, root, errors, warnings);

      if (!errors.Any())
      {
        return writer.ToString();
      }

      foreach (var error in errors)
      {
        context.Trace.TraceError("", context.DocumentSnapshot.SourceFile.FileName, new TextPosition(error.Line, error.Column, 0), error.Text);
      }

      foreach (var warning in warnings)
      {
        context.Trace.TraceWarning("", context.DocumentSnapshot.SourceFile.FileName, new TextPosition(warning.Line, warning.Column, 0), warning.Text);
      }

      if (errors.Any())
      {
        throw new EmitException(Texts.Layout_contains_errors, context.DocumentSnapshot.SourceFile);
      }

      return string.Empty;
    }

    [Sitecore.NotNull]
    protected Sitecore.Data.Items.Item GetDestinationItem([Sitecore.NotNull] IEmitContext context, [Sitecore.NotNull] Layout layout, [Sitecore.NotNull] Sitecore.Data.Items.Item item)
    {
      if (item.TemplateID != TemplateIDs.Template)
      {
        return item;
      }

      // item is a template - apply layout to the standard values
      var standardValuesId = item[FieldIDs.StandardValues];

      if (string.IsNullOrEmpty(standardValuesId))
      {
        var template = item.Template;
        if (template != null)
        {
          standardValuesId = template.InnerItem[FieldIDs.StandardValues];
        }
      }

      if (!string.IsNullOrEmpty(standardValuesId))
      {
        item = item.Database.GetItem(standardValuesId);
      }

      if (item == null)
      {
        throw new EmitException(Texts.Cannot_apply_a_layout_to_a_template__The_template_needs_a_Standard_Values_, layout.DocumentSnapshot);
      }

      return item;
    }

    protected void SaveLayoutField([Sitecore.NotNull] Sitecore.Data.Items.Item item, [Sitecore.NotNull] string layout)
    {
      using (new EditContext(item))
      {
        item[FieldIDs.LayoutField] = layout;
      }
    }

    [Sitecore.NotNull]
    private Sitecore.Data.Items.Item[] FindRenderingItems([Sitecore.NotNull] IEnumerable<Sitecore.Data.Items.Item> renderingItems, [Sitecore.NotNull] string renderingItemId)
    {
      var n = renderingItemId.LastIndexOf('.');
      if (n < 0)
      {
        return new Sitecore.Data.Items.Item[0];
      }

      renderingItemId = renderingItemId.Mid(n + 1);

      return renderingItems.Where(r => r.Name == renderingItemId).ToArray();
    }

    [Sitecore.NotNull]
    private string GetPlaceholders([Sitecore.NotNull] XElement renderingElement, [Sitecore.NotNull] Sitecore.Data.Items.Item renderingItem)
    {
      var id = renderingElement.GetAttributeValue("Id");
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

    private bool IsContentProperty([Sitecore.NotNull] XElement renderingElement, [Sitecore.NotNull] XElement child)
    {
      return child.Name.LocalName.StartsWith(renderingElement.Name.LocalName + ".");
    }

    [Sitecore.NotNull]
    private Sitecore.Data.Items.Item[] ResolveRenderingItem([Sitecore.NotNull] IEnumerable<Sitecore.Data.Items.Item> renderingItems, [Sitecore.NotNull] string renderingItemId)
    {
      var path = "/" + renderingItemId.Replace(".", "/");

      return renderingItems.Where(r => r.Paths.Path.EndsWith(path, StringComparison.InvariantCultureIgnoreCase)).ToArray();
    }

    private void WriteBool([Sitecore.NotNull] XmlTextWriter output, [Sitecore.NotNull] XElement renderingElement, [Sitecore.NotNull] string id, [Sitecore.NotNull] string attributeName, [Sitecore.NotNull] string name, [Sitecore.NotNull] List<Message> errors, bool ignoreValue = false)
    {
      var value = renderingElement.GetAttributeValue(attributeName);
      if (string.IsNullOrEmpty(value))
      {
        return;
      }

      if (value != "True" && value != "False")
      {
        errors.Add(new Message(id + ": Boolean parameter must have value \"True\" or \"False\".", renderingElement, attributeName));
        value = "False";
      }

      var b = value == "True";
      if (b == ignoreValue)
      {
        return;
      }

      output.WriteAttributeString(name, b ? "1" : "0");
    }

    private void WriteDataSource([Sitecore.NotNull] XmlTextWriter output, [Sitecore.NotNull] XElement renderingElement, [Sitecore.NotNull] Database database)
    {
      var dataSource = renderingElement.GetAttributeValue("DataSource");
      if (string.IsNullOrEmpty(dataSource))
      {
        return;
      }

      var item = database.GetItem(dataSource);
      output.WriteAttributeString("ds", item?.ID.ToString() ?? dataSource);
    }

    private void WriteDevice([Sitecore.NotNull] IParseContext context, [Sitecore.NotNull] XmlTextWriter output, [Sitecore.NotNull] IEnumerable<Sitecore.Data.Items.Item> renderingItems, [Sitecore.NotNull] Database database, [Sitecore.NotNull] XElement deviceElement, [Sitecore.NotNull] List<Message> errors, [Sitecore.NotNull] List<Message> warnings)
    {
      output.WriteStartElement("d");

      var deviceName = deviceElement.GetAttributeValue("Name");
      if (string.IsNullOrEmpty(deviceName))
      {
        errors.Add(new Message("Device element is missing \"Name\" attribute.", deviceElement));
      }
      else
      {
        var devices = database.GetItem(ItemIDs.DevicesRoot);
        if (devices == null)
        {
          errors.Add(new Message("Devices not found in database.", deviceElement));
        }
        else
        {
          var device = devices.Children[deviceName];
          if (device == null)
          {
            errors.Add(new Message($"Device \"{deviceName}\" not found.", deviceElement, "Name"));
          }
          else
          {
            output.WriteAttributeString("id", device.ID.ToString());
          }
        }
      }

      var layoutPlaceholders = string.Empty;
      var layoutPath = deviceElement.GetAttributeValue("Layout");
      if (!string.IsNullOrEmpty(layoutPath))
      {
        var l = database.GetItem(layoutPath);
        if (l == null)
        {
          throw new RetryableEmitException(Texts.Layout_not_found_, context.DocumentSnapshot.SourceFile, layoutPath);
        }

        output.WriteAttributeString("l", l.ID.ToString());
        layoutPlaceholders = this.GetPlaceholders(deviceElement, l);
      }

      foreach (var renderingElement in deviceElement.Elements())
      {
        this.WriteRendering(output, renderingItems, database, renderingElement, layoutPlaceholders, errors, warnings);
      }

      output.WriteEndElement();
    }

    private void WriteLayout([Sitecore.NotNull] IParseContext context, [Sitecore.NotNull] XmlTextWriter output, [Sitecore.NotNull] Database database, [Sitecore.NotNull] XElement layoutElement, [Sitecore.NotNull] List<Message> errors, [Sitecore.NotNull] List<Message> warnings)
    {
      // todo: cache this in the build context
      // todo: use better search
      var renderingItems = database.SelectItems("fast://*[" + RenderingIdsFastQuery + "]").ToList();

      output.WriteStartElement("r");

      // Do not make this Elements("Device") - XML namespaces will mess it up.
      foreach (var deviceElement in layoutElement.Elements())
      {
        this.WriteDevice(context, output, renderingItems, database, deviceElement, errors, warnings);
      }

      output.WriteEndElement();
    }

    private void WriteParameters([Sitecore.NotNull] XmlTextWriter output, [Sitecore.NotNull] XElement renderingElement, [Sitecore.NotNull] Sitecore.Data.Items.Item renderingItem, [Sitecore.NotNull] string id, [Sitecore.NotNull] List<Message> errors, [Sitecore.NotNull] List<Message> warnings)
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

      foreach (var attribute in renderingElement.Attributes())
      {
        properties[attribute.Name.LocalName] = attribute.Value;
      }

      foreach (var child in renderingElement.Elements())
      {
        if (this.IsContentProperty(renderingElement, child))
        {
          var name = child.Name.LocalName.Mid(renderingElement.Name.LocalName.Length + 1);
          var value = string.Join(string.Empty, child.Nodes().Select(n => n.ToString()).ToArray());

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
                  errors.Add(new Message($"{id}: Boolean parameter must have value \"True\", \"False\", \"{{Binding ... }}\" or \"{{@ ... }}\".", renderingElement, attributeName));
                }

                value = MainUtil.GetBool(value, false) ? "1" : "0";
              }

              break;
          }
        }
        else
        {
          warnings.Add(new Message(string.Format("{1}: Parameter \"{0}\" is not defined in the parameters template.", attributeName, id), renderingElement, attributeName));
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

    private void WritePlaceholder([Sitecore.NotNull] XmlTextWriter output, [Sitecore.NotNull] XElement renderingElement, [Sitecore.NotNull] string id, [Sitecore.NotNull] List<Message> warnings, [Sitecore.NotNull] string placeholders)
    {
      var placeholder = renderingElement.GetAttributeValue("Placeholder");

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
          warnings.Add(new Message(string.Format("{2}: Placeholder \"{0}\" is not defined in the parent rendering. Parent rendering has these placeholders: {1}.", placeholder, placeholders.Mid(1, placeholders.Length - 2), id), renderingElement, "Placeholder"));
        }
      }

      output.WriteAttributeString("ph", placeholder);
    }

    private void WriteRendering([Sitecore.NotNull] XmlTextWriter output, [Sitecore.NotNull] IEnumerable<Sitecore.Data.Items.Item> renderingItems, [Sitecore.NotNull] Database database, [Sitecore.NotNull] XElement renderingElement, [Sitecore.NotNull] string placeholders, [Sitecore.NotNull] List<Message> errors, [Sitecore.NotNull] List<Message> warnings)
    {
      string renderingItemId;

      if (renderingElement.Name.LocalName == "r")
      {
        renderingItemId = renderingElement.GetAttributeValue("id");
      }
      else if (renderingElement.Name.LocalName == "Rendering")
      {
        renderingItemId = renderingElement.GetAttributeValue("RenderingName");
      }
      else
      {
        renderingItemId = renderingElement.Name.LocalName;
      }

      var id = renderingElement.GetAttributeValue("Id");
      if (string.IsNullOrEmpty(id))
      {
        id = renderingItemId;
      }

      if (string.IsNullOrEmpty(renderingItemId))
      {
        errors.Add(new Message($"Unknown element \"{id}\".", renderingElement));
        return;
      }

      Sitecore.Data.Items.Item renderingItem;
      if (ID.IsID(renderingItemId))
      {
        renderingItem = database.GetItem(renderingItemId);
      }
      else
      {
        var matches = this.ResolveRenderingItemId(database, renderingItems, renderingItemId);

        if (matches.Length == 0)
        {
          errors.Add(new Message($"Rendering \"{renderingItemId}\" not found.", renderingElement));
          return;
        }

        if (matches.Length > 1)
        {
          errors.Add(new Message($"Ambiguous rendering match. {matches.Length} renderings match \"{renderingItemId}\".", renderingElement));
          return;
        }

        renderingItem = matches[0];
      }

      if (renderingItem == null)
      {
        errors.Add(new Message($"Rendering \"{renderingItemId}\" not found.", renderingElement));
        return;
      }

      output.WriteStartElement("r");

      this.WriteBool(output, renderingElement, id, "Cacheable", "cac", errors);
      output.WriteAttributeString("id", renderingItem.ID.ToString());
      this.WriteDataSource(output, renderingElement, database);
      this.WriteParameters(output, renderingElement, renderingItem, id, errors, warnings);
      this.WritePlaceholder(output, renderingElement, id, warnings, placeholders);

      // WriteAttributeStringNotEmpty(@"uid", this.UniqueId);
      this.WriteBool(output, renderingElement, id, "VaryByData", "vbd", errors);
      this.WriteBool(output, renderingElement, id, "VaryByDevice", "vbdev", errors);
      this.WriteBool(output, renderingElement, id, "VaryByLogin", "vbl", errors);
      this.WriteBool(output, renderingElement, id, "VaryByParameters", "vbp", errors);
      this.WriteBool(output, renderingElement, id, "VaryByQueryString", "vbqs", errors);
      this.WriteBool(output, renderingElement, id, "VaryByUser", "vbu", errors);

      output.WriteEndElement();

      if (renderingElement.Elements().Any(child => !this.IsContentProperty(renderingElement, child)))
      {
        var placeHolders = renderingItem["Place Holders"];

        if (string.IsNullOrEmpty(placeHolders))
        {
          errors.Add(new Message($"The \"{renderingElement.Name.LocalName}\" element cannot have any child elements as it does not define any placeholders in its 'Place Holders' field.", renderingElement));
        }
        else if (placeHolders.IndexOf("$Id", StringComparison.InvariantCulture) >= 0 && string.IsNullOrEmpty(renderingElement.GetAttributeValue("Id")))
        {
          errors.Add(new Message($"The \"{renderingElement.Name.LocalName}\" element must have an ID as it has child elements.", renderingElement));
        }
      }

      foreach (var child in renderingElement.Elements())
      {
        if (this.IsContentProperty(renderingElement, child))
        {
          continue;
        }

        this.WriteRendering(output, renderingItems, database, child, this.GetPlaceholders(renderingElement, renderingItem), errors, warnings);
      }
    }

    public class Message
    {
      public Message([Sitecore.NotNull] string text, int line, int column)
      {
        this.Text = text;
        this.Line = line;
        this.Column = column;
      }

      public Message([Sitecore.NotNull] string text, [Sitecore.NotNull] XElement element)
      {
        this.Text = text;

        var lineInfo = element as IXmlLineInfo;
        this.Line = lineInfo.LineNumber;
        this.Column = lineInfo.LinePosition;
      }

      public Message([Sitecore.NotNull] string text, [Sitecore.NotNull] XElement element, [Sitecore.NotNull] string attributeName)
      {
        this.Text = text;

        if (!element.HasAttributes)
        {
          return;
        }

        IXmlLineInfo lineInfo;

        var attribute = element.Attribute(attributeName);
        if (attribute != null)
        {
          lineInfo = attribute;
        }
        else
        {
          lineInfo = element;
        }

        this.Line = lineInfo.LineNumber;
        this.Column = lineInfo.LinePosition;
      }

      public int Column { get; }

      public int Line { get; }

      [Sitecore.NotNull]
      public string Text { get; }
    }
  }
}