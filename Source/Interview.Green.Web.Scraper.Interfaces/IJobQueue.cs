using System;
using System.Collections.Generic;

namespace Interview.Green.Web.Scraper.Interfaces
{
    public interface IJobQueue
    {
        Guid RequestId { get; }
        DateTime RequestedAt { get; }
        JobStatus Status { get; set; }
    }
}
