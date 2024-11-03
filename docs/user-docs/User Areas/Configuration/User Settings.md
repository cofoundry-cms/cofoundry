## Global user settings

User settings can be configured globally using the standard ASP.NET configuration system e.g. by adding configuration settings to your `app.config` file. Modifying settings in this way affects all user areas, including the Cofoundry admin panel user area, unless otherwise stated in the documentation. A full list of permissions can be found in the [common config settings section](/references/common-config-settings), but categories include:

- [AccountRecovery](/references/common-config-settings#accountrecovery): Configures the behavior of the self-service account recovery feature
- [Authentication](/references/common-config-settings#authentication): Configures security parameters for authentication.
- [AccountVerification](/references/common-config-settings#accountverification): Configures the behavior of the account verification feature. Note that the Cofoundry admin panel does not support an account verification flow and therefore these settings do not apply.
- [Cleanup](/references/common-config-settings#cleanup): Configures the background task that runs to clean up stale user data.
- [Cookies](/references/common-config-settings#cookies): Configures the auth cookie
- [EmailAddress](/references/common-config-settings#emailaddress): Configures the default email address validation rules.
- [Password](/references/common-config-settings#password): Configures the default password policy.
- [Username](/references/common-config-settings#username): Configures the default username validation rules.

**Example:**

```json
{
  "Cofoundry": {
    "Users": {
      "Password": {
        "MinLength": 12,
        "MinUniqueCharacters": 5
      },
      "AccountRecovery": {
        "ExpireAfter": "01:00:00"
      },
      "Username": {
        "AllowAnyDigit": false,
        "AdditionalAllowedCharacters": "-."
      }
    }
  }
}
```

## Customizing a user area settings via IUserAreaDefinition

Modifying settings for a specific user area can be done in the `ConfigureOptions(UserAreaOptions)` method in your definition class. In this example we increase the minimum number of unique characters required in a password to 6:

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