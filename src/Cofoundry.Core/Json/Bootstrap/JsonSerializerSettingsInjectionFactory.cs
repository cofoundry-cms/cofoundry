using Cofoundry.Core.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Core.Json.Registration
{
    public class JsonSerializerSettingsInjectionFactory : IInjectionFactory<JsonSerializerSettings>
    {
        private readonly IJsonSerializerSettingsFactory _jsonSerializerSettingsFactory;

        public JsonSerializerSettingsInjectionFactory(
            IJsonSerializerSettingsFactory jsonSerializerSettingsFactory
            )
        {
            _jsonSerializerSettingsFactory = jsonSerializerSettingsFactory;
        }

        public JsonSerializerSettings Create()
        {
            return _jsonSerializerSettingsFactory.Create();
        }
    }
}
