// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Security.Accounts;

namespace Sitecore.Jobs
{
    public class JobOptions
    {
        public JobOptions(string jobName, string category, string name, object backgroundJob, string runjob)
        {
        }

        public TimeSpan AfterLife
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public User ContextUser
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }
    }
}
