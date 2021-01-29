using Cofoundry.Core.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// JsonConverter for converting the NestedDataModelMultiTypeItem
    /// objects used in the NestedDataModelMultiTypeCollection value.
    /// </summary>
    public class NestedDataModelMultiTypeItemJsonConverter : JsonConverter
    {
        private readonly INestedDataModelTypeRepository _nestedDataModelTypeRepository;

        public NestedDataModelMultiTypeItemJsonConverter(
            INestedDataModelTypeRepository nestedDataModelTypeRepository
            )
        {
            _nestedDataModelTypeRepository = nestedDataModelTypeRepository;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(NestedDataModelMultiTypeItem).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var json = JObject.Load(reader);
            var result = new NestedDataModelMultiTypeItem();

            var typeName = json.GetValue(nameof(result.TypeName), StringComparison.OrdinalIgnoreCase);
            var nestedDataModelType = _nestedDataModelTypeRepository.GetByName(typeName.Value<string>());

            result.TypeName = nestedDataModelType.Name;

            var modelData = json.GetValue(nameof(result.Model), StringComparison.OrdinalIgnoreCase);
            if (modelData == null) return result;

            using (var dataModelReader = modelData.CreateReader())
            {
                result.Model = (INestedDataModel)serializer.Deserialize(dataModelReader, nestedDataModelType);
            }

            return result;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
