// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.WebServer.Web
{
    public class StaticFileHttpListenerHandler : HttpListenerHandlerBase
    {
        public StaticFileHttpListenerHandler() : base(9000)
        {
        }

        public override bool CanProcessRequest(HttpListenerContext context, IProject project)
        {
            var filePath = "~" + PathHelper.NormalizeItemPath(context.Request.Url.AbsolutePath);

            return project.Files.Any(f => string.Equals(f.FilePath, filePath, StringComparison.OrdinalIgnoreCase));
        }

        public override void ProcessRequest(HttpListenerContext context, IProject project)
        {
            var filePath = "~" + PathHelper.NormalizeItemPath(context.Request.Url.AbsolutePath);

            var file = project.Files.First(f => string.Equals(f.FilePath, filePath, StringComparison.OrdinalIgnoreCase));
            var fileName = file.Snapshot.SourceFile.AbsoluteFileName;

            context.Response.StatusCode = 200;
            context.Response.ContentType = MimeMapping.GetMimeMapping(fileName);
            using (var stream = new FileStream(fileName, FileMode.Open))
            {
                stream.CopyTo(context.Response.OutputStream);
            }
        }
    }
}
