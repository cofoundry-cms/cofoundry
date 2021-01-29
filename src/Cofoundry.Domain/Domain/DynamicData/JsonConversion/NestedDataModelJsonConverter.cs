using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Json.NET converter to convert a json representation of a concrete 
    /// nested data model type into a INestedDataModel property.
    /// </summary>
    /// <typeparam name="TDataModel">
    /// The concrete data model type to interpret the json model as.
    /// </typeparam>
    public class NestedDataModelJsonConverter<TDataModel> : JsonConverter where TDataModel : class, INestedDataModel
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(INestedDataModel).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var newSerializer = JsonSerializer.CreateDefault();
            return newSerializer.Deserialize<TDataModel>(reader);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
