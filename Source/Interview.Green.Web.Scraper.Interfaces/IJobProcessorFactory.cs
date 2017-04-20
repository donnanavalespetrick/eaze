using System;

namespace Interview.Green.Web.Scraper.Interfaces
{
    public interface IJobProcessorFactory
    {
        IJobProcessor GetJobProcessor(Type jobType);
    }
}
