using Interview.Green.Web.Scraper.Interfaces;
using System.Collections.Generic;

namespace Interview.Green.Web.Scrapper.ViewModel
{
    public class JobCreationRequest
    {
        public JobType Type { get; set; }
        public IDictionary<string, string> Data { get; set; }
    }
}