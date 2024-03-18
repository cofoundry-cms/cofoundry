﻿using Cofoundry.Core.Json;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Singleton creation of JsonSerializerSettings which includes
/// DynamicDataModelDefaultValueJsonConverter
/// </summary>
public class DynamicDataModelJsonSerializerSettingsCache
{
    private readonly JsonSerializerSettings _jsonSerializerSettings;
    private readonly INestedDataModelTypeRepository _nestedDataModelTypeRepository;
    private readonly ILogger<DynamicDataModelJsonSerializerSettingsCache> _logger;

    public DynamicDataModelJsonSerializerSettingsCache(
        ILogger<DynamicDataModelJsonSerializerSettingsCache> logger,
        IJsonSerializerSettingsFactory jsonSerializerSettingsFactory,
        INestedDataModelTypeRepository nestedDataModelTypeRepository
        )
    {
        _logger = logger;
        _nestedDataModelTypeRepository = nestedDataModelTypeRepository;

        var settings = jsonSerializerSettingsFactory.Create();
        settings.Converters.Add(new DynamicDataModelDefaultValueJsonConverter(jsonSerializerSettingsFactory.Create()));
        settings.Converters.Add(new NestedDataModelMultiTypeItemJsonConverter(_nestedDataModelTypeRepository));
        settings.Error = HandleDeserializationError;
        settings.ObjectCreationHandling = ObjectCreationHandling.Replace;

        _jsonSerializerSettings = settings;
    }

    public JsonSerializerSettings GetInstance()
    {
        return _jsonSerializerSettings;
    }

    /// <summary>
    /// The might be errors when reading a model where the model type 
    /// may have changed e.g. the property name is the same but the type
    /// changed. These errors shouldn't break the application.
    /// </summary>
    private void HandleDeserializationError(object? sender, Newtonsoft.Json.Serialization.ErrorEventArgs errorArgs)
    {
        _logger.LogWarning(errorArgs.ErrorContext.Error, "Error deserializing dynamic data model data");
        errorArgs.ErrorContext.Handled = true;
    }
}
