// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Globalization;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Compiling.FieldCompilers
{
    public interface IFieldCompileContext
    {
        [NotNull]
        CultureInfo Culture { get; }

        [NotNull, ItemNotNull]
        IEnumerable<IFieldCompiler> FieldCompilers { get; }

        [NotNull]
        ITraceService Trace { get; }
    }
}
