using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Domain;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using Cofoundry.Core.Json;
using Cofoundry.Core;

namespace Cofoundry.Web.Admin
{
    /// <summary>
    /// A custom model binder for commands like AddPageVersionBlockCommand which is required to
    /// resolve the child PageBlockDataModel
    /// </summary>
    public class PageVersionBlockDataModelCommandModelBinder : IModelBinder
    {
        private readonly IPageBlockTypeDataModelTypeFactory _pageBlockTypeDataModelTypeFactory;
        private readonly IEntityDataModelJsonConverterFactory _entityDataModelJsonConverterFactory;

        public PageVersionBlockDataModelCommandModelBinder(
            IPageBlockTypeDataModelTypeFactory pageBlockTypeDataModelTypeFactory,
            IEntityDataModelJsonConverterFactory entityDataModelJsonConverterFactory
            )
        {
            _pageBlockTypeDataModelTypeFactory = pageBlockTypeDataModelTypeFactory;
            _entityDataModelJsonConverterFactory = entityDataModelJsonConverterFactory;
        }

        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null) throw new ArgumentNullException(nameof(bindingContext));
            var jsonString = await ReadBodyAsString(bindingContext);

            var json = JObject.Parse(jsonString);
            var pageBlockTypeIdProperty = json.GetValue("PageBlockTypeId", StringComparison.OrdinalIgnoreCase);
            var dataModelConverter = await GetBlockDataTypeConverterAsync(pageBlockTypeIdProperty?.Value<int>());

            if (dataModelConverter == null)
            {
                dataModelConverter = new NullModelJsonConverter<IPageBlockTypeDataModel>();
            }
            
            var result = JsonConvert.DeserializeObject(jsonString, bindingContext.ModelType, dataModelConverter);
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

        private async Task<JsonConverter> GetBlockDataTypeConverterAsync(int? pageBlockTypeId)
        {
            // If there's no id then the model probably wasn't supplied and should be
            // considered null which will cause a validation error
            if (!pageBlockTypeId.HasValue || pageBlockTypeId < 1) return null;

            var dataBlockType = await _pageBlockTypeDataModelTypeFactory.CreateByPageBlockTypeIdAsync(pageBlockTypeId.Value);
            EntityNotFoundException.ThrowIfNull(dataBlockType, pageBlockTypeId);

            var converter = _entityDataModelJsonConverterFactory.Create(dataBlockType);

            return converter;
        }
    }
}