using Cofoundry.Core;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Cofoundry.Web
{
    public class EnumBinder<T> : IModelBinder where T : struct
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            var parsedValue = EnumParser.ParseOrNull<T>(value.FirstValue);

            if (parsedValue.HasValue)
            {
                bindingContext.Result = ModelBindingResult.Success(value);
            }

            return Task.CompletedTask;
        }
    }
}