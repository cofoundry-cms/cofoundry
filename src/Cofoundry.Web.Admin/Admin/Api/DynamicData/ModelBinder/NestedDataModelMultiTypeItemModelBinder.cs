using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Domain;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using Cofoundry.Core.Json;
using Cofoundry.Domain.Internal;

namespace Cofoundry.Web.Admin
{
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
            if (bindingContext == null) throw new ArgumentNullException(nameof(bindingContext));

            var settings = _dynamicDataModelJsonSerializerSettingsCache.GetInstance();

            var jsonString = await ReadBodyAsString(bindingContext);

            var result = JsonConvert.DeserializeObject(jsonString, bindingContext.ModelType, settings);

            bindingContext.Result = ModelBindingResult.Success(result);
        }

        private async Task<string> ReadBodyAsString(ModelBindingContext bindingContext)
        {
            string body;
            using (var reader = new StreamReader(bindingContext.ActionContext.HttpContext.Request.Body, Encoding.UTF8))
            {
                body = await reader.ReadToEndAsync();
            }

            return body;
        }
    }
}