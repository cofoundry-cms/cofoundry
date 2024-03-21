﻿using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace Cofoundry.Web;

/// <summary>
/// A model binder that binds a json payload containing a partial
/// model update (PATCH) to a Delta object that can applied to
/// an existing model futher down the pipeline.
/// </summary>
public class JsonDeltaModelBinder : IModelBinder
{
    private readonly JsonSerializerSettings _jsonSerializerSettings;

    public JsonDeltaModelBinder(JsonSerializerSettings jsonSerializerSettings)
    {
        _jsonSerializerSettings = jsonSerializerSettings;
    }

    public async Task BindModelAsync(ModelBindingContext bindingContext)
    {
        ArgumentNullException.ThrowIfNull(bindingContext);

        var jsonString = await ReadBodyAsString(bindingContext);

        var genericArguments = bindingContext
            .ModelType
            .GetTypeInfo()
            .GetGenericArguments();

        if (genericArguments.Length != 1)
        {
            throw new InvalidOperationException($"JsonDeltaModelBinder can only act on a type of IDelta<TModel>. Incorrect number of generic arguments found on type '{bindingContext.ModelType.FullName}' ({genericArguments.Length})");
        }

        var deltaType = typeof(JsonDelta<>).MakeGenericType(genericArguments.Single());
        var result = Activator.CreateInstance(deltaType, new object[] { jsonString, _jsonSerializerSettings });

        bindingContext.Result = ModelBindingResult.Success(result);
    }

    private static async Task<string> ReadBodyAsString(ModelBindingContext bindingContext)
    {
        string body;
        using (var reader = new StreamReader(bindingContext.ActionContext.HttpContext.Request.Body, Encoding.UTF8))
        {
            body = await reader.ReadToEndAsync();
        }

        return body;
    }
}
