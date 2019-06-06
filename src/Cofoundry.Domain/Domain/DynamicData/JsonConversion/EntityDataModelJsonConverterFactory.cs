using Cofoundry.Core.Json;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    public class EntityDataModelJsonConverterFactory : IEntityDataModelJsonConverterFactory
    {
        private readonly IJsonSerializerSettingsFactory _jsonSerializerSettingsFactory;
        private readonly INestedDataModelTypeRepository _nestedDataModelTypeRepository;

        public EntityDataModelJsonConverterFactory(
            IJsonSerializerSettingsFactory jsonSerializerSettingsFactory,
            INestedDataModelTypeRepository nestedDataModelTypeRepository
            )
        {
            _jsonSerializerSettingsFactory = jsonSerializerSettingsFactory;
            _nestedDataModelTypeRepository = nestedDataModelTypeRepository;
        }

        public JsonConverter Create(Type concreteDataModelType)
        {
            return new EntityDataModelJsonConverter(
                _jsonSerializerSettingsFactory,
                _nestedDataModelTypeRepository,
                concreteDataModelType
                );
        }
    }
}
