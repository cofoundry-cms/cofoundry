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

namespace Cofoundry.Web.Admin
{
    /// <summary>
    /// A custom model binder for commands like AddPageVersionBlockCommand which is required to
    /// resolve the child PageBlockDataModel
    /// </summary>
    public class PageVersionBlockDataModelCommandModelBinder : IModelBinder
    {
        private readonly IPageBlockTypeDataModelTypeFactory _pageBlockTypeDataModelTypeFactory;

        public PageVersionBlockDataModelCommandModelBinder(
            IPageBlockTypeDataModelTypeFactory pageBlockTypeDataModelTypeFactory
            )
        {
            _pageBlockTypeDataModelTypeFactory = pageBlockTypeDataModelTypeFactory;
        }

        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null) throw new ArgumentNullException(nameof(bindingContext));
            var jsonString = await ReadBodyAsString(bindingContext);

            var json = JObject.Parse(jsonString);
            var pageBlockTypeIdProperty = json.GetValue("PageBlockTypeId", StringComparison.OrdinalIgnoreCase);

            JsonConverter dataModelConverter;
            if (pageBlockTypeIdProperty == null)
            {
                dataModelConverter = new NullPageBlockDataModelJsonConverter();
            }
            else
            {
                dataModelConverter = await GetBlockDataTypeConverterAsync(pageBlockTypeIdProperty.Value<int>());
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

        private async Task<JsonConverter> GetBlockDataTypeConverterAsync(int pageBlockTypeId)
        {
            var dataBlockType = await _pageBlockTypeDataModelTypeFactory.CreateByPageBlockTypeIdAsync(pageBlockTypeId);
            var converterType = typeof(PageBlockDataModelJsonConverter<>).MakeGenericType(dataBlockType);

            return (JsonConverter)Activator.CreateInstance(converterType);
        }
    }
}