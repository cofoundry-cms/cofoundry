using Cofoundry.Core.Json;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
    /// <summary>
    /// Extends the MvcJsonOptions configuration adding Cofoundry specific
    /// settings, which mostly just brings in settings from IJsonSerializerSettingsFactory.
    /// </summary>
    public class CofoundryMvcJsonOptionsConfiguration : IMvcJsonOptionsConfiguration
    {
        private readonly IJsonSerializerSettingsFactory _jsonSerializerSettingsFactory;

        public CofoundryMvcJsonOptionsConfiguration(
            IJsonSerializerSettingsFactory jsonSerializerSettingsFactory
            )
        {
            _jsonSerializerSettingsFactory = jsonSerializerSettingsFactory;
        }

        public void Configure(MvcNewtonsoftJsonOptions options)
        {
            _jsonSerializerSettingsFactory.Configure(options.SerializerSettings);
        }
    }
}
