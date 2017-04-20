using System.Threading.Tasks;

namespace Interview.Green.Web.Scraper.Interfaces
{
    /// <summary>
    /// Job processor interface for different jobs. 
    /// Ex. WebScrapingJob, Other jobs
    /// </summary>
    public interface IJobProcessor
    {
        Task<bool> StartAsync(IJobQueue job);
    }
}
