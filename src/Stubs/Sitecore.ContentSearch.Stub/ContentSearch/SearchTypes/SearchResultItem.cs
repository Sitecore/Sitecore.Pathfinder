// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Data;

namespace Sitecore.ContentSearch.SearchTypes
{
    public class SearchResultItem
    {
        [NotNull]
        public ID TemplateId
        {
            get { throw new NotImplementedException(); }
        }

        [NotNull]
        public ID ItemId
        {
            get { throw new NotImplementedException(); }
        }
    }
}
