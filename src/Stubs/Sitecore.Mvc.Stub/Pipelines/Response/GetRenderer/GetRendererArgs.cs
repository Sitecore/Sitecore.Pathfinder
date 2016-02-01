// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Data.Items;
using Sitecore.Mvc.Presentation;

namespace Sitecore.Mvc.Pipelines.Response.GetRenderer
{
    public class GetRendererArgs : MvcPipelineArgs
    {
        [NotNull]
        public Rendering Rendering
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        [CanBeNull]
        public Renderer Result
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        [NotNull]
        public LayoutItem LayoutItem
        {
            get { throw new NotImplementedException(); }
        }
    }
}
