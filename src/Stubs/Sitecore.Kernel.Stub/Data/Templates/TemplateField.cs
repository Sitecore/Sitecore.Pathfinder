// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Globalization;

namespace Sitecore.Data.Templates
{
    public class TemplateField
    {
        [NotNull]
        public ID ID
        {
            get { throw new NotImplementedException(); }
        }

        [NotNull]
        public string Key
        {
            get { throw new NotImplementedException(); }
        }

        [NotNull]
        public string Name
        {
            get { throw new NotImplementedException(); }
        }

        [NotNull]
        public TemplateSection Section
        {
            get { throw new NotImplementedException(); }
        }

        public int Sortorder
        {
            get { throw new NotImplementedException(); }
        }

        [NotNull]
        public string Source
        {
            get { throw new NotImplementedException(); }
        }

        [NotNull]
        public Template Template
        {
            get { throw new NotImplementedException(); }
        }

        [NotNull]
        public string Type
        {
            get { throw new NotImplementedException(); }
        }

        [NotNull]
        public string GetToolTip([NotNull] Language language)
        {
            throw new NotImplementedException();
        }
    }
}
