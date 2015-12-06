// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.IO;
using Sitecore.Pathfinder.Compiling.Builders;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Projects.Templates;

namespace Sitecore.Pathfinder.Languages
{
    public abstract class LanguageBase : ILanguage
    {
        public abstract bool CanHandleExtension(string extension);

        public abstract void WriteItem(TextWriter textWriter, Item item);

        public abstract void WriteLayout(TextWriter textWriter, string databaseName, LayoutBuilder layoutBuilder);

        public abstract void WriteTemplate(TextWriter textWriter, Template template);
    }
}
