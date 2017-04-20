using Interview.Green.Web.Scraper.Interfaces;
using Interview.Green.Web.Scraper.Models;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;

namespace Interview.Green.Web.Scraper.Service
{
    /// <summary>
    /// Job scheduler service
    /// </summary>
    public class JobSchedulerService : IJobSchedulerService
    {
        private static IList<IJobQueue> jobs = new List<IJobQueue>();
        private static IScheduler scheduler;

        /// <summary>
        /// Handles job requests from scheduler
        /// </summary>
        public class JobHandler : IJob
        {
            public static IJobProcessorFactory jobFactory = new JobProcessorFactory();

            /// <summary>
            /// Execute job requests from scheduler
            /// </summary>
            /// <param name="context">Job execution context</param>
            public void Execute(IJobExecutionContext context)
            {
                var requestId = (Guid)context.JobDetail.JobDataMap["RequestId"];

                var job = jobs.First(x => x.RequestId == requestId);
                job.Status = JobStatus.RUNNING;

                var service = jobFactory.GetJobProcessor(job.GetType());
                job.Status = service.StartAsync(job).Result ? JobStatus.COMPLETED : JobStatus.FAILED;
            }
        }

        /// <summary>
        /// CTOR. Configure and start scheduler.
        /// </summary>
        static JobSchedulerService()
        {
            var properties = new NameValueCollection { { "quartz.threadPool.threadCount", "2" } }; //TODO: This should not be hardcoded and must be moved to config
            var schedulerFactory = new StdSchedulerFactory(properties);
            scheduler = schedulerFactory.GetScheduler();
            scheduler.Start();  
        }

        /// <summary>
        /// Schedule job.
        /// </summary>
        /// <param name="job">Job to schedule</param>
        public async Task ScheduleJobAsync(IJobQueue job)
        {
            jobs.Add(job);

            var jobDataMap = new JobDataMap();
            jobDataMap.Add("RequestId", job.RequestId);

            IJobDetail jobDetail = JobBuilder.Create<JobHandler>()
                .WithIdentity(job.RequestId.ToString(), "JobQueue")
                .UsingJobData(jobDataMap)
                .Build();

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity(job.RequestId.ToString(), "JobQueue")
                .StartNow()
                .Build();

            await Task.Run(() => scheduler.ScheduleJob(jobDetail, trigger));
        }

        /// <summary>
        /// Get jobs
        /// </summary>
        /// <returns>List of jobs</returns>
        public async Task<IList<IJobQueue>> GetJobsAsync()
        {
            return await Task.Run(() => jobs);
        }

        /// <summary>
        /// Get job
        /// </summary>
        /// <param name="requestId">Unique identifier for a job</param>
        /// <returns>Job or null if not found</returns>
        public async Task<IJobQueue> GetJobAsync(Guid requestId)
        {
            return await Task.Run(() => jobs.Single(x => x.RequestId == requestId));
        }

        /// <summary>
        /// Delete job. Queued and Running jobs cannot be deleted
        /// </summary>
        /// <param name="requestId">Unique identifier for a job</param>
        /// <returns>True if the job was deleted, false if not found</returns>
        public async Task<bool> DeleteJobAsync(Guid requestId)
        {
            var job = await this.GetJobAsync(requestId);
            if (job == null) return false;
            if (job.Status == JobStatus.QUEUED || job.Status == JobStatus.RUNNING) return false;
            return jobs.Remove(job);
        }
    }

    
}