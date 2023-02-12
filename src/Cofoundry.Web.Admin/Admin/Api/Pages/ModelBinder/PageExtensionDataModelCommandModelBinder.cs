using Cofoundry.Domain.Internal;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace Cofoundry.Web.Admin;

/// <summary>
/// A custom model binder for commands like AddPageCommand which is
/// required to resolve extension data properties to concrete types.
/// </summary>
public class PageExtensionDataModelCommandModelBinder : IModelBinder
{
    private readonly IPageModelExtensionConfigurationRepository _pageModelExtensionConfigurationRepository;

    public PageExtensionDataModelCommandModelBinder(
        IPageModelExtensionConfigurationRepository pageModelExtensionConfigurationRepository
        )
    {
        _pageModelExtensionConfigurationRepository = pageModelExtensionConfigurationRepository;
    }

    public async Task BindModelAsync(ModelBindingContext bindingContext)
    {
        ArgumentNullException.ThrowIfNull(bindingContext);

        var jsonString = await ReadBodyAsString(bindingContext);

        var json = JObject.Parse(jsonString);
        var pageTemplateId = IntParser.ParseOrNull(json.GetValue("pageTemplateId", StringComparison.OrdinalIgnoreCase));
        JsonConverter dataModelConverter = null;
        IReadOnlyCollection<ExtensionRegistrationOptions> options = Array.Empty<ExtensionRegistrationOptions>();

        if (pageTemplateId.HasValue)
        {
            options = _pageModelExtensionConfigurationRepository.GetByTemplateId(pageTemplateId.Value);
        }

        dataModelConverter = new EntityExtensionDataModelDictionaryJsonConverter(options);

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
}
