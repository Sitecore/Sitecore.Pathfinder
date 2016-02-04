// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;

namespace Sitecore.Data
{
    public class ID
    {
        public ID([NotNull] string guid)
        {
            throw new NotImplementedException();
        }

        public ID(Guid guid)
        {
        }

        public static bool IsID(string id)
        {
            throw new NotImplementedException();
        }

        public static implicit operator ID(TemplateID templateID)
        {
            throw new NotImplementedException();
        }

        public Guid ToGuid()
        {
            throw new NotImplementedException();
        }
    }
}
