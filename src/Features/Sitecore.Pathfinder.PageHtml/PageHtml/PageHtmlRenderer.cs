// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.IO;
using Sitecore.Mvc;
using Sitecore.Mvc.Pipelines;
using Sitecore.Mvc.Pipelines.Response.RenderRendering;
using Sitecore.Mvc.Presentation;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Mvc.Presentation;
using Sitecore.Text;

namespace Sitecore.Pathfinder.PageHtml.PageHtml
{
    public class PageHtmlRenderer : IRenderer
    {
        public const string FieldNamespaceName = "http://www.sitecore.net/pathfinder/field";

        public const string PlaceholderNamespaceName = "http://www.sitecore.net/pathfinder/placeholder";

        public const string RenderingNamespaceName = "http://www.sitecore.net/pathfinder/rendering";

        [Diagnostics.NotNull]
        public string FilePath { get; set; } = string.Empty;

        [Diagnostics.CanBeNull]
        public Rendering Rendering { get; set; }

        public void Render(TextWriter writer)
        {
            if (!FileUtil.FileExists(FilePath))
            {
                return;
            }

            var html = FileUtil.ReadFromFile(FilePath);

            var renderings = GetRenderings(Context.Item.Database, html);

            var pageDefinition = new PageDefinition();
            pageDefinition.Renderings.AddRange(renderings);

            var oldPageDefinition = PageContext.Current.PageDefinition;
            PageContext.Current.PageDefinition = pageDefinition;

            foreach (var rendering in pageDefinition.Renderings.Where(r => string.IsNullOrEmpty(r.Placeholder)))
            {
                PipelineService.Get().RunPipeline("mvc.renderRendering", new RenderRenderingArgs(rendering, writer));
            }

            PageContext.Current.PageDefinition = oldPageDefinition;
        }

        [CanBeNull]
        protected virtual Item FindRendering([NotNull] RenderContext context, [NotNull] string renderingName, [NotNull] string renderingId)
        {
            var renderings = context.RenderingItems.Where(r => r.Name == renderingName).ToList();

            if (renderings.Count != 1)
            {
                if (!string.IsNullOrEmpty(renderingId))
                {
                    Guid guid;
                    if (Guid.TryParse(renderingId, out guid))
                    {
                        renderings = context.RenderingItems.Where(r => r.ID.Guid == guid).ToList();
                    }
                    else
                    {
                        renderings = context.RenderingItems.Where(r => string.Equals(r.Paths.Path, renderingId, StringComparison.OrdinalIgnoreCase)).ToList();
                    }
                }
            }

            if (!renderings.Any())
            {
                throw new InvalidOperationException("Rendering not found");
            }

            if (renderings.Count > 1)
            {
                throw new InvalidOperationException("Ambiguous rendering");
            }

            return renderings.First();
        }

        protected virtual void WriteField([NotNull] RenderContext context, [NotNull] XElement element)
        {
            WriteLiteral(context);

            var fieldName = element.GetAttributeValue("name");
            if (string.IsNullOrEmpty(fieldName))
            {
                fieldName = element.Name.LocalName;
            }

            var rendering = new Rendering
            {
                Placeholder = context.Placeholder,
                Renderer = new Field(fieldName),
                RenderingType = "View",
                DeviceId = Context.Device.ID.Guid
            };

            context.Renderings.Add(rendering);
        }

        protected virtual void WriteLiteral([NotNull] RenderContext context)
        {
            var text = context.LiteralText.ToString();
            context.LiteralText = new StringWriter();

            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            var rendering = new Rendering
            {
                Placeholder = context.Placeholder,
                Renderer = new Literal(text),
                RenderingType = "View",
                DeviceId = Context.Device.ID.Guid
            };

            context.Renderings.Add(rendering);
        }

        protected virtual void WriteLiteral([NotNull] RenderContext context, [NotNull] XElement element)
        {
            context.LiteralText.Write("<");

            context.LiteralText.Write(element.Name.LocalName);

            foreach (var attribute in element.Attributes())
            {
                context.LiteralText.Write(" ");

                if (attribute.Name.Namespace.NamespaceName == "http://www.w3.org/2000/xmlns/")
                {
                    context.LiteralText.Write("xmlns:");
                }

                context.LiteralText.Write(attribute.Name.LocalName);
                context.LiteralText.Write("=\"");
                context.LiteralText.Write(HttpUtility.HtmlAttributeEncode(attribute.Value));
                context.LiteralText.Write("\"");
            }

            context.LiteralText.Write(">");

            foreach (var node in element.Nodes())
            {
                WriteRenderings(context, node);
            }

            context.LiteralText.Write("</");
            context.LiteralText.Write(element.Name.LocalName);
            context.LiteralText.Write(">");
        }

        protected virtual void WriteRendering([NotNull] RenderContext context, [NotNull] XElement element)
        {
            WriteLiteral(context);

            var renderingItem = FindRendering(context, element.Name.LocalName, element.GetAttributeValue("renderingid"));
            if (renderingItem == null)
            {
                return;
            }

            var rendering = new Rendering
            {
                Id = renderingItem.ID.Guid,
                Placeholder = context.Placeholder,
                RenderingItem = new RenderingItem(renderingItem),
                RenderingType = "View",
                DeviceId = Context.Device.ID.Guid
            };

            var parameters = new UrlString();

            foreach (var attribute in element.Attributes())
            {
                switch (attribute.Name.LocalName.ToLowerInvariant())
                {
                    case "renderingid":
                        continue;
                    case "datasource":
                        rendering.DataSource = attribute.Value;
                        continue;
                    case "cacheable":
                        rendering.Caching.Cacheable = attribute.Value == "1";
                        continue;
                    case "conditions":

                        // todo: conditions
                        continue;
                    case "tests":

                        // todo: mv tests
                        continue;
                    case "placeholder":
                        rendering.Placeholder = attribute.Value;
                        continue;
                    case "uniqueid":
                        rendering.UniqueId = Guid.Parse(attribute.Value);
                        continue;
                    case "varybydata":
                        rendering.Caching.VaryByData = attribute.Value == "1";
                        continue;
                    case "varybydevice":
                        rendering.Caching.VaryByDevice = attribute.Value == "1";
                        continue;
                    case "varybylogin":
                        rendering.Caching.VaryByLogin = attribute.Value == "1";
                        continue;
                    case "varybyparameters":
                        rendering.Caching.VaryByParameters = attribute.Value == "1";
                        continue;
                    case "varybyquerystring":
                        rendering.Caching.VaryByQueryString = attribute.Value == "1";
                        continue;
                    case "varybyuser":
                        rendering.Caching.VaryByUser = attribute.Value == "1";
                        continue;
                    case "rules":

                        // todo: mv tests
                        continue;
                    default:
                        parameters.Parameters[attribute.Name.LocalName] = attribute.Value;
                        break;
                }
            }

            var p = parameters.ToString();
            if (!string.IsNullOrEmpty(p))
            {
                rendering.Parameters = new RenderingParameters(parameters.ToString());
            }

            var placeholder = context.Placeholder;

            foreach (var placeholderElement in element.Elements())
            {
                if (placeholderElement.Name.NamespaceName != PlaceholderNamespaceName)
                {
                    throw new InvalidOperationException("Placeholder expected");
                }

                context.Placeholder = placeholderElement.Name.LocalName;

                foreach (var node in placeholderElement.Nodes())
                {
                    WriteRenderings(context, node);
                }

                WriteLiteral(context);
            }

            context.Placeholder = placeholder;

            context.Renderings.Add(rendering);
        }

        [NotNull, ItemNotNull]
        private IEnumerable<Rendering> GetRenderings([NotNull] Database database, [NotNull] string html)
        {
            // todo: cache this!!!
            var renderings = database.GetItemsByTemplate(ServerConstants.Renderings.ViewRenderingId, ServerConstants.Renderings.MethodRendering, ServerConstants.Renderings.UrlRendering, ServerConstants.Renderings.WebcontrolRendering, TemplateIDs.XSLRendering);

            var root = html.ToXElement(LoadOptions.SetLineInfo);
            if (root == null)
            {
                throw new InvalidOperationException("Page Html is not valid");
            }

            var context = new RenderContext(renderings);

            WriteRenderings(context, root);
            WriteLiteral(context);

            return context.Renderings;
        }

        private void WriteRenderings([NotNull] RenderContext context, [NotNull] XNode node)
        {
            var element = node as XElement;
            if (element != null)
            {
                if (element.Name.NamespaceName == RenderingNamespaceName)
                {
                    WriteRendering(context, element);
                }
                else if (element.Name.NamespaceName == FieldNamespaceName)
                {
                    WriteField(context, element);
                }
                else
                {
                    WriteLiteral(context, element);
                }
            }
            else
            {
                context.LiteralText.Write(node.ToString());
            }
        }

        public class Field : Renderer
        {
            public Field([NotNull] string fieldName)
            {
                FieldName = fieldName;
            }

            [NotNull]
            protected string FieldName { get; }

            public override void Render([NotNull] TextWriter writer)
            {
                writer.Write(PageContext.Current.HtmlHelper.Sitecore().Field(FieldName, Context.Item));
            }
        }

        public class Literal : Renderer
        {
            public Literal([NotNull] string text)
            {
                Text = text;
            }

            [NotNull]
            protected string Text { get; }

            public override void Render([NotNull] TextWriter writer)
            {
                writer.Write(Text);
            }
        }

        protected class RenderContext
        {
            public RenderContext([NotNull, ItemNotNull] IEnumerable<Item> renderingItems)
            {
                RenderingItems = renderingItems;
            }

            [NotNull]
            public StringWriter LiteralText { get; set; } = new StringWriter();

            [NotNull]
            public string Placeholder { get; set; } = string.Empty;

            [NotNull, ItemNotNull]
            public IEnumerable<Item> RenderingItems { get; }

            [NotNull, ItemNotNull]
            public ICollection<Rendering> Renderings { get; } = new List<Rendering>();
        }
    }
}
