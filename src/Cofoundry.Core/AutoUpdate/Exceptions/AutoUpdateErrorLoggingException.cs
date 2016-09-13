using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.AutoUpdate
{
    /// <summary>
    /// A composite exception that is thrown when there is an error logging an auto-update exception
    /// to the database. This may occur specifically if there is a problem with the first script
    /// that creates the logging table.
    /// </summary>
    public class AutoUpdateErrorLoggingException : Exception
    {
        public AutoUpdateErrorLoggingException()
            : base()
        {
        }

        public AutoUpdateErrorLoggingException(Exception wrappedException, Exception loggingException)
            : base(CreateMessage(wrappedException, loggingException), wrappedException)
        {
            LoggingException = loggingException;
        }

        private static string CreateMessage(Exception wrappedException, Exception loggingException)
        {
            return $"Error logging AutoUpdate exception. Logging exception: {wrappedException?.Message}, Original exception = {wrappedException?.Message}";
        }

        public Exception LoggingException { get; private set; }
    }
}
