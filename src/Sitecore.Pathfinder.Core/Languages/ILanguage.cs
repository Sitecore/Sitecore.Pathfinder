// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using System.IO;
using Sitecore.Pathfinder.Compiling.Builders;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Projects.Templates;

namespace Sitecore.Pathfinder.Languages
{
    [InheritedExport]
    public interface ILanguage
    {
        bool CanHandleExtension([NotNull] string extension);

        void WriteItem([NotNull] TextWriter textWriter, [NotNull] Item item);

        void WriteLayout([NotNull] TextWriter textWriter, [NotNull] string databaseName, [NotNull] LayoutBuilder layoutBuilder);

        void WriteTemplate([NotNull] TextWriter textWriter, [NotNull] Template template);
    }
}
