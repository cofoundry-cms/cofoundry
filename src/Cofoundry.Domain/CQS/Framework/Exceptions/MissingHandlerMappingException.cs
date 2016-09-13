using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.CQS
{
    /// <summary>
    /// An exception for when a Handler cann not be found for an IQuery or ICommand. Typically
    /// this means a problem with handler registration.
    /// </summary>
    public class MissingHandlerMappingException : Exception
    {
        private const string errorMessage = "Could not locate a handler for type: '{0}'.";

        public MissingHandlerMappingException()
        {
        }

        public MissingHandlerMappingException(string message)
            : base(message)
        {
        }

        public MissingHandlerMappingException(Type t)
            : base(FormatMessage(t))
        {
        }

        private static string FormatMessage(Type t)
        {
            if (t == null) return string.Format(errorMessage, "NULL");
            return string.Format(errorMessage, t.FullName);
        }
    }
}
