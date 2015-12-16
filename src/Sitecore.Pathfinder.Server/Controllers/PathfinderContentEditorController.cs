// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Web.Mvc;
using Sitecore.Pathfinder.ProjectTrees;
using Sitecore.Web;

namespace Sitecore.Pathfinder.Controllers
{
    public class PathfinderContentEditorController : Controller
    {
        [Diagnostics.NotNull]
        public ActionResult Index([Diagnostics.NotNull] string route)
        {
            var uri = WebUtil.GetQueryString("uri");

            var selection = new List<ProjectTreeUri>();
            if (!string.IsNullOrEmpty(uri))
            {
                selection.Add(new ProjectTreeUri(uri));
            }

            ViewBag.Selection = selection;

            return View("~/sitecore/shell/client/Applications/Pathfinder/ContentEditors/shell.cshtml");
        }
    }
}
