using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.Json.Overridable
{
    /// <summary>
    /// Gets the default JsonSerializerSettings used by Cofoundry and assigned
    /// to the default Json serializer in asp.net MVC and web api.
    /// </summary>
    public class DefaultJsonSerializerSettingsFactory : IJsonSerializerSettingsFactory
    {
        /// <summary>
        /// Creates a new JsonSerializerSettings instance .
        /// </summary>
        public JsonSerializerSettings Create()
        {
            var settings = new JsonSerializerSettings();

            return Configure(settings);
        }

        /// <summary>
        /// Applies the json serializer settings to an existing settings instance.
        /// </summary>
        /// <param name="settings">An existing settings instance to apply updated settings to.</param>
        public virtual JsonSerializerSettings Configure(JsonSerializerSettings settings)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));

            // Force dates to include the miliseconds portion, which fixes issues with Angular date field validation.
            settings.DateFormatString = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.fffK";
            settings.Converters.Add(new StringEnumConverter());
            settings.Converters.Add(new HtmlStringJsonConverter());
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            return settings;
        }
    }
}
