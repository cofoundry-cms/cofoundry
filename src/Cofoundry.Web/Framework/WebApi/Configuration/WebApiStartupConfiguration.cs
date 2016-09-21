using Cofoundry.Core.Json;
using Cofoundry.Web.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;

namespace Cofoundry.Web.WebApi
{
    public class WebApiStartupConfiguration : IWebApiStartupConfiguration
    {
        private readonly IJsonSerializerSettingsFactory _jsonSerializerSettingsFactory;

        public WebApiStartupConfiguration(
            IJsonSerializerSettingsFactory jsonSerializerSettingsFactory
            )
        {
            _jsonSerializerSettingsFactory = jsonSerializerSettingsFactory;
        }

        public void Configure(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            ConfigureJsonFormatter(config);

            config.Formatters.Add(new MultipartFormDataFormatter());

            config.Services.Add(typeof(IExceptionLogger), new WebApiExceptionLogger());
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