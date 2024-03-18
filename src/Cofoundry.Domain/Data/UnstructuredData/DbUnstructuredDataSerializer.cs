﻿using Cofoundry.Domain.Internal;
using Newtonsoft.Json;
using System.Reflection;

namespace Cofoundry.Domain.Data.Internal;

/// <summary>
/// Handles serialization for unstructured data stored in the db, e.g.
/// Page Module data
/// </summary>
public class DbUnstructuredDataSerializer : IDbUnstructuredDataSerializer
{
    private readonly DynamicDataModelJsonSerializerSettingsCache _dynamicDataModelJsonSerializerSettingsCache;

    public DbUnstructuredDataSerializer(
        DynamicDataModelJsonSerializerSettingsCache dynamicDataModelJsonSerializerSettingsCache
        )
    {
        _dynamicDataModelJsonSerializerSettingsCache = dynamicDataModelJsonSerializerSettingsCache;
    }

    public object? Deserialize(string? serialized, Type type)
    {
        if (string.IsNullOrEmpty(serialized))
        {
            if (type.GetTypeInfo().IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }

        return JsonConvert.DeserializeObject(serialized, type, GetDeserializerSettings());
    }

    public T? Deserialize<T>(string? serialized)
    {
        if (string.IsNullOrEmpty(serialized)) return default;

        return JsonConvert.DeserializeObject<T>(serialized, GetDeserializerSettings());
    }

    public string? Serialize(object? toSerialize)
    {
        var s = JsonConvert.SerializeObject(toSerialize);

        return s;
    }

    private JsonSerializerSettings GetDeserializerSettings()
    {
        return _dynamicDataModelJsonSerializerSettingsCache.GetInstance();
    }
}
