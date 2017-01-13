// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks
{
    public class UpdateWebsiteFiles : WebBuildTaskBase
    {
        [ImportingConstructor]
        public UpdateWebsiteFiles() : base("update-website-files")
        {
        }

        public override void Run(IBuildContext context)
        {
            UpdateWebsiteFiles(context);
        }
    }
}
