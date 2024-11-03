*This section deals with the Cofoundry configuration framework, if you're looking for a list of configuration settings you can find these in [our reference section](/references/common-config-settings).*

## Overview

Cofoundry includes a system for easily creating strongly typed configuration settings just by defining a POCO class that inherits from `IConfigurationSettings`. These settings classes are automatically picked up by the DI system and bound to your config source at runtime using the underlying [ASP.NET Core `IOptions` accessor](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options). 

Additionally, configuration is checked to be valid using `IModelValidationService` so you can also take advantage of data annotations such as `[Required]` and `[MaxLength]` to ensure that setting values are entered correctly.

#### Setting Names

By default, settings use the format **{ClassName}:{PropertyName}** where *ClassName* has the text *"Settings"* removed from the end, e.g. *'ContactForm:IsContactFormEnabled'*

To add an additional level of namespacing to your settings you can use the class annotation `[NamespacedConfigurationSetting("MyNamespace")]`, which is what Cofoundry uses internally to make names like *'Cofoundry:Mail:DefaultFromAddress'*. This is particularly useful if you are making a plugin and want to ensure your config settings don't conflict.

## Example

appsettings.json:

```js
{
  "ContactForm": {
    "IsContactFormEnabled": true,
    "NotificationToAddress": "ignore-me@cofoundry.org"
  }
}
```

ContactFormSettings.cs:

```csharp
using Cofoundry.Core.Configuration;

public class ContactFormSettings : IConfigurationSettings
{
    public bool IsContactFormEnabled { get; set; }
    
    [Required]
    [EmailAddress]
    public string NotificationToAddress { get; set; } = string.Empty;
}
```

ContactController.cs:

```csharp
public class ContactController : Controller
{
    private readonly ContactFormSettings _simpleTestSiteSettings;

    public ContactController(
        ContactFormSettings simpleTestSiteSettings
        )
    {
        _simpleTestSiteSettings = simpleTestSiteSettings;
    }
    
    // .. implementation
}
```

