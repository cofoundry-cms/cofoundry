using Cofoundry.Core.Json;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain.Internal
{
    public class DynamicDataModelDefaultValueJsonConverter : JsonConverter
    {
        private readonly JsonSerializerSettings _jsonSerializerSettings;

        public DynamicDataModelDefaultValueJsonConverter(
            JsonSerializerSettings copyOfJsonSerializerSettings
            )
        {
            copyOfJsonSerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            copyOfJsonSerializerSettings.DefaultValueHandling = DefaultValueHandling.Ignore;
            _jsonSerializerSettings = copyOfJsonSerializerSettings;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(DynamicDataModelDefaultValue).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var newSerializer = JsonSerializer.Create(_jsonSerializerSettings);
            newSerializer.Serialize(writer, value);
        }
    }
}
