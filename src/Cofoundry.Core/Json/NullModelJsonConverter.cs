using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Core.Json
{
    /// <summary>
    /// <para>
    /// Json.NET converter used to ignore parsing of a child
    /// object that is an interface type when the concrete type 
    /// cannot be determined. E.g. this is used when deserializing
    /// the model property of AddCustomEntityCommand when the 
    /// model is null or the provided custom entity definition 
    /// is not registered with the system.
    /// </para>
    /// <para>
    /// Without using this converter Json.NET will throw an 
    /// exception.
    /// </para>
    /// </summary>
    public class NullModelJsonConverter<T> : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(T).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            // need to read the object to prevent exception
            var jObject = JObject.Load(reader);
            return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
