using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.Json
{
    /// <summary>
    /// Gets the default JsonSerializerSettings used by Cofoundry and assigned
    /// to the default Json serializer in asp.net MVC and web api.
    /// </summary>
    public class JsonSerializerSettingsFactory : IJsonSerializerSettingsFactory
    {
        public JsonSerializerSettings Create()
        {
            var settings = new JsonSerializerSettings();

            settings.Converters.Add(new StringEnumConverter());
            settings.Converters.Add(new HtmlStringJsonConverter());
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            return settings;
        }
    }
}
