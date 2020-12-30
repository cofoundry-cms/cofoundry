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

namespace Cofoundry.Web.Admin
{
    /// <summary>
    /// A custom model binder for commands like AddPageVersionBlockCommand which is required to
    /// resolve the child PageBlockDataModel
    /// </summary>
    public class NestedDataModelMultiTypeItemModelBinder : IModelBinder
    {
        private readonly IJsonSerializerSettingsFactory _jsonSerializerSettingsFactory;
        private readonly INestedDataModelTypeRepository _nestedDataModelTypeRepository;

        public NestedDataModelMultiTypeItemModelBinder(
            IJsonSerializerSettingsFactory jsonSerializerSettingsFactory,
            INestedDataModelTypeRepository nestedDataModelTypeRepository
            )
        {
            _jsonSerializerSettingsFactory = jsonSerializerSettingsFactory;
            _nestedDataModelTypeRepository = nestedDataModelTypeRepository;
        }

        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null) throw new ArgumentNullException(nameof(bindingContext));

            var settings = _jsonSerializerSettingsFactory.Create();
            settings.Converters.Add(new NestedDataModelMultiTypeItemJsonConverter(_jsonSerializerSettingsFactory, _nestedDataModelTypeRepository));

            var jsonString = await ReadBodyAsString(bindingContext);
            var newSerializer = JsonSerializer.Create(settings);

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