// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.Net;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.WebServer.Web
{
    public interface IHttpListenerHandler
    {
        double Priority { get; }

        bool CanProcessRequest([NotNull] HttpListenerContext context, [NotNull] IProject project);

        void ProcessRequest([NotNull] HttpListenerContext context, [NotNull] IProject project);
    }
}
