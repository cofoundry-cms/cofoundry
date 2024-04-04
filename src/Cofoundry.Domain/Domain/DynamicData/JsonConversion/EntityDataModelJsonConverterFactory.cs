﻿using Cofoundry.Domain.Internal;
using Newtonsoft.Json;

namespace Cofoundry.Domain;

public class EntityDataModelJsonConverterFactory : IEntityDataModelJsonConverterFactory
{
    private readonly DynamicDataModelJsonSerializerSettingsCache _dynamicDataModelJsonSerializerSettingsCache;

    public EntityDataModelJsonConverterFactory(
        DynamicDataModelJsonSerializerSettingsCache dynamicDataModelJsonSerializerSettingsCache
        )
    {
        _dynamicDataModelJsonSerializerSettingsCache = dynamicDataModelJsonSerializerSettingsCache;
    }

    public JsonConverter Create(Type dataModelType)
    {
        return new EntityDataModelJsonConverter(
            _dynamicDataModelJsonSerializerSettingsCache,
            dataModelType
            );
    }
}
