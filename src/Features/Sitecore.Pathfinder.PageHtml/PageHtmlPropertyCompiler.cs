// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using Sitecore.Pathfinder.Compiling.Compilers;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Languages.Renderings;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Snapshots;
using Sitecore.Pathfinder.Text;

namespace Sitecore.Pathfinder.PageHtml
{
    public class PageHtmlPropertyCompiler : CompilerBase
    {
        public const string LayoutPage = "Layout.Page";

        public const string PlaceholderNamespaceName = "http://www.sitecore.net/pathfinder/placeholder";

        public const string RenderingNamespaceName = "http://www.sitecore.net/pathfinder/rendering";

        // must come after RenderingCompiler, or renderings will not be found
        [ImportingConstructor]
        public PageHtmlPropertyCompiler([NotNull] IFileSystemService fileSystem) : base(9000)
        {
            FileSystem = fileSystem;
        }

        [NotNull]
        protected IFileSystemService FileSystem { get; }

        public override bool CanCompile(ICompileContext context, IProjectItem projectItem)
        {
            var item = projectItem as Item;
            return item != null && item.ContainsProperty(LayoutPage);
        }

        public override void Compile(ICompileContext context, IProjectItem projectItem)
        {
            var item = projectItem as Item;
            Assert.Cast(item, nameof(item));

            var fileName = item.GetValue<string>(LayoutPage)?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(fileName))
            {
                return;
            }

            var property = item.GetProperty<string>(LayoutPage);

            // find page html file
            var rendering = item.Project.Files.FirstOrDefault(i => string.Equals(i.FilePath, fileName, StringComparison.OrdinalIgnoreCase));
            if (rendering == null)
            {
                context.Trace.TraceError(Msg.C1047, Texts.Rendering_reference_not_found, TraceHelper.GetTextNode(property), fileName);
            }

            // find MvcLayout
            var layoutItem = item.Project.ProjectItems.OfType<Item>().FirstOrDefault(i => i.ItemIdOrPath == "/sitecore/layout/Layouts/MvcLayout");
            if (layoutItem == null)
            {
                context.Trace.TraceError(Msg.C1048, Texts.Layout_reference_not_found, TraceHelper.GetTextNode(property), "/sitecore/layout/Layouts/MvcLayout");
            }

            if (rendering == null || layoutItem == null)
            {
                return;
            }

            var sourceFile = rendering.Snapshots.First().SourceFile;

            var writer = new StringWriter();
            var output = new XmlTextWriter(writer);
            output.Formatting = Formatting.Indented;

            var buildContext = new BuildContext(context, sourceFile, output, item, layoutItem);

            var literalRendering = FindRendering(buildContext, "Literal", string.Empty, TextSpan.Empty);
            if (literalRendering == null)
            {
                return;
            }

            buildContext.Placeholder = layoutItem["Place Holders"];
            buildContext.LiteralRendering = literalRendering;

            ParseHtml(buildContext);

            SetRenderingsField(buildContext, buildContext.Item, writer.ToString());
        }

        [CanBeNull]
        private Rendering FindRendering([NotNull] BuildContext context, [NotNull] string renderingName, [NotNull] string renderingId, TextSpan textSpan)
        {
            var renderings = context.Project.ProjectItems.OfType<Rendering>().Where(r => r.ItemName == renderingName).ToList();

            if (renderings.Count != 1)
            {
                if (!string.IsNullOrEmpty(renderingId))
                {
                    Guid guid;
                    if (Guid.TryParse(renderingId, out guid))
                    {
                        renderings = context.Project.ProjectItems.OfType<Rendering>().Where(r => r.Uri.Guid == guid).ToList();
                    }
                    else
                    {
                        renderings = context.Project.ProjectItems.OfType<Rendering>().Where(r => string.Equals(r.QualifiedName, renderingId, StringComparison.OrdinalIgnoreCase)).ToList();
                    }
                }
            }

            if (!renderings.Any())
            {
                context.CompileContext.Trace.TraceError(0, "Rendering not found", context.SourceFile.AbsoluteFileName, textSpan, renderingName);
                return null;
            }

            if (renderings.Count > 1)
            {
                context.CompileContext.Trace.TraceError(0, "Ambigeous rendering", context.SourceFile.AbsoluteFileName, textSpan, renderingName);
                return null;
            }

            return renderings.First();
        }

        private void ParseHtml([NotNull] BuildContext context)
        {
            var html = FileSystem.ReadAllText(context.SourceFile.AbsoluteFileName);
            if (string.IsNullOrEmpty(html))
            {
                return;
            }

            var root = html.ToXElement(LoadOptions.PreserveWhitespace);
            if (root == null)
            {
                context.CompileContext.Trace.TraceError(Msg.C1048, "Page Html is not valid", context.SourceFile);
                return;
            }

            WriteLayout(context, root);
        }

        private void SetRenderingsField([NotNull] BuildContext context, [NotNull] Item item, [NotNull] string fieldValue)
        {
            var field = item.Fields["__Renderings"];
            if (field == null)
            {
                field = context.CompileContext.Factory.Field(item, TextNode.Empty, "__Renderings", fieldValue);
                item.Fields.Add(field);
            }
            else
            {
                field.Value = fieldValue;
            }
        }

        private void WriteLayout([NotNull] BuildContext context, [NotNull] XElement root)
        {
            context.Output.WriteStartElement("r");

            var deviceItems = context.Project.ProjectItems.OfType<Item>().Where(i => string.Equals(i.TemplateIdOrPath, "/sitecore/templates/System/Layout/Device", StringComparison.OrdinalIgnoreCase) || string.Equals(i.TemplateIdOrPath, "{B6F7EEB4-E8D7-476F-8936-5ACE6A76F20B}", StringComparison.OrdinalIgnoreCase));
            var deviceItem = deviceItems.First();

            context.Output.WriteStartElement("d");
            context.Output.WriteAttributeString("id", deviceItem.Uri.Guid.Format());
            context.Output.WriteAttributeString("l", context.LayoutItem.Uri.Guid.Format());

            WriteRenderings(context, root);
            WriteLiteral(context);

            context.Output.WriteEndElement();

            context.Output.WriteEndElement();
        }

        private void WriteLiteral([NotNull] BuildContext context, [NotNull] XElement element)
        {
            context.Literal.Write("<");

            context.Literal.Write(element.Name.LocalName);

            foreach (var attribute in element.Attributes())
            {
                context.Literal.Write(" ");

                if (attribute.Name.Namespace.NamespaceName == "http://www.w3.org/2000/xmlns/")
                {
                    context.Literal.Write("xmlns:");
                }

                context.Literal.Write(attribute.Name.LocalName);
                context.Literal.Write("=\"");
                context.Literal.Write(HttpUtility.HtmlAttributeEncode(attribute.Value));
                context.Literal.Write("\"");
            }

            context.Literal.Write(">");

            foreach (var node in element.Nodes())
            {
                WriteRenderings(context, node);
            }

            context.Literal.Write("</");
            context.Literal.Write(element.Name.LocalName);
            context.Literal.Write(">");
        }

        private void WriteLiteral([NotNull] BuildContext context)
        {
            var text = context.Literal.ToString();
            context.Literal = new StringWriter();

            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            var par = new UrlString();
            par.Parameters["text"] = text;

            context.Output.WriteStartElement("r");
            context.Output.WriteAttributeString("id", context.LiteralRendering.RenderingItemUri.Guid.Format());
            context.Output.WriteAttributeString("ph", context.Placeholder);
            context.Output.WriteAttributeString("par", par.ToString());
            context.Output.WriteEndElement();
        }

        private void WriteRendering([NotNull] BuildContext context, [NotNull] XElement element)
        {
            WriteLiteral(context);

            var rendering = FindRendering(context, element.Name.LocalName, element.GetAttributeValue("renderingid"), new TextSpan(element));
            if (rendering == null)
            {
                return;
            }

            context.Output.WriteStartElement("r");
            context.Output.WriteAttributeString("id", rendering.RenderingItemUri.Guid.Format());
            context.Output.WriteAttributeString("ph", context.Placeholder);

            var parameters = new UrlString();

            foreach (var attribute in element.Attributes())
            {
                switch (attribute.Name.LocalName.ToLowerInvariant())
                {
                    case "renderingid":
                        continue;
                    case "datasource":
                        context.Output.WriteAttributeString("ds", attribute.Value);
                        continue;
                    case "cacheable":
                        context.Output.WriteAttributeString("cac", attribute.Value);
                        continue;
                    case "conditions":
                        context.Output.WriteAttributeString("cnd", attribute.Value);
                        continue;
                    case "tests":
                        context.Output.WriteAttributeString("mvt", attribute.Value);
                        continue;
                    case "placeholder":
                        context.Output.WriteAttributeString("ph", attribute.Value);
                        continue;
                    case "uniqueid":
                        context.Output.WriteAttributeString("uid", attribute.Value);
                        continue;
                    case "varybydata":
                        context.Output.WriteAttributeString("vbd", attribute.Value);
                        continue;
                    case "varybydevice":
                        context.Output.WriteAttributeString("vbdev", attribute.Value);
                        continue;
                    case "varybylogin":
                        context.Output.WriteAttributeString("vbl", attribute.Value);
                        continue;
                    case "varybyparameters":
                        context.Output.WriteAttributeString("vbp", attribute.Value);
                        continue;
                    case "varybyquerystring":
                        context.Output.WriteAttributeString("vbqs", attribute.Value);
                        continue;
                    case "varybyuser":
                        context.Output.WriteAttributeString("vbu", attribute.Value);
                        continue;
                    case "rules":
                        context.Output.WriteAttributeString("rls", attribute.Value);
                        continue;
                    default:
                        parameters.Parameters[attribute.Name.LocalName] = attribute.Value;
                        break;
                }
            }

            var p = parameters.ToString();
            if (!string.IsNullOrEmpty(p))
            {
                context.Output.WriteAttributeString("par", p);
            }

            context.Output.WriteEndElement();

            var placeholder = context.Placeholder;

            foreach (var placeholderElement in element.Elements())
            {
                if (placeholderElement.Name.NamespaceName != PlaceholderNamespaceName)
                {
                    context.CompileContext.Trace.TraceError(0, "Placeholder expected", context.SourceFile.AbsoluteFileName, new TextSpan(placeholderElement), placeholderElement.Name.LocalName);
                }

                context.Placeholder = placeholderElement.Name.LocalName;

                foreach (var node in placeholderElement.Nodes())
                {
                    WriteRenderings(context, node);
                }

                WriteLiteral(context);
            }

            context.Placeholder = placeholder;
        }

        private void WriteRenderings([NotNull] BuildContext context, [NotNull] XNode node)
        {
            var element = node as XElement;
            if (element != null)
            {
                if (element.Name.NamespaceName == RenderingNamespaceName)
                {
                    WriteRendering(context, element);
                }
                else
                {
                    WriteLiteral(context, element);
                }
            }
            else
            {
                context.Literal.Write(node.ToString());
            }
        }

        private class BuildContext
        {
            public BuildContext([NotNull] ICompileContext context, [NotNull] ISourceFile sourceFile, [NotNull] XmlTextWriter output, [NotNull] Item item, [NotNull] Item layoutItem)
            {
                CompileContext = context;
                SourceFile = sourceFile;
                Output = output;
                Item = item;
                LayoutItem = layoutItem;
            }

            [NotNull]
            public ICompileContext CompileContext { get; }

            [NotNull]
            public Item Item { get; }

            [NotNull]
            public Item LayoutItem { get; }

            [NotNull]
            public StringWriter Literal { get; set; } = new StringWriter();

            [NotNull]
            public Rendering LiteralRendering { get; set; }

            [NotNull]
            public XmlTextWriter Output { get; }

            [NotNull]
            public string Placeholder { get; set; } = string.Empty;

            [NotNull]
            public IProject Project => Item.Project;

            [NotNull]
            public ISourceFile SourceFile { get; }
        }
    }
}
