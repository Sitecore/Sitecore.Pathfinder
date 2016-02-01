// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;

namespace Sitecore.Data.Templates
{
    public class Template
    {
        [NotNull]
        public ID[] BaseIDs
        {
            get { throw new NotImplementedException(); }
        }

        [NotNull]
        public ID ID { get; private set; }

        [NotNull]
        public string Name { get; private set; }

        [CanBeNull]
        public TemplateField GetField([NotNull] string fieldName)
        {
            throw new NotImplementedException();
        }

        [NotNull, ItemNotNull]
        public TemplateField[] GetFields(bool includeBaseFields)
        {
            throw new NotImplementedException();
        }
    }
}
