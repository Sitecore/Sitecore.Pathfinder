// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;

namespace Sitecore.Data.Templates
{
    public class TemplateSection
    {
        [NotNull]
        public string Key
        {
            get { throw new NotImplementedException(); }
        }

        public int Sortorder
        {
            get { throw new NotImplementedException(); }
        }

        [NotNull]
        public ID ID
        {
            get { throw new NotImplementedException(); }
        }

        [NotNull]
        public string Name
        {
            get { throw new NotImplementedException(); }
        }

        [NotNull, ItemNotNull]
        public TemplateField[] GetFields()
        {
            throw new NotImplementedException();
        }
    }
}
