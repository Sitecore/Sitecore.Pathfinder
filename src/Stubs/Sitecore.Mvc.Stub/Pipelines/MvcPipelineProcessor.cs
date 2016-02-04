// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

namespace Sitecore.Mvc.Pipelines
{
    public abstract class MvcPipelineProcessor<TArg> where TArg : MvcPipelineArgs
    {
        public abstract void Process(TArg args);
    }
}
