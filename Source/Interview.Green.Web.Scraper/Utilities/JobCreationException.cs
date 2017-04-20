using System;

namespace Interview.Green.Web.Scrapper.Utilities
{
    public class JobCreationException : Exception
    {
        public JobCreationException()
        {
        }

        public JobCreationException(string message)
            : base(message)
        {
        }

        public JobCreationException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}