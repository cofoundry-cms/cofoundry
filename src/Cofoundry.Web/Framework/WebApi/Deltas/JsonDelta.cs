﻿using Newtonsoft.Json;

namespace Cofoundry.Web;

/// <summary>
/// Represents a partial update of a model, intended for use in
/// a web api http patch operation with a json payload.
/// </summary>
public class JsonDelta<TModel>
    : IDelta<TModel>
    where TModel : notnull
{
    private readonly string _jsonData;
    private readonly JsonSerializerSettings _jsonSerializerSettings;

    public JsonDelta(string jsonData, JsonSerializerSettings jsonSerializerSettings)
    {
        _jsonData = jsonData;
        _jsonSerializerSettings = jsonSerializerSettings;
    }

    /// <summary>
    /// Updates an existing model with the data from the delta.
    /// </summary>
    /// <param name="model">Model to update.</param>
    public void Patch(TModel model)
    {
        JsonConvert.PopulateObject(_jsonData, model, _jsonSerializerSettings);
    }
}
