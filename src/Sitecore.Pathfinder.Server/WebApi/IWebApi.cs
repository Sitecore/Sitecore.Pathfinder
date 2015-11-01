// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Web.Mvc;

namespace Sitecore.Pathfinder.WebApi
{
    public interface IWebApi
    {
        [Diagnostics.CanBeNull]
        ActionResult Execute();
    }
}
