using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Service to get connection info about someone connecting to the site
    /// </summary>
    public class ClientConnectionService : IClientConnectionService
    {
        public ClientConnectionInfo GetConnectionInfo()
        {
            var info = new ClientConnectionInfo();

            if (HttpContext.Current != null && HttpContext.Current.Request != null)
            {
                var request = HttpContext.Current.Request;

                info.IPAddress = GetIPAddress(request);
                info.UserAgent = request.UserAgent;
            }

            return info;
        }

        private static string GetIPAddress(HttpRequest request)
        {
            return request.UserHostAddress;
        }
    }

}
