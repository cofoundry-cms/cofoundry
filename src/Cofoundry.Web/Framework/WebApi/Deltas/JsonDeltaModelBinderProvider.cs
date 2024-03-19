﻿using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace Cofoundry.Web;

/// <summary>
/// Model binder provider to accompany JsonDeltaModelBinder.
/// </summary>
public class JsonDeltaModelBinderProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var modelType = context.Metadata.ModelType;

        if (context.Metadata.IsComplexType
            && modelType.IsConstructedGenericType
            && modelType.GetGenericTypeDefinition() == typeof(IDelta<>)
            )
        {
            return new BinderTypeModelBinder(typeof(JsonDeltaModelBinder));
        }

        return null;
    }
}
