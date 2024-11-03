***Note: Cofoundry currently uses JSON.NET and configures the default ASP.NET Core parser to use JSON.NET. We intend to move to the new System.Text.Json parser for Cofoundry at some point, and make any changes to the default parser opt-in.***

## Cofoundry default JSON settings

Cofoundry uses a customized set of JSON.NET `JsonSerializerSettings` including:

- Camel case property naming
- A slightly tweaked DateFormatString that fixes a parsing issue in AngularJS: `"yyyy'-'MM'-'dd'T'HH':'mm':'ss.fffK"`
- Additional converters such as `StringEnumConverter` and `HtmlStringJsonConverter`    

You can access these settings by requesting either of these classes from the DI resolver:

- `JsonSerializerSettings`: A singleton instance of the Cofoundry defaults
- `IJsonSerializerSettingsFactory`: A factory that can be used to create new instances of the Cofoundry default `JsonSerializerSettings` or can be used to apply the default configuration to an existing `JsonSerializerSettings` instance.

## ASP.NET & JsonConvert defaults

The Cofoundry `JsonSerializerSettings` are also applied as the default settings for both JSON.NET and ASP.NET.

Cofoundry uses these settings in isolation and so it is safe to change the JSON.NET or ASP.NET defaults if you need to without breaking Cofoundry. 

This can be done in your `Program.cs` file by re-applying the `AddNewtonsoftJson` configuration after Cofoundry has been added:

```csharp
builder.Services
    .AddMvc()
    .AddCofoundry(builder.Configuration)
    .AddNewtonsoftJson(o =>
    {
        // e.g. reset the contract resolver to use PascalCase.
        o.SerializerSettings.ContractResolver = new DefaultContractResolver();
        JsonConvert.DefaultSettings = () => o.SerializerSettings;
    });
```

## Enhancing the Cofoundry default JSON configuration

If you need to enhance the default Cofoundry `JsonSerializerSettings`, you will need to override `IJsonSerializerSettingsFactory` with your own implementation. 

It's important that your implementation inherits from our own `DefaultJsonSerializerSettingsFactory`, so that the base settings required for Cofoundry to function are preserved.

```csharp
using Cofoundry.Core.DependencyInjection;
using Cofoundry.Core.Json.Overridable;
using Newtonsoft.Json;

public class ExampleDependencyRegistration : IDependencyRegistration
{
    public void Register(IContainerRegister container)
    {
        // IJsonSerializerSettingsFactory should be registered as Singleton lifetime
        var overrideOptions = RegistrationOptions.Override();
        overrideOptions.Lifetime = InstanceLifetime.Singleton;

        container.Register<IJsonSerializerSettingsFactory, JsonSerializerSettingsFactory>(overrideOptions);
    }
}

public class JsonSerializerSettingsFactory : DefaultJsonSerializerSettingsFactory
{
    public override JsonSerializerSettings Configure(JsonSerializerSettings settings)
    {
        // Ensure default Cofoundry settings are applied
        base.Configure(settings);

        settings.Converters.Add(new ExampleConverter());

        return settings;
    }
}
```