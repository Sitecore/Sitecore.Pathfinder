// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using React;
using React.Exceptions;
using React.TinyIoC;
using Sitecore.Data.Fields;
using Sitecore.Mvc;
using Sitecore.Mvc.Presentation;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Mvc.Presentation;
using Sitecore.Web.UI.WebControls;

namespace Sitecore.Pathfinder.React.Mvc.Presentation
{
    public class JsxRenderer : IRenderer
    {
        public JsxRenderer([Diagnostics.NotNull] Sitecore.Mvc.Presentation.Rendering rendering, [Diagnostics.NotNull] string filePath)
        {
            Rendering = rendering;
            FilePath = filePath;
        }

        protected static IReactEnvironment Environment
        {
            get
            {
                try
                {
                    return ReactEnvironment.Current;
                }
                catch (TinyIoCResolutionException ex)
                {
                    throw new ReactNotInitialisedException("ReactJS.NET has not been initialised correctly.", ex);
                }
            }
        }

        [Diagnostics.NotNull]
        protected string FilePath { get; }

        [Diagnostics.NotNull]
        protected Sitecore.Mvc.Presentation.Rendering Rendering { get; }

        public void Render(TextWriter writer)
        {
            var props = GetProps();

            var componentName = Path.GetFileNameWithoutExtension(FilePath).Replace("-", string.Empty);
            IReactComponent reactComponent = Environment.CreateComponent(componentName, props);

            writer.WriteLine(reactComponent.RenderHtml());
            writer.WriteLine($"<script>{reactComponent.RenderJavaScript()}</script>");
        }

        protected virtual dynamic GetProps()
        {
            dynamic props = new ExpandoObject();
            var propsDictionary = (IDictionary<string, object>)props;

            dynamic placeholders = new ExpandoObject();
            var placeholdersDictionary = (IDictionary<string, object>)placeholders;

            propsDictionary["placeholders"] = placeholders;

            var dataSourceItem = !string.IsNullOrEmpty(Rendering.DataSource) ? (Context.Database.GetItem(Rendering.DataSource) ?? Context.Item) : Context.Item;
            foreach (Field field in dataSourceItem.Fields)
            {
                if (!field.Name.StartsWith("__"))
                {
                    propsDictionary.Add(field.Name, FieldRenderer.Render(dataSourceItem, field.Name));
                }
            }

            var placeholdersField = Rendering.RenderingItem.InnerItem["Place Holders"];
            if (string.IsNullOrEmpty(placeholdersField))
            {
                return props;
            }

            var controlId = Rendering.Parameters["id"] ?? string.Empty;
            dynamic placeholderId = null;

            var placeholderKeys = placeholdersField.Split(Constants.Comma, StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()).ToList();
            foreach (var placeholderKey in placeholderKeys)
            {
                if (placeholderKey.StartsWith("$Id."))
                {
                    if (placeholderId == null)
                    {
                        placeholderId = new ExpandoObject();
                        placeholdersDictionary["$Id"] = placeholderId;
                    }

                    ((IDictionary<string, object>)placeholderId)[placeholderKey.Mid(3)] = PageContext.Current.HtmlHelper.Sitecore().Placeholder(controlId + placeholderKey.Mid(3)).ToString();
                }
                else
                {
                    placeholdersDictionary[placeholderKey] = PageContext.Current.HtmlHelper.Sitecore().Placeholder(placeholderKey).ToString();
                }
            }

            return props;
        }
    }
}
