using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interview.Green.Web.Scraper.Interfaces
{
    public interface IJobSchedulerService
    {
        Task ScheduleJobAsync(IJobQueue job);
        Task<IList<IJobQueue>> GetJobsAsync();
        Task<IJobQueue> GetJobAsync(Guid requestId);
        Task<bool> DeleteJobAsync(Guid requestId);
    }
}