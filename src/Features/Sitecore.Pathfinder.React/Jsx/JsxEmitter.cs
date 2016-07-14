// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.IO;
using System.Linq;
using Sitecore.Pathfinder.Emitting;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Languages.Renderings;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.React.Jsx
{
    public class JsxEmitter : EmitterBase
    {
        public JsxEmitter() : base(Constants.Emitters.Layouts)
        {
        }

        public override bool CanEmit(IEmitContext context, IProjectItem projectItem)
        {
            var rendering = projectItem as Rendering;
            return rendering != null && string.Equals(PathHelper.GetExtension(rendering.FilePath), ".jsx", StringComparison.OrdinalIgnoreCase);
        }

        public override void Emit(IEmitContext context, IProjectItem projectItem)
        {
            // validate the ReactJS.NET is installed
            var binDirectory = Path.Combine(context.Configuration.GetWebsiteDirectory(), "bin");
            if (!context.FileSystem.DirectoryExists(binDirectory))
            {
                return;
            }

            var reactFileName = Path.Combine(binDirectory, "React.Core.dll");
            if (!context.FileSystem.FileExists(reactFileName))
            {
                context.Trace.TraceWarning(Msg.E1007, "React.Core.dll is missing from the website /bin directory. ReactJS.net is probably not installed correctly, and .jsx renderings will not work.", projectItem.Snapshot.SourceFile);
            }
        }
    }
}
