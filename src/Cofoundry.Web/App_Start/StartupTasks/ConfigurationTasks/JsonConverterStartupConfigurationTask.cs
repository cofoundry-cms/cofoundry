using Cofoundry.Core.Json;
using Microsoft.AspNetCore.Builder;
using Newtonsoft.Json;

namespace Cofoundry.Web;

/// <summary>
/// Configures the default JsonSerializerSettings for Json.Net using
/// the registered IJsonSerializerSettingsFactory.
/// </summary>
public class JsonConverterStartupConfigurationTask : IStartupConfigurationTask
{
    private readonly IJsonSerializerSettingsFactory _jsonSerializerSettingsFactory;

    public JsonConverterStartupConfigurationTask(
        IJsonSerializerSettingsFactory jsonSerializerSettingsFactory
        )
    {
        _jsonSerializerSettingsFactory = jsonSerializerSettingsFactory;
    }

    /// <summary>
    /// Get the json config in early in case it's required later on in the process.
    /// </summary>
    public int Ordering
    {
        get { return (int)StartupTaskOrdering.Early; }
    }

    public void Configure(IApplicationBuilder app)
    {
        JsonConvert.DefaultSettings = _jsonSerializerSettingsFactory.Create;
    }
}
