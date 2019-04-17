using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Web
{
    /// <summary>
    /// An exception to be thrown from the AutoUpdateMiddleware when an error
    /// occurs in the auto update process. This wraps the real exception from
    /// the hosted service so it can be reported in the http response.
    /// </summary>
    public class AutoUpdateFailedException : Exception
    {
        const string DEFAULT_MESSAGE = "An error has occured while updating the system. See the logs for more details.";

        public AutoUpdateFailedException()
            : base(DEFAULT_MESSAGE)
        {

        }

        public AutoUpdateFailedException(Exception autoUpdateException)
            : base(FormatMessage(autoUpdateException), autoUpdateException)
        {

        }

        private static string FormatMessage(Exception autoUpdateException)
        {
            if (autoUpdateException == null) return DEFAULT_MESSAGE;

            return "Error updating the system: " + autoUpdateException.Message;
        }
    }
}
