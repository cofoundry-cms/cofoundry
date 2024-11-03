The account verification feature allows you flag a user account as verified or activated. This "verified" status is deliberately generic so the meaning will depends on your application, but as an example it could be that you simply run a manual check of a user and mark them as verified in the admin panel, or you could use an email-based flow to verify that the email address they registered with is a working email address.

> The **RegistrationAndVerificationSample** project in the [Cofoundry.Samples.UserAreas sample](https://github.com/cofoundry-cms/Cofoundry.Samples.UserAreas) contains an example of a registration and verification flow.

## Setting the verified status

The simplest way to programmatically set the verification status of a user is to execute the `UpdateUserAccountVerificationStatusCommand` via `IAdvancedContentRepository`, allowing you to simply toggle the verification status:

```csharp
await _advancedContentRepository
    .Users()
    .AccountVerification()
    .UpdateStatusAsync(new UpdateUserAccountVerificationStatusCommand()
    {
        UserId = 123,
        IsAccountVerified = true
    });
```

## Querying the Verification Status

Most user queries will return the verification status. One of the simplest is accessing the status via the current user context:

```csharp
var currentUserContext = await _contentRepository
    .Users()
    .Current()
    .Get()
    .AsUserContext()
    .ExecuteAsync();

if (currentUserContext.IsAccountVerified)
{
    // ...do something
}
```

More often you'll want to verify that a user account is verified before you sign them in:

```csharp
var authResult = await _advancedContentRepository
    .Users()
    .Authentication()
    .AuthenticateCredentials(new AuthenticateUserCredentialsQuery()
    {
        UserAreaCode = MemberUserArea.Code,
        Username = "Example User",
        Password = "ExamplePassword"
    })
    .ExecuteAsync();

authResult.ThrowIfNotSuccess();

if (!authResult.User.IsAccountVerified)
{
    // ...do something
}
```

## Requiring Verification

If you want to ensure unverified accounts cannot be signed in, you can configure this in your user area definition:

```csharp
public class MemberUserArea : IUserAreaDefinition
{
    // ... other properties omitted

    public void ConfigureOptions(UserAreaOptions options)
    {
        options.AccountVerification.RequireVerification = true;
    }
}
```

Setting `RequireVerification` to `true` will cause a validation exception to be thrown with the error code "cf-users-auth-account-not-verified" when an unverified user attempts to sign in.

## Email-based verification flow

One of the most common forms of account verification is to send an email to a user to verify that they have registered with a valid email address. Cofoundry has a set of queries and commands to manage this flow for you.

The email-based account verification flow has the following steps:

1. Initiation: An email notification with a unique verification link is sent to the user.
2. Validation: The user follows the link in their email, which contains a unique single-use authorization token. The token can be validated as a separate action if you want to handle any errors manually.
3. Completion: The token is validated and the user is marked as verified.

The email-based account verification flow APIs can be found in the `IAdvancedContentRepository` in the `Users().AccountVerification().EmailFlow()` method chain.

## User area configuration

In order to support the account recovery feature, you will need to configure the `VerificationUrlBase` setting in your user area definition. This is the relative base path used to construct the URL for the account verification completion form. A unique token will be added to the URL as a query parameter, it is then resolved using `ISiteUrlResolver.MakeAbsolute` and inserted into to the email notification.

As an example, the configuration value "/auth/account/verify" would be transformed to "https://example.com/auth/account/verify?t={token}".

The path can be configured in the `ConfigureOptions(UserAreaOptions)` method in your definition:

```csharp
public class MemberUserArea : IUserAreaDefinition
{
    // ... other properties omitted
    
    public void ConfigureOptions(UserAreaOptions options)
    {
        options.AccountVerification.VerificationUrlBase = "/members/registration/verify";
    }
}
```

The path can include other query parameters, which will be merged into the resulting URL.

## Initialization

To initiate an account verification request, execute `InitiateUserAccountVerificationViaEmailCommand` via the `IAdvancedContentRepository`:

```csharp
await _advancedContentRepository
    .Users()
    .AccountVerification()
    .EmailFlow()
    .InitiateAsync(new InitiateUserAccountVerificationViaEmailCommand()
    {
        UserId = 123
    });
```

Typically you'd initiate the flow during the user registration process, but you could also re-run the command if the user had lost their verification email.

### Rate limiting

Account verification initiation is rate limited by IP address to mitigate abuse. If the rate limit is exceed a `ValidationErrorException` is thrown with the code "cf-users-account-verification-initiation-rate-limit-exceeded". Rate limiting can be configured via `IUserAreaDefinition.ConfigureOptions(options)`:

- `options.AccountVerification.InitiationRateLimit.Quantity`: The maximum number of account verification initiation attempts to allow within the rate limit time window. Defaults to 16 attempts. If zero or less, then rate limiting does not occur.
- `options.AccountVerification.InitiationRateLimit.Window`: The time-window in which to count account verification initiation attempts when rate limiting, specified as a `TimeSpan`. Defaults to 24 hours. If zero or less, then rate limiting does not occur.

### Email Template

By default, a basic email template is used to send the email notification. If you want to customize this template, read the [docs on email notification customization](/user-areas/configuration/email-notification-customization).

## Validation

When a user follows the link in the email notification, they will be redirected to your user verification page. The request can be validated and completed in the same command, but you can validate the authorization token separately and handle validation errors manually if you prefer.

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

    [Route("verify")]
    public async Task<IActionResult> Verify(string t)
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
    .AccountVerification()
    .EmailFlow()
    .Validate(new()
    {
        UserAreaCode = MemberUserArea.Code,
        Token = "{token-from-url}"
    })
    .ExecuteAsync();

tokenValidationResult.ThrowIfNotSuccess();
```

In the above example we throw an exception if any occurred, but we could also use the `tokenValidationResult.Error` property to display the error or conditionally check the `tokenValidationResult.Error.ErrorCode` property to perform an action for specific errors. The following validation errors can occur:

- **"cf-users-account-verification-request-not-found":** Invalid id and token combination. This can include situations where the id or token are not correctly formatted, or if the request cannot be located in the database.
- **"cf-users-account-verification-request-already-complete":** The request exists but has already been completed.
- **"cf-users-account-verification-request-expired":** The request exists but has expired.
- **"cf-users-account-verification-request-invalidated":** The request has been invalidated, likely because the account has already been verified by another request.
- **"cf-users-account-verification-request-email-mismatch":** The request exists but the user has updated their email since the request was sent and is therefore considered expired.

### Token expiry

The period in which an account verification request is valid can be configured via `IUserAreaDefinition.ConfigureOptions(options)`:

- `options.AccountRecovery.ExpireAfter`: The length of time an account verification token is valid for, specified as a `TimeSpan`. Defaults to 7 days. If zero or less, then time-based validation does not occur.

## Completion

To complete the request, execute `CompleteUserAccountVerificationViaEmailCommand` using the same authorization token:

```csharp
await _contentRepository
    .Users()
    .AccountVerification()
    .EmailFlow()
    .CompleteAsync(new CompleteUserAccountVerificationViaEmailCommand()
    {
        UserAreaCode = MemberUserArea.Code,
        Token = "{token-from-url}"
    });
```

The token will be automatically re-validated, throwing a validation exception if it is invalid.
