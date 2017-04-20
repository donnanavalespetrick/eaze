using Interview.Green.Web.Scraper.Interfaces;
using Interview.Green.Web.Scraper.Models;
using System;

namespace Interview.Green.Web.Scraper.Service
{
    public class JobProcessorFactory : IJobProcessorFactory
    {
        /// <summary>
        /// Identifies the appropriate job processor based on the type of job
        /// </summary>
        /// <param name="jobType">Job type</param>
        /// <returns>Created job processor</returns>
        public IJobProcessor GetJobProcessor(Type jobType)
        {
            if (jobType == typeof(WebScrapeJob))
                return new WebScrapeProcessor();
            return null;
        }
    }
}
