// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Projects.Items.FieldResolvers
{
    public interface IFieldResolver
    {
        double Priority { get; }

        bool CanResolve(ITraceService trace, [NotNull] IProject project, [NotNull] Field field);

        [NotNull]
        string Resolve(ITraceService trace, [NotNull] IProject project, [NotNull] Field field);
    }
}
