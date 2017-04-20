using Interview.Green.Web.Scraper.Controllers;
using Interview.Green.Web.Scraper.Interfaces;
using Interview.Green.Web.Scraper.Models;
using Interview.Green.Web.Scrapper.ViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Hosting;

namespace Interview.Green.Web.Scrapper.Tests
{
    #region Unit Tests
    [TestClass]
    public class JobControllerTests
    {
        [TestMethod]
        public void TestGetJob_JobFound()
        {
            var jobSchedulerService = new Mock<IJobSchedulerService>();

            var webScrapeJob = new WebScrapeJob();
            jobSchedulerService.Setup(jbs => jbs.GetJobAsync(It.IsAny<Guid>()))
               .Returns(Task.FromResult<IJobQueue>(webScrapeJob));

            var jobController = new JobController(jobSchedulerService.Object);
            jobController.Request = new HttpRequestMessage();
            jobController.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
            var controllerResponse = jobController.Get(webScrapeJob.RequestId);

            Assert.AreEqual(HttpStatusCode.OK, controllerResponse.Result.StatusCode);
            Assert.AreEqual(webScrapeJob, controllerResponse.Result.Content.ReadAsAsync<WebScrapeJob>().Result);
        }

        [TestMethod]
        public void TestGetJob_JobNotFound()
        {
            var jobSchedulerService = new Mock<IJobSchedulerService>();

            var webScrapeJob = new WebScrapeJob();
            jobSchedulerService.Setup(jbs => jbs.GetJobAsync(It.IsAny<Guid>()))
               .Returns(Task.FromResult<IJobQueue>(null));

            var jobController = new JobController(jobSchedulerService.Object);
            jobController.Request = new HttpRequestMessage();
            jobController.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
            var controllerResponse = jobController.Get(webScrapeJob.RequestId);

            Assert.AreEqual(HttpStatusCode.NotFound, controllerResponse.Result.StatusCode);
            Assert.AreEqual(null, controllerResponse.Result.Content);
        }


        [TestMethod]
        public void TestPostJob_Success()
        {
            var jobSchedulerService = new Mock<IJobSchedulerService>();
            jobSchedulerService.Setup(jbs => jbs.ScheduleJobAsync(It.IsAny<IJobQueue>()))
               .Returns(Task.FromResult(TaskStatus.RanToCompletion));

            var jobController = new JobController(jobSchedulerService.Object);
            jobController.Request = new HttpRequestMessage();
            jobController.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
            var jobData = new JobCreationRequest
                {
                    Type = JobType.SCRAPE,
                    Data = new Dictionary<string, string>()
                    {
                        { "url", @"https://google.com/"},
                        { "selector", "span"}
                    }
                };
            var controllerResponse = jobController.Post(jobData);

            Assert.AreEqual(HttpStatusCode.Created, controllerResponse.Result.StatusCode);
        }

        [TestMethod]
        public void TestPostJob_Failure_InsufficientRequestParameters()
        {
            var jobSchedulerService = new Mock<IJobSchedulerService>();

            var jobController = new JobController(jobSchedulerService.Object);
            jobController.Request = new HttpRequestMessage();
            jobController.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

            //Job request with missing information
            var jobData = new JobCreationRequest
            {
                Type = JobType.SCRAPE,
            };
            var controllerResponse = jobController.Post(jobData);

            Assert.AreEqual(HttpStatusCode.BadRequest, controllerResponse.Result.StatusCode);
        }

        [TestMethod]
        public void TestPostJob_Failure_InvalidUrl()
        {
            var jobSchedulerService = new Mock<IJobSchedulerService>();

            var jobController = new JobController(jobSchedulerService.Object);
            jobController.Request = new HttpRequestMessage();
            jobController.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

            //Job request with missing invalid Url
            var jobData = new JobCreationRequest
            {
                Type = JobType.SCRAPE,
                Data = new Dictionary<string, string>()
                    {
                        { "url", "sdsaddsa2234"},
                        { "selector", "span"}
                    }
            };

            var controllerResponse = jobController.Post(jobData);

            Assert.AreEqual(HttpStatusCode.BadRequest, controllerResponse.Result.StatusCode);
        }

        [TestMethod]
        public void TestPostJob_Failure_InvalidJobType()
        {
            var jobSchedulerService = new Mock<IJobSchedulerService>();
            var jobController = new JobController(jobSchedulerService.Object);
            jobController.Request = new HttpRequestMessage();
            jobController.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

            //Job request with other jobtype
            var jobData = new JobCreationRequest
            {
                Type = JobType.OTHERJOBS,
                Data = new Dictionary<string, string>()
                    {
                        { "url", @"sdsad://dsa2234.com/"},
                        { "selector", "span"}
                    }
            };

            var controllerResponse = jobController.Post(jobData);

            Assert.AreEqual(HttpStatusCode.BadRequest, controllerResponse.Result.StatusCode);
        }
    }
    #endregion



    [TestClass]
    public class JobControllerIntegrationTests
    {
        [TestMethod]
        public async Task TestPostAndGet()
        {
            HttpConfiguration config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{id}", new { id = RouteParameter.Optional });

            HttpServer server = new HttpServer(config);
            using (HttpMessageInvoker client = new HttpMessageInvoker(server))
            {
                var data = new JobCreationRequest
                {
                    Type = JobType.SCRAPE,
                    Data = new Dictionary<string, string>() {
                        { "url", @"https://www.eaze.com/"},
                        { "selector", "footer a"}
                    }
                };

                var postRequest = new HttpRequestMessage
                {
                    RequestUri = new Uri("http://localhost:10820/api/Job"),
                    Method = HttpMethod.Post,
                    Content = new ObjectContent<JobCreationRequest>(data, new System.Net.Http.Formatting.JsonMediaTypeFormatter())
                };

                postRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                Guid requestId;
                using (HttpResponseMessage postResponse = client.SendAsync(postRequest, CancellationToken.None).Result)
                {
                    requestId = postResponse.Content.ReadAsAsync<Guid>().Result;
                    Assert.AreEqual(HttpStatusCode.Created, postResponse.StatusCode);
                }

                var getRequest = new HttpRequestMessage
                {
                    RequestUri = new Uri(string.Format("http://localhost:10820/api/Job/{0}", requestId)),
                    Method = HttpMethod.Get,
                };
                getRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //job is still running
                using (HttpResponseMessage getResponse1 = client.SendAsync(getRequest, CancellationToken.None).Result)
                {
                    var job = getResponse1.Content.ReadAsAsync<WebScrapeJob>().Result;
                    Assert.AreEqual(JobStatus.RUNNING, job.Status);
                    Assert.AreEqual(HttpStatusCode.OK, getResponse1.StatusCode);
                }

                Thread.Sleep(11000);

                //job has completed
                using (HttpResponseMessage getResponse2 = await client.SendAsync(getRequest, CancellationToken.None))
                {
                    var job = getResponse2.Content.ReadAsAsync<WebScrapeJob>().Result;
                    Assert.AreEqual(JobStatus.COMPLETED, job.Status);
                    Assert.AreEqual(HttpStatusCode.OK, getResponse2.StatusCode);
                    Assert.AreEqual(6, job.Scrapes.Count);
                    Assert.IsTrue(job.Scrapes.Contains("Privacy"));
                }
            }
        }
    }

}
