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
    /// A custom model binder for commands like AddPageVersionModuleCommand which is required to
    /// resolve the child PageModuleDataModel
    /// </summary>
    public class PageVersionModuleDataModelCommandModelBinder : IModelBinder
    {
        private readonly IPageModuleDataModelTypeFactory _moduleDataModelTypeFactory;

        public PageVersionModuleDataModelCommandModelBinder(
            IPageModuleDataModelTypeFactory moduleDataModelTypeFactory
            )
        {
            _moduleDataModelTypeFactory = moduleDataModelTypeFactory;
        }

        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null) throw new ArgumentNullException(nameof(bindingContext));
            var jsonString = await ReadBodyAsString(bindingContext);

            var json = JObject.Parse(jsonString);
            var pageModuleTypeIdProperty = json.GetValue("PageModuleTypeId", StringComparison.OrdinalIgnoreCase);

            JsonConverter dataModelConverter;
            if (pageModuleTypeIdProperty == null)
            {
                dataModelConverter = new NullPageModuleDataModelJsonConverter();
            }
            else
            {
                dataModelConverter = await GetModuleDataTypeConverterAsync(pageModuleTypeIdProperty.Value<int>());
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

        private async Task<JsonConverter> GetModuleDataTypeConverterAsync(int pageModuleTypeId)
        {
            var dataModelType = await _moduleDataModelTypeFactory.CreateByPageModuleTypeIdAsync(pageModuleTypeId);
            var converterType = typeof(PageModuleDataModelJsonConverter<>).MakeGenericType(dataModelType);

            return (JsonConverter)Activator.CreateInstance(converterType);
        }
    }
}