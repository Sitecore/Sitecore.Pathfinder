// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Specialized;

namespace Sitecore.Jobs
{
    public class JobStatus
    {
        public bool Failed
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        [NotNull, ItemNotNull]
        public StringCollection Messages
        {
            get { throw new NotImplementedException(); }
        }

        public JobState State
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }
    }
}
