﻿using Cofoundry.Domain.Internal;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System.Text;

namespace Cofoundry.Web.Admin;

/// <summary>
/// A custom model binder for commands like AddPageVersionBlockCommand which is required to
/// resolve the child PageBlockDataModel
/// </summary>
public class NestedDataModelMultiTypeItemModelBinder : IModelBinder
{
    private readonly DynamicDataModelJsonSerializerSettingsCache _dynamicDataModelJsonSerializerSettingsCache;

    public NestedDataModelMultiTypeItemModelBinder(
        DynamicDataModelJsonSerializerSettingsCache dynamicDataModelJsonSerializerSettingsCache
        )
    {
        _dynamicDataModelJsonSerializerSettingsCache = dynamicDataModelJsonSerializerSettingsCache;
    }

    public async Task BindModelAsync(ModelBindingContext bindingContext)
    {
        ArgumentNullException.ThrowIfNull(bindingContext);

        var settings = _dynamicDataModelJsonSerializerSettingsCache.GetInstance();
        var jsonString = await ReadBodyAsString(bindingContext);
        var result = JsonConvert.DeserializeObject(jsonString, bindingContext.ModelType, settings);

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
