// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects.Items;

// to generate IFactory.generated.cs, run "gulp generate-factory" in the root directory

namespace Sitecore.Pathfinder.Configuration
{
    public partial interface IFactory
    {
        [NotNull]
        Field Field([NotNull] Item item, [NotNull] string fieldName, [NotNull] string fieldValue);

        [NotNull]
        T Resolve<T>();

        [CanBeNull]
        T Resolve<T>([NotNull] string contractName);

        [NotNull, ItemNotNull]
        IEnumerable<T> ResolveMany<T>();

        [NotNull]
        // ReSharper disable once InconsistentNaming
        XmlWriter XmlWriter([NotNull] TextWriter writer, bool encoderShouldEmitUTF8Identifier = false);

        [NotNull]
        // ReSharper disable once InconsistentNaming
        XmlWriter XmlWriter([NotNull] StringBuilder writer, bool encoderShouldEmitUTF8Identifier = false);
    }
}
