// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Data.Templates;
using Sitecore.Pathfinder.Emitters;
using Sitecore.Pathfinder.Projects.Items;

namespace Sitecore.Pathfinder.Builders.FieldResolvers
{
    public interface IFieldResolver
    {
        double Priority { get; }

        bool CanResolve([Diagnostics.NotNull] IEmitContext context, [Diagnostics.NotNull] TemplateField templateField, [Diagnostics.NotNull] Field field);

        [Diagnostics.NotNull]
        string Resolve([Diagnostics.NotNull] IEmitContext context, [Diagnostics.NotNull] TemplateField templateField, [Diagnostics.NotNull] Field field);
    }
}
