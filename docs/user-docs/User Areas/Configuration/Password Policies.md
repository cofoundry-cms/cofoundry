When a user password is changed, Cofoundry will validate the password against a set of criteria. The default password policy includes the following criteria:

- Must be at least 10 characters.
- Must be 300 characters or less.
- Must have at least 5 unique characters.
- Must not be the same as the users current password.
- Must not be the same as the users email or username.
- Must not be a sequence of numbers or characters e.g. "abcdefgh" or "87654321".

Cofoundry offers various ways of customizing a password policy, from applying basic settings globally to creating bespoke validators that can be applied to specific user areas.

## Global password settings

The easiest way to modify the default policy is to add configuration settings to your `app.config` file. Modifying these settings will change the default policy for all user areas, including the Cofoundry admin panel user area. The following basic settings can be modified:

- **Cofoundry:Identity:Password:MinLength:** The minimum length of a password. Defaults to 10 and anything less is not recommended. Must be between 6 and 2048 characters.
- **Cofoundry:Identity:Password:MaxLength:** The maximum length of a password. Defaults to 300 characters and must be between 6 and 2048 characters.
- **Cofoundry:Identity:Password:MinUniqueCharacters:** The number of unique characters required in a password. This is to prevent passwords like "aabbccdd". Defaults to 5 unique characters.

**Example:**

```json
{
  "Cofoundry": {
    "Identity:Password": {
        "MinLength": 10,
        "MaxLength": 300,
        "MinUniqueCharacters": 5
    }
  }
}
```

## Customizing a user area policy via IUserAreaDefinition

If you need to modify the basic password policy settings for a specific user area, you can do this in the `ConfigureOptions(UserAreaOptions)` interface method in your definition class. In this example we increase the minimum number of unique characters required to 6:

```csharp
using Cofoundry.Domain;

public class MemberUserArea : IUserAreaDefinition
{
    public const string Code = "MEM";

    public string UserAreaCode => Code;

    public string Name => "Member";

    public bool AllowPasswordSignIn => true;

    // other properties removed for brevity

    public void ConfigureOptions(UserAreaOptions options)
    {
        options.Password.MinUniqueCharacters = 6;
    }
}
```

## Customizing a user area policy via IPasswordPolicyConfiguration

For more control over the password policy for an individual user area you can implement `IPasswordPolicyConfiguration<TUserAreaDefinition>`, which provides complete control over the password policy builder. Your implementation will be registered automatically by the Cofoundry DI system.

In the following example, we'll upgrade the password policy of the built-in `CofoundryAdminUserArea` by increasing the minimum length, building on top of the default configuration:

```csharp
using Cofoundry.Domain;

public class AdminPasswordPolicyConfiguration : IPasswordPolicyConfiguration<CofoundryAdminUserArea>
{
    public void Configure(IPasswordPolicyBuilder builder)
    {
        builder.UseDefaults(c => c.MinLength = 12);
    }
}
```

So far we're only altering simple properties and relying on the defaults, but let's take it a step further and replace the `UseDefaults()` method to illustrate how we can add validators to the policy manually:

```csharp
public class AdminPasswordPolicyConfiguration : IPasswordPolicyConfiguration<CofoundryAdminUserArea>
{
    public void Configure(IPasswordPolicyBuilder builder)
    {
        builder
            .SetDescription($"Passwords must be between 12 and 300 characters.")
            .ValidateMinLength(12)
            .ValidateMaxLength(300)
            .ValidateMinUniqueCharacters(5)
            .ValidateNotCurrentPassword()
            .ValidateNotPersonalData()
            .ValidateNotSequential()
            ;
    }
}
```

### Changing the description

In the above example we added a description using `builder.SetDescription(string)`. The description is an optional property that can be used to succinctly describe the policy, and it is used in the admin panel to help guide users when creating a new password. If you're altering the password policy for the Cofoundry admin user area then you may want to also update the description, however it's not required for other custom user areas.

If do want to use the dynamically generated description yourself, it can be retrieved through `IAdvancedContentRepository`:

```csharp
var policyDescription = await _advancedContentRepository
    .UserAreas()
    .PasswordPolicies()
    .GetByCode(MemberUserArea.Code)
    .AsDescription()
    .ExecuteAsync();
```

The policy description also contains a collection of criteria statements from each of the validators, which is useful is you need a more thorough description of the policy.

### Configuring multiple user areas

To apply your `IPasswordPolicyConfiguration` to multiple user areas, simply implement the interface for each definition type:

```csharp
public class ExamplePasswordPolicyConfiguration 
    : IPasswordPolicyConfiguration<CofoundryAdminUserArea>
    , IPasswordPolicyConfiguration<MemberUserArea>
{
    public void Configure(IPasswordPolicyBuilder builder)
    {
        // Add config here
    }
}
```

## Creating custom password validators

### A basic password validator

Validators can be either synchronous or asynchronous, but let's start with a simple example of a synchronous validator that requires a password to contain a digit.

By default Cofoundry doesn't enforce any "character-composition" requirements, as this goes against current guidance (see [OWASP Authentication Cheat Sheet](https://cheatsheetseries.owasp.org/cheatsheets/Authentication_Cheat_Sheet.html#implement-proper-password-strength-controls)), however if the requirement is mandated by a client we can still enforce it by implementing a custom validator:

```csharp
public class RequireDigitNewPasswordValidator : INewPasswordValidator
{
    public string Criteria => "Must contain a digit.";

    public ValidationError? Validate(INewPasswordValidationContext context)
    {
        ValidationError? result = null;

        if (!context.Password.Any(char.IsDigit))
        {
            result = new ValidationError("Password must contain a digit", context.PropertyName);
        }

        return result;
    }
}
```

The `Validate(INewPasswordValidationContext)` method should return a `ValidationError` if validation was unsuccessful, otherwise `null` should be returned to indicate success.

We can now add the validator to our `IPasswordPolicyConfiguration` implementation:

```csharp
using Cofoundry.Core.Validation;
using Cofoundry.Domain;

public class MemberPasswordPolicyConfiguration  : IPasswordPolicyConfiguration<MemberUserArea>
{
    public void Configure(IPasswordPolicyBuilder builder)
    {
        builder
            .UseDefaults()
            .AddValidator<RequireDigitNewPasswordValidator>();
    }
}
```

### An async password validator

In some cases you may need to run async code to validate a password e.g. to query a database or read a file. In the following example we query a fictional repository of forbidden passwords, and return an error if the password matches one of the values.

This example also highlights that validators supports DI. Our DI system automatically scans for validators and registers them with the DI container.

```csharp
using Cofoundry.Core.Validation;
using Cofoundry.Domain;

public class NotInBlocklistNewPasswordValidator : IAsyncNewPasswordValidator
{
    private readonly BadPasswordRepository _badPasswordRepository;

    public NotInBlocklistNewPasswordValidator(BadPasswordRepository badPasswordRepository)
    {
        _badPasswordRepository = badPasswordRepository;
    }

    public string Criteria => $"Must not appear in the block-list.";

    public async Task<ValidationError> ValidateAsync(INewPasswordValidationContext context)
    {
        ValidationError result = null;

        var blocklist = await _badPasswordRepository.GetAll();
            
        if (blocklist.Any(b => context.Password.Equals(b, StringComparison.OrdinalIgnoreCase)))
        {
            result = new ValidationError("Password is invalid because it appears in the block-list", context.PropertyName);
        }

        return result;
    }
}
```

Note that synchronous validators always run first, and async validators will only be invoked if no other errors are found.

## Configuring a password validator

If you want to make your validator re-usable with different user areas or websites with different configurations you can add an `INewPasswordValidatorWithConfig<TOptions>` implementation to your validator.

```csharp
using Cofoundry.Core.Validation;
using Cofoundry.Domain;

public class RequireDigitNewPasswordValidator 
    : INewPasswordValidator
    , INewPasswordValidatorWithConfig<int>
{
    public string Criteria => $"Must contain at least {NumberOfDigitsRequired} digits.";

    public int NumberOfDigitsRequired { get; private set; }

    public void Configure(int numberOfDigitsRequired)
    {
        NumberOfDigitsRequired = numberOfDigitsRequired;
    }

    public ValidationError? Validate(INewPasswordValidationContext context)
    {
        ValidationError? result = null;

        var numberOfDigits = context.Password.Count(char.IsDigit);
        if (numberOfDigits < NumberOfDigitsRequired)
        {
            result = new ValidationError($"Password must contain at least {NumberOfDigitsRequired} digits", context.PropertyName);
        }

        return result;
    }
}
```

We can now configure the validator in our `IPasswordPolicyConfiguration` implementation:

```csharp
using Cofoundry.Core.Validation;
using Cofoundry.Domain;

public class MemberPasswordPolicyConfiguration  : IPasswordPolicyConfiguration<MemberUserArea>
{
    public void Configure(IPasswordPolicyBuilder builder)
    {
        builder
            .UseDefaults()
            .AddValidatorWithConfig<RequireDigitNewPasswordValidator, int>(6);
    }
}
```

If you're making a reusable validator for different projects, you may want to add an extension method to make configuration a bit more intuitive:

```csharp
using Cofoundry.Domain;

public static class IPasswordPolicyBuilderExtensions
{
    public static IPasswordPolicyBuilder ValidateMinDigits(this IPasswordPolicyBuilder builder, int numberOfDigitsRequired)
    {
        return builder.AddValidatorWithConfig<RequireDigitNewPasswordValidator, int>(6);
    }
}
```

Our configuration now looks like this:

```csharp
public class MemberPasswordPolicyConfiguration  : IPasswordPolicyConfiguration<MemberUserArea>
{
    public void Configure(IPasswordPolicyBuilder builder)
    {
        builder
            .UseDefaults()
            .ValidateMinDigits(6);
    }
}
```