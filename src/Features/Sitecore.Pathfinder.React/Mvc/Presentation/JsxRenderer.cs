// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using React;
using React.Exceptions;
using React.TinyIoC;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Mvc.Presentation;
using Sitecore.Web.UI.WebControls;

namespace Sitecore.Pathfinder.Mvc.Presentation
{
    public class JsxRenderer : Renderer
    {
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

        [Diagnostics.NotNull]
        public string FilePath { get; set; } = string.Empty;

        [Diagnostics.NotNull]
        public Sitecore.Mvc.Presentation.Rendering Rendering { get; set; }

        public override void Render([Diagnostics.NotNull] TextWriter writer)
        {
            var props = GetProps();

            IReactComponent reactComponent = Environment.CreateComponent(Path.GetFileNameWithoutExtension(FilePath), props);

            writer.WriteLine(reactComponent.RenderHtml());
            writer.WriteLine($"<script src=\"{FilePath}\"></script>");
            writer.WriteLine($"<script>{reactComponent.RenderJavaScript()}</script>");
        }

        private Item GetDataSourceItem()
        {
            return !string.IsNullOrEmpty(Rendering.DataSource) ? (Context.Database.GetItem(Rendering.DataSource) ?? Context.Item) : Context.Item;
        }

        private dynamic GetProps()
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
    }
}
