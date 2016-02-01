// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Data.Fields;

namespace Sitecore.Data.Templates
{
    public class Template
    {
        public string Name { get; private set; }

        public ID ID { get; private set; }

        public IEnumerable<TemplateField> GetFields(bool b)
        {
            yield break;
        }
    }
}