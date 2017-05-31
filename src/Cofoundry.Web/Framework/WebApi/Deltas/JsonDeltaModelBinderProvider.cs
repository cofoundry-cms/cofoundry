using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace Cofoundry.Web
{
    /// <summary>
    /// Model binder provider to accompany JsonDeltaModelBinder.
    /// </summary>
    public class JsonDeltaModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
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
}