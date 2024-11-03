The account recovery feature allows a user to regain access to their account when their credentials no longer work. This is often referred to as a password reset or forgot password feature. In Cofoundry, the account recovery flow has the following steps:

1. [Initiation](#initiating-account-recovery): The user provides their username and this is used to validate their account and send them an email with a link that can use to create a new password.
2. [Validation](#validating-an-account-recovery-request): The user follows the link in their email, which contains a unique single-use authorization token. The token should be validated with Cofoundry before a password reset form is displayed.
3. [Completion](#completing-an-account-recovery-request): The user enters a new password, which is sent to Cofoundry with the authentication token to re-validate and complete the password reset.

> The AuthenticationSample project in the [Cofoundry.Samples.UserAreas sample](https://github.com/cofoundry-cms/Cofoundry.Samples.UserAreas) contains an example account recovery flow.

The account recovery APIs can be found in the `IAdvancedContentRepository` in the `Users().AccountRecovery()` method chain.

## User area configuration

In order to support the account recovery feature, your user area needs to be configured to allow password sign in: 

```csharp
public class MemberUserArea : IUserAreaDefinition
{
    // ... other properties omitted
    
    public bool AllowPasswordSignIn => true;
}
```

You will also need to configure the `RecoveryUrlBase` setting. This is the relative base path used to construct the URL for the account recovery completion form. A unique token will be added to the URL as a query parameter, it is then resolved using `ISiteUrlResolver.MakeAbsolute` and inserted into to the email notification.

As an example, the configuration value "/auth/account-recovery/complete" would be transformed to "https://example.com/auth/account-recovery/complete?t={token}".

The path can be configured in the `ConfigureOptions(UserAreaOptions)` method in your definition:

```csharp
public class MemberUserArea : IUserAreaDefinition
{
    // ... other properties omitted
    
    public bool AllowPasswordSignIn => true;

    public void ConfigureOptions(UserAreaOptions options)
    {
        options.AccountRecovery.RecoveryUrlBase = "/members/reset-password";
    }
}
```

The path can include other query parameters, which will be merged into the resulting URL.

## Initiating account recovery

To initiate an account recovery request, execute `InitiateUserAccountRecoveryViaEmailCommand` via the `IAdvancedContentRepository`:

```csharp
await _advancedContentRepository
    .Users()
    .AccountRecovery()
    .InitiateAsync(new()
    {
        UserAreaCode = MemberUserArea.Code,
        Username = "ExampleUsername"
    });
```

### Rate limiting

Account recovery initiation is rate limited by IP address to mitigate abuse. If the rate limit is exceed a `ValidationErrorException` is thrown with the code "cf-users-account-recovery-initiation-rate-limit-exceeded". Rate limiting can be configured via `IUserAreaDefinition.ConfigureOptions(options)`:

- `options.AccountRecovery.InitiationRateLimit.Quantity`: The maximum number of account recovery initiation attempts to allow within the rate limit time window. Defaults to 16 attempts. If zero or less, then rate limiting does not occur.
- `options.AccountRecovery.InitiationRateLimit.Window`: The time-window in which to count account recovery initiation attempts when rate limiting, specified as a `TimeSpan`. Defaults to 24 hours. If zero or less, then rate limiting does not occur.

### Execution duration

Account recovery initiation applies a "random duration" technique to mitigate timing-based enumeration attacks to discover valid usernames. This can be configured via `IUserAreaDefinition.ConfigureOptions(options)`:

- `options.AccountRecovery.ExecutionDuration.Enabled`: Controls whether the randomized execution duration feature is enabled. Defaults to `true`.
- `options.AccountRecovery.ExecutionDuration.MinInMilliseconds`: The inclusive lower bound of the randomized execution duration, measured in milliseconds (1000ms = 1s). Defaults to 1.5 second.
- `options.AccountRecovery.ExecutionDuration.MaxInMilliseconds`: The inclusive upper bound of the randomized execution duration, measured in milliseconds (2000ms = 2s). Defaults to 2 seconds.

Note that the minimum duration should exceed the expected duration of the command, and this duration will depend on the response times of your database and the method you choose to dispatch emails. A long duration is used by default to account for slow email dispatch services.

### Email Template

By default, a basic email template is used to send the email notification. If you want to customize this template, read the [docs on email notification customization](/user-areas/configuration/email-notification-customization).

## Validating an account recovery request

When a user follows the link in the email notification, they will be redirected to your password reset page. On arrival you should validate token using the `ValidateUserAccountRecoveryByEmailQuery` query.

### Extracting the authorization token 

By default the authorization token is passed in a query parameter named "t". One way to extract this is to use the `IAuthorizedTaskTokenUrlHelper`, but you can also use other methods such as ASP.NET parameter binding if you feel that's easier. The follow example shows both methods used in a controller:

```csharp
[Route("members")]
public class MemberController : Controller
{
    private readonly IAdvancedContentRepository _advancedContentRepository;
    private readonly IAuthorizedTaskTokenUrlHelper _authorizedTaskTokenUrlHelper;

    public MemberController(
        IAdvancedContentRepository advancedContentRepository,
        IAuthorizedTaskTokenUrlHelper authorizedTaskTokenUrlHelper
        )
    {
        _advancedContentRepository = advancedContentRepository;
        _authorizedTaskTokenUrlHelper = authorizedTaskTokenUrlHelper;
    }

    [Route("reset-password")]
    public async Task<IActionResult> ResetPassword(string t)
    {
        // t == token
        var token = _authorizedTaskTokenUrlHelper.ParseTokenFromQuery(Request.Query);

        // ... other code omitted
    }
}
```

### Executing the validation query

Once you have the token you can execute the validation query:

```csharp
var tokenValidationResult = await _advancedContentRepository
    .Users()
    .AccountRecovery()
    .Validate(new()
    {
        UserAreaCode = MemberUserArea.Code,
        Token = "{token-from-url}"
    })
    .ExecuteAsync();

tokenValidationResult.ThrowIfNotSuccess();
```

In the above example we throw an exception if any occurred, but we could also use the `tokenValidationResult.Error` property to display the error or conditionally check the `tokenValidationResult.Error.ErrorCode` property to perform an action for specific errors. The following validation errors can occur:

- **"cf-users-account-recovery-request-not-found":** Invalid id and token combination. This can include situations where the id or token are not correctly formatted, or if the request cannot be located in the database.
- **"cf-users-account-recovery-request-already-complete":** The request exists but has already been completed.
- **"cf-users-account-recovery-request-expired":** The request exists but has expired.
- **"cf-users-account-recovery-request-invalidated":** The request has been invalidated, likely because the password has already been updated, or a valid sign in has occurred.

### Token expiry

The period in which an account recovery request is valid can be configured via `IUserAreaDefinition.ConfigureOptions(options)`:

- `options.AccountRecovery.ExpireAfter`: The length of time an account recovery token is valid for, specified as a `TimeSpan`. Defaults to 16 hours. If zero or less, then time-based validation does not occur.

## Completing an account recovery request

To complete the request, execute `CompleteUserAccountRecoveryViaEmailCommand` using the same authorization token and the new password:

```csharp
await _advancedContentRepository
    .Users()
    .AccountRecovery()
    .CompleteAsync(new()
    {
        UserAreaCode = MemberUserArea.Code,
        Token = "{token-from-url}",
        NewPassword = "ExamplePassword"
    });
```

The token will be automatically re-validated, throwing a validation exception if it is invalid.

### Email Template

Whenever a user password is changed, an email notification is sent to the user to ensure that they are aware of the change. By default, a basic email template is used to send the notification. If you want to customize this template, read the [docs on email notification customization](/user-areas/configuration/email-notification-customization).
