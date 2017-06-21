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
    /// A custom model binder for commands like AddCustomEntityCommand which is required to
    /// resolve the child PageModuleDataModel
    /// </summary>
    public class CustomEntityDataModelCommandModelBinder : IModelBinder
    {
        private readonly ICustomEntityDefinitionRepository _customEntityDefinitionRepository;

        public CustomEntityDataModelCommandModelBinder(
            ICustomEntityDefinitionRepository customEntityDefinitionRepository
            )
        {
            _customEntityDefinitionRepository = customEntityDefinitionRepository;
        }

        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null) throw new ArgumentNullException(nameof(bindingContext));
            var jsonString = await ReadBodyAsString(bindingContext);

            var json = JObject.Parse(jsonString);
            var customEntityDefinitionCodeProperty = json.GetValue("CustomEntityDefinitionCode", StringComparison.OrdinalIgnoreCase);

            JsonConverter dataModelConverter;
            if (customEntityDefinitionCodeProperty == null)
            {
                dataModelConverter = new NullCustomEntityDataModelJsonConverter();
            }
            else
            {
                dataModelConverter = GetModuleDataTypeConverter(customEntityDefinitionCodeProperty.Value<string>());
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

        private JsonConverter GetModuleDataTypeConverter(string customEntityDefinitionCode)
        {
            var definition = _customEntityDefinitionRepository.GetByCode(customEntityDefinitionCode);
            var dataModelType = definition.GetDataModelType();
            var converterType = typeof(CustomEntityVersionDataModelJsonConverter<>).MakeGenericType(dataModelType);

            return (JsonConverter)Activator.CreateInstance(converterType);
        }
    }
}