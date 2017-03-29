using Cofoundry.Core;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Service to get connection info about someone connecting to the site
    /// </summary>
    public class ClientConnectionService : IClientConnectionService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ClientConnectionService(
            IHttpContextAccessor httpContextAccessor
            )
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public ClientConnectionInfo GetConnectionInfo()
        {
            var context = _httpContextAccessor.HttpContext;

            var info = new ClientConnectionInfo();

            if (context != null && context.Request != null)
            {
                info.IPAddress = context?.Connection?.RemoteIpAddress?.ToString();
                info.UserAgent = context.Request?.Headers?.GetOrDefault("User-Agent");
            }

            return info;
        }
    }

}
