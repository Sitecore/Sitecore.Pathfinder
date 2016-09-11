// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using System.Net;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.WebServer.Web
{
    [InheritedExport(typeof(IHttpListenerHandler))]
    public abstract class HttpListenerHandlerBase : IHttpListenerHandler
    {
        protected HttpListenerHandlerBase(double priority)
        {
            Priority = priority;
        }

        public double Priority { get; }

        public abstract bool CanProcessRequest(HttpListenerContext context, IProject project);

        public abstract void ProcessRequest(HttpListenerContext context, IProject project);
    }
}
