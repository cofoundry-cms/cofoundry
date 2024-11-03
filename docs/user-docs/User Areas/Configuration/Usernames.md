If your user area is not configured to use an email address as the username, you may want to customize the username formatting and validation behavior. Whenever a user is added or their username is updated, Cofoundry will run the username through a number of processes:

- **[Normalization](#customizing-username-normalization):** The process of tidying up a username into a consistent format. This value may be used for display and so it's rare that you'd want to change it dramatically, therefore the default implementation simply trims the input e.g. " E.Example" becomes "E.Example".
- **[Uniquification](#customizing-username-uniquification):** The process of formatting a username into a format that can be used for comparing usernames, e.g. to prevent duplicate registrations via a uniqueness check and to lookup a user during sign in. By default we normalize and lowercase the username, for example "E.Example" becomes "e.example".
- **[Validation](#customizing-username-validation):** The process of ensuring a username is in a valid format and passes any domain rules such as uniqueness checks. By default the validator simply checks that username is less than 150 characters and is unique.

By default these processes do minimal formatting and validation, however you can customize all these processes if you want to alter the behavior and be more restrictive.

## Customizing username normalization

> Be careful when customizing this behavior on an existing user area, because any changes can break compatibility with existing users defined using the default normalizer.

You can override the normalization process for a given user area by implementing `IUsernameNormalizer<TUserArea>`. Once defined, your custom formatter will automatically be registered by the Cofoundry DI system and will be used in place of the default for your user area.

## Customizing username uniquification

> Be careful when customizing this behavior on an existing user area, because any changes can break compatibility with existing users defined using the default normalizer.

By default the uniquification process simply normalizes and lowercases the username, however you may want to be more restrictive when validating username uniqueness e.g. removing special characters so that "JaneDoe", "Jane.Doe" and "Jane Doe" are considered equivalent and map to the same user account.

To override the process for a given user area, implement `IUsernameUniquifier<TUserArea>`. Once defined, your custom formatter will automatically be registered by the Cofoundry DI system and will be used in place of the default for your user area. Here's an example implementation that removes special characters:

```csharp
using Cofoundry.Core;
using Cofoundry.Domain;

public class CustomerUsernameUniquifier : IUsernameUniquifier<CustomerUserArea>
{
    private readonly IUsernameNormalizer _usernameNormalizer;

    public CustomerUsernameUniquifier(IUsernameNormalizer usernameNormalizer)
    {
        _usernameNormalizer = usernameNormalizer;
    }

    public string? Uniquify(string? username)
    {
        var normalizedUsername = _usernameNormalizer.Normalize(username);
        if (string.IsNullOrWhiteSpace(normalizedUsername))
        {
            return null;
        }

        var result = normalizedUsername
            .ToLowerInvariant()
            .Where(char.IsLetterOrDigit);

        return string.Concat(result);
    }
}
```

## Customizing username validation

The default username validation rules are as follows:

- Must be able to be parsed by the username normalization process
- Must 150 characters or less
- Must be unique

### Customizing via global settings

The easiest way to configure more restrictive rules is to add configuration settings to your `app.config` file. Modifying these settings will change the validation process for all user areas. The following settings can be modified:

- **Cofoundry:Users:Username:AllowAnyCharacter:** Allows any character in a username, effectively bypassing characters validation. Defaults to `true`, to ensure maximum compatibility to the widest range of usernames when integrating with external systems. When `true` any settings for `AllowAnyLetters`, `AllowAnyDigit` and `AdditionalAllowedCharacters` are ignored. Note that username character validation is ignored when `IUserAreaDefinition.UseEmailAsUsername` is set to `true`, because the format is already validated against the configured `EmailAddressOptions`.
- **Cofoundry:Users:Username:AllowAnyLetter:** Allows a username to contain any character classed as a unicode letter as determined by `Char.IsLetter`. This setting is ignored when `AllowAnyCharacter` is set to `true`, which is the default behavior.
- **Cofoundry:Users:Username:AllowAnyDigit:** Allows a username to contain any character classed as a decimal digit as determined by `Char.IsDigit` i.e 0-9. This setting is ignored when `AllowAnyCharacter` is set to `true`, which is the default behavior.
- **Cofoundry:Users:Username:AdditionalAllowedCharacters:** Allows any of the specified characters in addition to the letters or digit characters permitted by the `AllowAnyLetter` and `AllowAnyDigit` settings. This setting is ignored when `AllowAnyCharacter` is set to `true`, which is the default behavior. The default settings specifies a handful of special characters commonly found in usernames: "-._' ".
- **Cofoundry:Users:Username:MinLength:** The minimum length of a username. Defaults to 1. Must be between 1 and 150 characters. 
- **Cofoundry:Users:Username:MaxLength:** The maximum length of a username. Defaults to 150 characters and must be between 1 and 150 characters.

**Example:**

```json
{
  "Cofoundry": {
    "Users:Username": {
        "AllowAnyCharacter": false,
        "AllowAnyLetter": true,
        "AllowAnyDigit": true,
        "AdditionalAllowedCharacters": "-._' ",
        "MinLength": 6,
        "MaxLength": 75
    }
  }
}
```

### Customizing via IUserAreaDefinition

If you need to modify validation settings for a specific user area, you can do this in the `ConfigureOptions(UserAreaOptions)` interface method in your definition class:

```csharp
using Cofoundry.Domain;

public class MemberUserArea : IUserAreaDefinition
{
    public const string Code = "MEM";

    public string UserAreaCode => Code;

    public string Name => "Member";

    public bool UseEmailAsUsername => false;

    // other properties removed for brevity

    public void ConfigureOptions(UserAreaOptions options)
    {
        options.Username.MinLength = 10;
    }
}
```

### Overriding the default IUsernameValidator

For complete control over email address validation you can implement your own `IUsernameValidator` and use the Cofoundry DI system to override the default implementation. In this example we inherit the Cofoundry `UsernameValidator` to take advantage of the existing validation mechanisms:

```csharp
using Cofoundry.Core.Validation;
using Cofoundry.Domain.Extendable;

public class ExampleUsernameValidator : UsernameValidator
{
    public ExampleUsernameValidator(
        IUserAreaDefinitionRepository userAreaDefinitionRepository,
        IAdvancedContentRepository contentRepository
        )
        : base(userAreaDefinitionRepository, contentRepository)
    {
    }

    public override async Task<IReadOnlyCollection<ValidationError>> GetErrorsAsync(IUsernameValidationContext context)
    {
        var errors = await base.GetErrorsAsync(context);
        if (errors.Any())
        {
            return errors;
        }

        // TODO: custom validation

        return errors;
    }
}
```

To override the default implementation, we have to register it manually:

```csharp
using Cofoundry.Core.DependencyInjection;

public class ExampleDependencyRegistration : IDependencyRegistration
{
    public void Register(IContainerRegister container)
    {
        container.Register<IUsernameValidator, ExampleUsernameValidator>(RegistrationOptions.Override());
    }
}
```