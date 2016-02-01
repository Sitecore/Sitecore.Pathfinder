// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.IO;
using Sitecore.Data.Items;

namespace Sitecore.Resources.Media
{
    public class MediaCreator
    {
        [CanBeNull]
        public Item CreateFromStream([NotNull] Stream stream, [NotNull] string path, [NotNull] MediaCreatorOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
