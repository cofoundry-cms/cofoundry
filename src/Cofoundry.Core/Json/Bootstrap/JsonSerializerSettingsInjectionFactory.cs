using Cofoundry.Core.DependencyInjection;
using Newtonsoft.Json;

namespace Cofoundry.Core.Json.Registration;

public class JsonSerializerSettingsInjectionFactory : IInjectionFactory<JsonSerializerSettings>
{
    private readonly IJsonSerializerSettingsFactory _jsonSerializerSettingsFactory;

    public JsonSerializerSettingsInjectionFactory(
        IJsonSerializerSettingsFactory jsonSerializerSettingsFactory
        )
    {
        _jsonSerializerSettingsFactory = jsonSerializerSettingsFactory;
    }

    public JsonSerializerSettings Create()
    {
        return _jsonSerializerSettingsFactory.Create();
    }
}
