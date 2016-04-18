// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Jobs;

namespace Sitecore.Pathfinder.Tasks
{
    public class BackgroundJob
    {
        [Diagnostics.NotNull]
        private readonly Action _action;

        private BackgroundJob([NotNull] Action action)
        {
            _action = action;
        }

        [NotNull]
        public static string Run([NotNull] string jobName, [NotNull] string category, [NotNull] Action action)
        {
            var jobOptions = new JobOptions(jobName, category, Client.Site.Name, new BackgroundJob(action), "RunJob")
            {
                AfterLife = TimeSpan.FromMinutes(1),
                ContextUser = Context.User
            };

            var job = JobManager.Start(jobOptions);

            return job.Handle.ToString();
        }

        public void RunJob()
        {
            var job = Context.Job;
            if (job == null)
            {
                return;
            }

            try
            {
                _action();
            }
            catch (Exception ex)
            {
                job.Status.Failed = true;
                job.Status.Messages.Add(ex.ToString());
            }

            job.Status.State = JobState.Finished;
        }
    }
}
