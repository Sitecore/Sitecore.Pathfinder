// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.IO;

namespace Sitecore.Data.Items
{
    public class MediaItem : CustomItemBase
    {
        public MediaItem([NotNull] Item innerItem) : base(innerItem)
        {
        }

        [NotNull]
        public string Extension
        {
            get { throw new NotImplementedException(); }
        }

        public long Size
        {
            get { throw new NotImplementedException(); }
        }

        [NotNull]
        public Stream GetMediaStream()
        {
            throw new NotImplementedException();
        }
    }
}
