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
        private readonly IJsonSerializerSettingsFactory _jsonSerializerSettingsFactory;

        public WebApiConfigurationStartupTask(
            IJsonSerializerSettingsFactory jsonSerializerSettingsFactory
            )
        {
            _jsonSerializerSettingsFactory = jsonSerializerSettingsFactory;
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
            config.MapHttpAttributeRoutes();

            ConfigureJsonFormatter(config);

            config.Formatters.Add(new MultipartFormDataFormatter());

            config.Services.Add(typeof(IExceptionLogger), new WebApiExceptionLogger());
            config.EnsureInitialized();
        }

        private void ConfigureJsonFormatter(HttpConfiguration config)
        {
            var jsonFormatter = config.Formatters.JsonFormatter;

            // make json default response
            jsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));

            jsonFormatter.SerializerSettings = _jsonSerializerSettingsFactory.Create();
            jsonFormatter.SerializerSettings.ContractResolver = new DeltaContractResolver();
        }
    }
}