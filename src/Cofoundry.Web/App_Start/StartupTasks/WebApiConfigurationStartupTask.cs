using Cofoundry.Core.Json;
using Cofoundry.Web.WebApi;
using Newtonsoft.Json;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;

namespace Cofoundry.Web
{
    /// <summary>
    /// Configures WebApi for all the essential parts of Cofoundry.
    /// </summary>
    public class WebApiConfigurationStartupTask : IStartupTask
    {
        private readonly IWebApiStartupConfiguration _webApiStartupConfiguration;

        public WebApiConfigurationStartupTask(
            IWebApiStartupConfiguration webApiStartupConfiguration
            )
        {
            _webApiStartupConfiguration = webApiStartupConfiguration;
        }

        /// <summary>
        /// Early because web api attribute routes should have priority
        /// </summary>
        public int Ordering
        {
            get { return (int)StartupTaskOrdering.Early; }
        }

        public void Run(IAppBuilder app)
        {
            GlobalConfiguration.Configure(Register);
        }

        private void Register(HttpConfiguration config)
        {
            _webApiStartupConfiguration.Configure(config);
            config.EnsureInitialized();
        }
    }
}