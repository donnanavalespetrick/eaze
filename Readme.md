
>Although the project name was kept as Web.Scraper, this WebApi is meant to handle different jobs. 
But for the purposes of this exercise, only the web scraping job has been implemented.
 
**How to test:**  
  While debugging locally, the endpoint will be http://localhost:10820/api/Job
  1. To request a job, send a POST request and include the following information in the RequestBody:
	  ```
		{
 		 "type": <The type of job you need done. NOTE: SCRAPE is the default job. Ex. "SCRAPE">,
		 "data":
		   {
			   //any information necessary for the job to be processed. For a web scraping job, supply the following:
	             "url": <The url of the website to be scraped. ex. "https://google.com/>",
			   "selector": <The css selector of the elements to be scraped. ex "span.importantSpan">
		   }
	    }

  2. The POST endpoint returns a Guid for the job that was requested. 
     This Guid can then be used to check the status and result of the job by sending a GET request /api/job/[Guid]
  
  3. A GET request /api/job/ with no Guid supplied will return the list of all the jobs (running, queued, completed)
  
  4. To delete a job, call DELETE /api/job/[Guid] - a queued or running job cannot be deleted.


