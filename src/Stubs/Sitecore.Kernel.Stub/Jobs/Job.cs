// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;

namespace Sitecore.Jobs
{
    public class Job
    {
        [NotNull]
        public object Handle
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        [NotNull]
        public JobStatus Status
        {
            get { throw new NotImplementedException(); }
        }
    }
}
