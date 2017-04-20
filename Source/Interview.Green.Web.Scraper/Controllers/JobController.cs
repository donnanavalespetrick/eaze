using Interview.Green.Web.Scraper.Interfaces;
using Interview.Green.Web.Scraper.Models;
using Interview.Green.Web.Scraper.Service;
using Interview.Green.Web.Scrapper.Utilities;
using Interview.Green.Web.Scrapper.ViewModel;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Interview.Green.Web.Scraper.Controllers
{
    public class JobController : ApiController
    {
        private IJobSchedulerService jobSchedulerService;

        public JobController()
        {
            jobSchedulerService = new JobSchedulerService();
        }

        /// <summary>
        /// CTOR - Unit Test DI
        /// </summary>
        /// <param name="jobSchedulerService"></param>
        internal JobController(IJobSchedulerService jobSchedulerService)
        {
            this.jobSchedulerService = jobSchedulerService;
        }

        // GET: api/job - gets all the jobs
        public async Task<HttpResponseMessage> Get()
        {
            return Request.CreateResponse(HttpStatusCode.OK, await jobSchedulerService.GetJobsAsync());
        }

        // GET: api/job/{some Guid} - get a specific job
        public async Task<HttpResponseMessage> Get(Guid id)
        {
            var result = await this.jobSchedulerService.GetJobAsync(id);
            if (result == null)
                return Request.CreateResponse(HttpStatusCode.NotFound);
            else
                return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        /// <summary>
        /// Schedules a job
        /// POST: api/job/
        /// </summary>
        /// <param name="createJobRequest">Contains request data such as what type of job was requested (ex Web Scrape),
        /// and other info necessary for the requested job- Ex if type is Webscrape, Url must be provided</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> Post([FromBody] JobCreationRequest createJobRequest)
        {
            try
            {
                var job = JobCreationFactory(createJobRequest);
                await jobSchedulerService.ScheduleJobAsync(job);
                Console.WriteLine(String.Format("Created {0}", job.RequestId));
                return Request.CreateResponse(HttpStatusCode.Created, job.RequestId);
            }
            catch (JobCreationException ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new HttpError(ex.Message));
            }
            catch (Exception ex) {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new HttpError(ex.Message));
            }
        }


        /// <summary>
        /// Deletes a specific job. Queued and Running jobs cannot be deleted.
        /// DELETE: api/job/{some Guid}
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> Delete(Guid id)
        {
            if (await this.jobSchedulerService.DeleteJobAsync(id))
                return Request.CreateResponse(HttpStatusCode.OK);
            else
                return Request.CreateResponse(HttpStatusCode.Conflict);
        }


        #region Private and Helper Methods
        /// <summary>
        /// Returns an IJobQueue specific to the job type sent in the request
        /// </summary>
        /// <param name="request">contains JobType</param>
        /// <returns></returns>
        private IJobQueue JobCreationFactory(JobCreationRequest request)
        {
            //NOTE:Only SCRAPE job has been implemented. Can be changed to a switch for more jobs
            //This factory can definitetly be enhanced.
            //IJobQueue implementations can also be modified to have a constructor that accepts a job request, 
            //and be reponsible for their own validation.

            if (request.Type == JobType.SCRAPE)
            {
                if (request.Data != null)
                {
                    var job = new WebScrapeJob();
                    Uri uri;
                    var uriCreated = Uri.TryCreate(request.Data["url"], UriKind.Absolute, out uri);
                    if (uriCreated)
                        job.Url = uri;
                    else
                        throw new JobCreationException("Url request parameter invalid.");
                    job.Selector = request.Data["selector"];
                    return job;
                }
                else
                    throw new JobCreationException("Insufficient request parameters.");
            }

            throw new JobCreationException("Type request parameter not supported.");
        }
        #endregion
    }
}