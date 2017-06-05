using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Contains connection information about a clients connection
    /// to the application e.g. IPAddress and the UserAgent string.
    /// </summary>
    public class ClientConnectionInfo
    {
        /// <summary>
        /// The IP Address of the client connected to the application.
        /// </summary>
        public string IPAddress { get; set; }

        /// <summary>
        /// The connecting client's user agent string (if connecting via a web browser).
        /// </summary>
        public string UserAgent { get; set; }
    }
}
