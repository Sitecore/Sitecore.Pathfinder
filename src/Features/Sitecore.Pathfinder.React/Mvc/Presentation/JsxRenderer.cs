// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.IO;
using React;
using React.Exceptions;
using React.TinyIoC;
using React.Web.Mvc;
using Sitecore.Mvc.Presentation;

namespace Sitecore.Pathfinder.Mvc.Presentation
{
    public class JsxRenderer : Renderer
    {
        [Diagnostics.NotNull]
        public string FilePath { get; set; } = string.Empty;

        [Diagnostics.NotNull]
        public Rendering Rendering { get; set; }

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
            var helper = PageContext.Current.HtmlHelper;

            writer.WriteLine(helper.React(Path.GetFileNameWithoutExtension(FilePath), new { name = "Daniel" }));
        }
    }
}
