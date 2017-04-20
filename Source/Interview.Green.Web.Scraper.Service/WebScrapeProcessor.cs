using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using Interview.Green.Web.Scraper.Interfaces;
using Interview.Green.Web.Scraper.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Interview.Green.Web.Scraper.Service
{
    /// <summary>
    /// Scrape
    /// </summary>
    public class WebScrapeProcessor : IJobProcessor
    {
        public const int ClientTimeOutInSeconds = 10; ////TODO: This should not be hardcoded and must be moved to congig
        public async Task<bool> StartAsync(IJobQueue job)
        {
            var webScrapeJob = (WebScrapeJob) job;

            using (HttpClient client = new HttpClient())
            {
                // Timeout if site doesn't respond
                client.Timeout = TimeSpan.FromSeconds(ClientTimeOutInSeconds);

                var html = await client.GetStringAsync(webScrapeJob.Url);
                // Console.WriteLine(html);

                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                webScrapeJob.Scrapes = new List<string>();
                if (!string.IsNullOrEmpty(webScrapeJob.Selector)) {
                    foreach (var selectedNode in doc.DocumentNode.QuerySelectorAll(webScrapeJob.Selector))
                    {
                        webScrapeJob.Scrapes.Add(selectedNode.InnerHtml);
                    }
                }
                return true;
            }
        }
    }
}