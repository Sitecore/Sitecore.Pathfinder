// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Languages.Json;
using Sitecore.Pathfinder.Languages.Xml;
using Sitecore.Pathfinder.Languages.Yaml;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.WebServer.Web
{
    public class ItemHttpListenerHandler : HttpListenerHandlerBase
    {
        public ItemHttpListenerHandler() : base(1000)
        {
        }

        public override bool CanProcessRequest(HttpListenerContext context, IProject project)
        {
            var extension = Path.GetExtension(context.Request.Url.AbsolutePath);
            if ((extension != ".json") && (extension != ".xml") && (extension != ".yaml"))
            {
                return false;
            }

            var itemPath = PathHelper.NormalizeItemPath(context.Request.Url.AbsolutePath);
            var n = itemPath.LastIndexOf('.');
            if (n >= 0)
            {
                itemPath = itemPath.Left(n);
            }

            return project.Items.Any(i => string.Equals(i.ItemIdOrPath, itemPath, StringComparison.OrdinalIgnoreCase));
        }

        public override void ProcessRequest(HttpListenerContext context, IProject project)
        {
            var itemPath = PathHelper.NormalizeItemPath(context.Request.Url.AbsolutePath);
            var n = itemPath.LastIndexOf('.');
            if (n >= 0)
            {
                itemPath = itemPath.Left(n);
            }

            var item = project.Items.First(i => string.Equals(i.ItemIdOrPath, itemPath, StringComparison.OrdinalIgnoreCase));

            var output = new StringWriter();

            switch (Path.GetExtension(context.Request.Url.AbsolutePath))
            {
                case ".json":
                    item.WriteAsJson(output);
                    break;
                case ".xml":
                    item.WriteAsXml(output);
                    break;
                case ".yaml":
                    item.WriteAsYaml(output);
                    break;
            }

            var text = output.ToString();
            var buffer = Encoding.UTF8.GetBytes(text);

            context.Response.ContentType = MimeMapping.GetMimeMapping(context.Request.Url.AbsolutePath);
            context.Response.ContentLength64 = buffer.Length;
            context.Response.OutputStream.Write(buffer, 0, buffer.Length);
        }
    }
}
