using Cofoundry.Core.Json;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// JsonConverter used for deserializing IEntityDataModel
    /// instances such as custom entity data models and page
    /// block data models.
    /// </summary>
    public class EntityDataModelJsonConverter : JsonConverter
    {
        private readonly IJsonSerializerSettingsFactory _jsonSerializerSettingsFactory;
        private readonly INestedDataModelTypeRepository _nestedDataModelTypeRepository;
        private readonly Type _dataModelType;

        public EntityDataModelJsonConverter(
            IJsonSerializerSettingsFactory jsonSerializerSettingsFactory,
            INestedDataModelTypeRepository nestedDataModelTypeRepository,
            Type dataModelType
            )
        {
            _jsonSerializerSettingsFactory = jsonSerializerSettingsFactory;
            _nestedDataModelTypeRepository = nestedDataModelTypeRepository;

            if (!dataModelType.IsClass)
            {
                throw new ArgumentException($"The data model type should be a concrete type", nameof(dataModelType));
            }

            if (!typeof(IEntityDataModel).IsAssignableFrom(dataModelType))
            {
                throw new ArgumentException($"The data model type should implement {nameof(IEntityDataModel)}", nameof(dataModelType));
            }

            _dataModelType = dataModelType;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(IEntityDataModel).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var settings = _jsonSerializerSettingsFactory.Create();
            settings.Converters.Add(new NestedDataModelMultiTypeItemJsonConverter(_jsonSerializerSettingsFactory, _nestedDataModelTypeRepository));

            var newSerializer = JsonSerializer.Create(settings);
            return newSerializer.Deserialize(reader, _dataModelType);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
