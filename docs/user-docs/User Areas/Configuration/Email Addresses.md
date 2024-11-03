Whenever a user is added or their email address is updated, Cofoundry will run the email through a number of processes:

- **[Normalization](#customizing-email-normalization):** The process of tidying up an email into a consistent format which can still be used to contact the user. By default we trim the value and lowercase the domain, but make no other alterations to ensure that the email is preserved as the user intended. For Example "Eric@EXAMPLE.COM" becomes "Eric@example.com".
- **[Uniquification](#customizing-email-uniquification):** The process of formatting an email into a format that can be used to prevent duplicate registrations via a uniqueness check. By default we normalize and lowercase the email, for example "Eric@EXAMPLE.COM" becomes "eric@example.com". This format is not used for contacting a user, only for uniqueness checks.
- **[Validation](#customizing-email-validation):** The process of ensuring an email address is in a valid format and passes any domain rules such as uniqueness checks. By default the validator simply checks that email is in the format "someone@somewhere" to ensure maximum compatibility to the widest range of email addresses, which is inline with the behavior of the ASP.NET [`[EmailAddress]`](https://docs.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.emailaddressattribute?view=net-6.0) data annotation attribute.

By default these processes do minimal formatting and validation because a more restrictive implementation may have side-effects that are undesirable in some scenarios. However you can customize all these processes if you want to alter the behavior and be more restrictive.

## Customizing email normalization

Customizing email normalization isn't recommended, because any changes may affect the delivery of emails, however you can override the process for a given user area by implementing `IEmailAddressNormalizer<TUserArea>`. Once defined, your custom formatter will automatically be registered by the Cofoundry DI system and will be used in place of the default for your user area.

## Customizing email uniquification

In some scenarios you may want to be more restrictive when validating email uniqueness, e.g. when accepting registrations for trial services. While there isn't a 100% effective method for preventing duplicate registrations, you may be able to filter some obvious duplication by customizing the email "uniquification" process.

To override the process for a given user area, implement `IEmailAddressUniquifier<TUserArea>`. Once defined, your custom formatter will automatically be registered by the Cofoundry DI system and will be used in place of the default for your user area. Here's an example implementation that prevents multiple registrations from the same gmail account:

```csharp
using Cofoundry.Core;
using Cofoundry.Domain;

public class CustomerEmailAddressUniquifier : IEmailAddressUniquifier<CustomerUserArea>
{
    private readonly IEmailAddressNormalizer _emailAddressNormalizer;

    public CustomerEmailAddressUniquifier(IEmailAddressNormalizer emailAddressNormalizer)
    {
        _emailAddressNormalizer = emailAddressNormalizer;
    }

    public NormalizedEmailAddress? UniquifyAsParts(string? emailAddress)
    {
        var normalized = _emailAddressNormalizer.NormalizeAsParts(emailAddress);
        return UniquifyAsParts(normalized);
    }

    public NormalizedEmailAddress? UniquifyAsParts(NormalizedEmailAddress? emailAddressParts)
    {
        const string GMAIL_DOMAIN = "gmail.com";

        if (emailAddressParts == null)
        {
            return null;
        }

        // merge both gmail domains as they point to the same inbox
        // ignore any plus addressing and remove superflous dots for gmail addresses only
        var uniqueEmail = emailAddressParts
            .MergeDomains(GMAIL_DOMAIN, "googlemail.com")
            .AlterIf(email => email.HasDomain(GMAIL_DOMAIN), email =>
            {
                return email
                    .WithoutPlusAddressing()
                    .AlterLocal(local => local.Replace(".", string.Empty));
            });

        return uniqueEmail;
    }
}
```

With the above example in place, a customer trying to register two variations of the same gmail account will receive a validation error.

## Customizing email validation

The default email address validation rules are as follows:

- Must be able to be parsed by the email normalization process i.e. in the format "someone@somewhere"
- Must be 3 or more characters
- Must be 150 characters or less

Additionally, if the user area is configured to use the email address as the username, then username validation process will also ensure the email address is unique.

### Customizing via global settings

The easiest way to configure more restrictive rules is to add configuration settings to your `app.config` file. Modifying these settings will change the validation process for all user areas, including the Cofoundry admin panel user area. The following settings can be modified:

- **Cofoundry:Users:EmailAddress:AllowAnyCharacter:** Allows any character in an email, effectively bypassing characters validation. Defaults to `true`, to ensure maximum compatibility to the widest range of email addresses. When `true` any settings for `AllowAnyLetters`, `AllowAnyDigit` and `AdditionalAllowedCharacters` are ignored.
- **Cofoundry:Users:EmailAddress:AllowAnyLetter:** Allows an email to contain any character classed as a unicode letter as determined by `Char.IsLetter`. This setting is ignored when `AllowAnyCharacter` is set to `true`, which is the default behavior.
- **Cofoundry:Users:EmailAddress:AllowAnyDigit:** Allows an email to contain any character classed as a decimal digit as determined by `Char.IsDigit` i.e 0-9. This setting is ignored when `AllowAnyCharacter` is set to `true`, which is the default behavior.
- **Cofoundry:Users:EmailAddress:AdditionalAllowedCharacters:** Allows any of the specified characters in addition to the letters or digit characters permitted by the `AllowAnyLetters` and `AllowAnyDigit` settings. This setting is ignored when `AllowAnyCharacter` is set to `true`, which is the default behavior. The @ symbol is always permitted. The default settings specifies the range of special characters permitted in unquoted email addresses, excluding comment parentheses "()", and the square brackets "[]" that are used to denote an IP address instead of a domain i.e "!#$%&'*+-/=?^_`{|}~.@". When enabling or altering these settings please be aware of the [full extent of acceptable email formats](https://en.wikipedia.org/wiki/Email_address#Syntax).
- **Cofoundry:Users:EmailAddress:MinLength:** The minimum length of an email address. Defaults to 3. Must be between 3 and 150 characters. 
- **Cofoundry:Users:EmailAddress:MaxLength:** The maximum length of an email address. Defaults to 150 characters and must be between 3 and 150 characters.
- **Cofoundry:Users:EmailAddress:RequireUnique:** Set this to `true` to ensure that an email cannot be allocated to more than one user per user area. Note that if `IUserAreaDefinition.UseEmailAsUsername` is set to `true` then this setting is ignored because usernames have to be unique. This defaults to `false` because a uniqueness check during registration can expose whether an email is registered or not, which may be sensitive information depending on the nature of the application.

**Example:**

```json
{
  "Cofoundry": {
    "Users:EmailAddress": {
        "AllowAnyCharacter": false,
        "AllowAnyLetter": true,
        "AllowAnyDigit": true,
        "AdditionalAllowedCharacters": "!#$%&'*+-/=?^_`{|}~.",
        "RequireUnique": true
    }
  }
}
```

### Customizing via IUserAreaDefinition

If you need to modify validation settings for a specific user area, you can do this in the `ConfigureOptions(UserAreaOptions)` interface method in your definition class. In this example we configure the validator to only allow unique email addresses:

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
        options.EmailAddress.RequireUnique = true;
    }
}
```

### Overriding the default IEmailAddressValidator

For complete control over email address validation you can implement your own `IEmailAddressValidator` and use the Cofoundry DI system to override the default implementation. In this example we inherit the Cofoundry `EmailAddressValidator` to take advantage of the existing validation mechanisms:

```csharp
using Cofoundry.Core.Validation;
using Cofoundry.Domain.Extendable;

public class ExampleEmailAddressValidator : EmailAddressValidator
{
    public ExampleEmailAddressValidator(
        IUserAreaDefinitionRepository userAreaDefinitionRepository,
        IAdvancedContentRepository contentRepository
        )
        : base(userAreaDefinitionRepository, contentRepository)
    {
    }

    public override async Task<IReadOnlyCollection<ValidationError>> GetErrorsAsync(IEmailAddressValidationContext context)
    {
        var errors = await base.GetErrorsAsync(context);
        if (errors.Count != 0)
        {
            return errors;
        }

        // TODO: your custom validation

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
        container.Register<IEmailAddressValidator, ExampleEmailAddressValidator>(RegistrationOptions.Override());
    }
}
```