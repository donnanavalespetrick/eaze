using Interview.Green.Web.Scraper.Interfaces;
using System;
using System.Collections.Generic;

namespace Interview.Green.Web.Scraper.Models
{
    public class WebScrapeJob : IJobQueue
    {
        public Guid RequestId { get; private set; }
        public DateTime RequestedAt { get; private set; }
        public JobStatus Status { get; set; }

        /// <summary>
        /// The url of the site to be scraped.
        /// </summary>
        public Uri Url { get; set; }
        /// <summary>
        /// The html/css selector to be scraped.
        /// </summary>
        public string Selector { get; set; }
        /// <summary>
        /// The result of the scrape.
        /// </summary>
        public IList<string> Scrapes { get; set; }

        public WebScrapeJob()
        {
            this.RequestId = Guid.NewGuid();
            this.RequestedAt = DateTime.UtcNow;
        }

        
    }
}
