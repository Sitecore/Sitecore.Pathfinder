// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Runtime.CompilerServices;
using Sitecore.Data.Items;

namespace Sitecore.Mvc.Presentation
{
    public class Rendering
    {
        [CanBeNull, IndexerName("indexer")]
        public string this[[NotNull] string propertyName]
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        [NotNull]
        public RenderingItem RenderingItem
        {
            get { throw new NotImplementedException(); }
        }

        [NotNull]
        public string RenderingType
        {
            get { throw new NotImplementedException(); }
        }

        [NotNull]
        public string DataSource
        {
            get { throw new NotImplementedException(); }
        }
    }
}
