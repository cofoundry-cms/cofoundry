using Cofoundry.Core.Json;
using Microsoft.AspNetCore.Builder;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Web
{
    /// <summary>
    /// Configures the default JsonSerializerSettings for Json.Net
    /// </summary>
    public class JsonConverterConfigurationStartupTask : IStartupTask
    {
        private readonly IJsonSerializerSettingsFactory _jsonSerializerSettingsFactory;

        public JsonConverterConfigurationStartupTask(
            IJsonSerializerSettingsFactory jsonSerializerSettingsFactory
            )
        {
            _jsonSerializerSettingsFactory = jsonSerializerSettingsFactory;
        }

        /// <summary>
        /// Get the json config in early in case it's required later on in the process.
        /// </summary>
        public int Ordering
        {
            get { return (int)StartupTaskOrdering.Early; }
        }

        public void Run(IApplicationBuilder app)
        {
            JsonConvert.DefaultSettings = _jsonSerializerSettingsFactory.Create;
        }
    }
}