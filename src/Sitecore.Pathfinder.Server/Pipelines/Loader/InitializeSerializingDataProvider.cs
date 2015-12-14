// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Serializing;
using Sitecore.Pipelines;

namespace Sitecore.Pathfinder.Pipelines.Loader
{
    public class InitializeSerializingDataProvider
    {
        public void Process([NotNull] PipelineArgs args)
        {
            SerializingDataProviderService.Initialize();
        }
    }
}
