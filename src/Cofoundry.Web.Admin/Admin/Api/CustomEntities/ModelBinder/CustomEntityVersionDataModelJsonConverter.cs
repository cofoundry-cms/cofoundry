using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Domain;
using System.Reflection;

namespace Cofoundry.Web.Admin
{
    public class CustomEntityVersionDataModelJsonConverter<T> : JsonConverter where T : ICustomEntityVersionDataModel
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(ICustomEntityVersionDataModel).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var newSerializer = JsonSerializer.CreateDefault();
            return newSerializer.Deserialize<T>(reader);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}