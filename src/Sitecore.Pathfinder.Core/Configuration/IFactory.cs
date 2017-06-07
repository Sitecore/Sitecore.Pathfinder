using System.IO;
using System.Xml;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Snapshots;

// to generate IFactory.generated.cs, run "gulp generate-factory" in the root directory

namespace Sitecore.Pathfinder.Configuration
{
    public partial interface IFactory
    {
        [NotNull]
        Field Field([NotNull] Item item, [NotNull] string fieldName, [NotNull] string fieldValue);

        [NotNull]
        ISnapshot Snapshot([NotNull] ISourceFile sourceFile);

        [NotNull]
        ISourceFile SourceFile([NotNull] IFileSystemService fileSystem, [NotNull] string absoluteFileName);

        [NotNull]
        // ReSharper disable once InconsistentNaming
        XmlWriter XmlWriter([NotNull] TextWriter writer, bool encoderShouldEmitUTF8Identifier = false);
    }
}
