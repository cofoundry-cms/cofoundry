using Cofoundry.Core.Json;
using Cofoundry.Domain.Internal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    public class EntityDataModelJsonConverterFactory : IEntityDataModelJsonConverterFactory
    {
        private readonly DynamicDataModelJsonSerializerSettingsCache _dynamicDataModelJsonSerializerSettingsCache;

        public EntityDataModelJsonConverterFactory(
            DynamicDataModelJsonSerializerSettingsCache dynamicDataModelJsonSerializerSettingsCache
            )
        {
            _dynamicDataModelJsonSerializerSettingsCache = dynamicDataModelJsonSerializerSettingsCache;
        }

        public JsonConverter Create(Type concreteDataModelType)
        {
            return new EntityDataModelJsonConverter(
                _dynamicDataModelJsonSerializerSettingsCache,
                concreteDataModelType
                );
        }
    }
}
