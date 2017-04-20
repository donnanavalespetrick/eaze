using Interview.Green.Web.Scraper.Interfaces;
using Interview.Green.Web.Scraper.Models;
using Interview.Green.Web.Scraper.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Interview.Green.Web.Scraper.Service.JobSchedulerService;

namespace Interview.Green.Web.Scrapper.Tests
{
    [TestClass]
    public class JobSchedulerServiceUnitTest
    {
        public class MockJobFactory : IJobProcessorFactory
        {
            public IJobProcessor GetJobProcessor(Type jobType)
            {
                return new MockJobProcessorService();
            }
        }

        public class MockJobProcessorService : IJobProcessor
        {
            public async Task<bool> StartAsync(IJobQueue job)
            {
                await Task.Delay(TimeSpan.FromSeconds(10));
                return true;
            }
        }

        [TestMethod]
        public async Task TestConcurrencyLimit()
        {
            JobHandler.jobFactory = new MockJobFactory();

            var jobSchedulerService = new JobSchedulerService();

            var jobs = new List<IJobQueue>();
            for (int i = 0; i < 3; i++)
            {
                var job = new WebScrapeJob();
                await jobSchedulerService.ScheduleJobAsync(job);
                jobs.Add(job);
                Console.WriteLine(job.RequestId);
            }

            Thread.Sleep(TimeSpan.FromSeconds(2));

            foreach (IJobQueue job in jobs)
            {
                var jobStatus = await jobSchedulerService.GetJobAsync(job.RequestId);
                Console.WriteLine("{0} {1}", job.RequestId, jobStatus.Status);
                if (jobs.IndexOf(job) == jobs.Count - 1)
                    Assert.AreEqual(JobStatus.QUEUED, jobStatus.Status);
                else
                    Assert.AreEqual(JobStatus.RUNNING, jobStatus.Status);
            }
        }
    }
}
