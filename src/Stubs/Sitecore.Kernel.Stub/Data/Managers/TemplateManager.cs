// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Collections;
using Sitecore.Data.Items;
using Sitecore.Data.Templates;

namespace Sitecore.Data.Managers
{
    public static class TemplateManager
    {
        public static Template GetTemplate([NotNull] ID id, [NotNull] Database database)
        {
            throw new NotImplementedException();
        }

        [NotNull]
        public static TemplateDictionary GetTemplates([NotNull] Database database)
        {
            throw new NotImplementedException();
        }

        [CanBeNull]
        public static Template GetTemplate([NotNull] Item item)
        {
            throw new NotImplementedException();
        }
    }
}
