***Backend _.Net_ Interview Coding Exercise***

> Using the 4.5 .Net Solution provided, build a solution to solve the problem.
 
**Problem:**
  1. We need an API endpoint that has the ability to post a new Web site job.
  2. The Api will process this job and return a id for lookup and status purpose.
  3. Currently they only job type needed is the web site scraping job.
  4. This endpoint will be hit very heavily, so it has been asked that we implement a job scheduler.
  5. All new requests are logged and an ID is given back as a response.
  6. The request ID can be used to check the current status of the job running and return back the   results of the job.
  7. Website scraping job is a simple job that does the following:
    * Makes a request to website and gathers its response.
    * If items to scrape were requested the next step should be to process the response and find the items.
    * Store the result of the job so it can be retrieved later by ID.

**Solution:**
  1. Interview.Green.Web.Scraper your endpoint during this exercise.

**Hints:**
  1. Look at using Quartz for scheduling.
  2. Be sure to write unit tests for different cases...
  3. Async everything...
  4. The solution has been "mocked up" but don't feel this is how it needs to be implemented.
  5. Concurrency with multiple jobs running.

**Bonus:**
  1. Solve this issue without using a database.
  2. Don't use any third party web scraping frameworks.
  3. Think how this API will be consumed and what you might suggest to improve this.
  4. Documentation & Local repo.
