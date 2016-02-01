// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.SecurityModel;

namespace Sitecore.Jobs
{
    public class JobOptions
    {
        public JobOptions(string jobName, string category, string name, object backgroundJob, string runjob)
        {
        }


        public User ContextUser { get; set; }

        public TimeSpan AfterLife { get; set; }
    }
}