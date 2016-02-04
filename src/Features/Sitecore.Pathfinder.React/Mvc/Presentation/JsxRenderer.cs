// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using React;
using React.Exceptions;
using React.TinyIoC;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Mvc;
using Sitecore.Mvc.Helpers;
using Sitecore.Mvc.Presentation;
using Sitecore.Web.UI.WebControls;

namespace Sitecore.Pathfinder.Mvc.Presentation
{
    public class JsxRenderer : Renderer
    {
        [Diagnostics.NotNull]
        public string FilePath { get; set; } = string.Empty;

        [Diagnostics.NotNull]
        public Sitecore.Mvc.Presentation.Rendering Rendering { get; set; }

        public static IReactEnvironment Environment
        {
            get
            {
                IReactEnvironment current;
                try
                {
                    current = ReactEnvironment.Current;
                }
                catch (TinyIoCResolutionException tinyIoCResolutionException)
                {
                    throw new ReactNotInitialisedException("ReactJS.NET has not been initialised correctly.", tinyIoCResolutionException);
                }

                return current;
            }
        }

        public override void Render([Diagnostics.NotNull] TextWriter writer)
        {
            var sitecoreHelper = PageContext.Current.HtmlHelper.Sitecore();

            var output = Render(sitecoreHelper, Context.Item, FilePath);

            writer.Write(output);
        }

        protected virtual Item GetDataSourceItem()
        {
            if (!string.IsNullOrEmpty(Rendering.DataSource))
            {
                return Context.Database.GetItem(Rendering.DataSource) ?? Context.Item;
            }

            return Context.Item;
        }

        protected virtual dynamic GetProps()
        {
            dynamic props = new ExpandoObject();
            var dataSourceItem = GetDataSourceItem();

            foreach (Field field in dataSourceItem.Fields)
            {
                if (field.Name.StartsWith("__"))
                {
                    continue;
                }

                ((IDictionary<string, object>)props).Add(field.Name.ToLowerInvariant(), FieldRenderer.Render(dataSourceItem, field.Name));
            }

            return props;
        }

        [Diagnostics.NotNull]
        protected virtual string Render([Diagnostics.NotNull] SitecoreHelper sitecoreHelper, [Diagnostics.NotNull] Item contextItem, [Diagnostics.NotNull] string filePath)
        {
            var props = GetProps();

            IReactComponent reactComponent = Environment.CreateComponent("Name", props);

            var output = new StringWriter();

            output.Write(reactComponent.RenderHtml());
            output.Write($"<script>{reactComponent.RenderJavaScript()}</script>");

            return output.ToString();
        }
    }
}
