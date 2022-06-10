using Cofoundry.Core.Json;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace Cofoundry.Web.Admin;

/// <summary>
/// A custom model binder for commands like AddCustomEntityCommand which is
/// required to resolve the child model property to a concrete type.
/// </summary>
public class CustomEntityDataModelCommandModelBinder : IModelBinder
{
    private readonly ICustomEntityDefinitionRepository _customEntityDefinitionRepository;
    private readonly IEntityDataModelJsonConverterFactory _entityDataModelJsonConverterFactory;

    public CustomEntityDataModelCommandModelBinder(
        ICustomEntityDefinitionRepository customEntityDefinitionRepository,
        IEntityDataModelJsonConverterFactory entityDataModelJsonConverterFactory
        )
    {
        _customEntityDefinitionRepository = customEntityDefinitionRepository;
        _entityDataModelJsonConverterFactory = entityDataModelJsonConverterFactory;
    }

    public async Task BindModelAsync(ModelBindingContext bindingContext)
    {
        ArgumentNullException.ThrowIfNull(bindingContext);

        var jsonString = await ReadBodyAsString(bindingContext);

        var json = JObject.Parse(jsonString);
        var customEntityDefinitionCodeProperty = json.GetValue("CustomEntityDefinitionCode", StringComparison.OrdinalIgnoreCase);
        var dataModelConverter = GetDataTypeConverter(customEntityDefinitionCodeProperty?.Value<string>());

        if (dataModelConverter == null)
        {
            dataModelConverter = new NullModelJsonConverter<ICustomEntityDataModel>();
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

    private JsonConverter GetDataTypeConverter(string customEntityDefinitionCode)
    {
        // If there's no code then the model probably wasn't supplied and should be
        // considered null which will cause a validation error
        if (string.IsNullOrWhiteSpace(customEntityDefinitionCode)) return null;

        // If there is a code but it's not registered in the system, then thats exeptional and we should throw
        var definition = _customEntityDefinitionRepository.GetRequiredByCode(customEntityDefinitionCode);

        var dataModelType = definition.GetDataModelType();
        var converterType = _entityDataModelJsonConverterFactory.Create(dataModelType);

        return converterType;
    }
}
