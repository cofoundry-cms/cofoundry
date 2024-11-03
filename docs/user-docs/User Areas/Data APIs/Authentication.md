Authentication APIs can be found in the `IAdvancedContentRepository` in the `Users().Authentication()` method chain.

> The **AuthenticationSample** project in the [Cofoundry.Samples.UserAreas sample](https://github.com/cofoundry-cms/Cofoundry.Samples.UserAreas) contains an example of authentication features.

## Signing In

### Single-step

Authentication and sign in can be completed in a single operation using `SignInWithCredentialsAsync(SignInUserWithCredentialsCommand)`. When signing in using this method, any validation errors that occur will be thrown as `ValidationErrorException`. This makes it more suitable when you do not need to take any action based on the type of error, or in a web API where the errors are sent directly to the client application.

```csharp
await _advancedContentRepository
    .Users()
    .Authentication()
    .SignInWithCredentialsAsync(new()
    {
        UserAreaCode = MemberUserArea.Code,
        Username = "user@example.com",
        Password = "ExamplePassword",
        RememberUser = true
    });
```

### Multi-step

Often it will be more suitable to use a multi-step sign in process where we might:

1. Authenticate the user with a query
2. Identify if there's any special actions we need to take e.g. a password change requirement
3. Sign in the user if the attempt is valid

This flow is more flexible and allows you to inject any custom requirements you have into the process.

```csharp
// First authenticate the user without signing them in
var authResult = await _advancedContentRepository
    .Users()
    .Authentication()
    .AuthenticateCredentials(new()
    {
        UserAreaCode = MemberUserArea.Code,
        Username = "user@example.com",
        Password = "ExamplePassword"
    })
    .ExecuteAsync();

// If credentials are not valid, throw
authResult.ThrowIfNotSuccess();

// If a special action is required
if (authResult.User.RequirePasswordChange)
{
    // ... redirect etc
}
else if (!authResult.User.IsAccountVerified)
{
    // ... redirect etc
}

// ... or additional custom validation

// If no action required: sign the user in
await _advancedContentRepository
    .Users()
    .Authentication()
    .SignInAuthenticatedUserAsync(new()
    {
        UserId = authResult.User.UserId,
        RememberUser = true
    });
```

### Validation Errors

The errors returned from the `AuthenticateCredentials(AuthenticateUserCredentialsQuery)` are the same that are thrown from `SignInWithCredentialsAsync(SignInUserWithCredentialsCommand)`. Each validation error has a unique code, and some use specific exception types that make them easier to catch if you prefer to use a `try`/`catch` approach:

- **cf-user-auth-invalid-credentials**: Either the username or password is invalid. Thrown as an `InvalidCredentialsAuthenticationException`
- **cf-user-auth-max-attempts-exceeded**: Too many failed authentication attempts have occurred either for the username or IP address.
- **cf-user-auth-not-specified**: The error was not specified. This can be used when an error is picked up outside of the core authentication operation e.g. in MVC if the ModelState is invalid and the result is returned before authentication is attempted.

These errors will also be thrown when signing in a user that does not pass these additional checks:

- **cf-user-auth-password-change-required**: The credentials are valid but a password change is required before sign in is permitted. This error isn't expected to be shown to the user but is instead expected to be intercepted and handled in the UI. Thrown as a `PasswordChangeRequiredException`.
- **cf-user-auth-account-not-verified**: The credentials are valid but the account has not been verified, and the user area is configured to not allow sign ins for unverified users. Thrown as an `AccountNotVerifiedException`.

### Rate limiting

Credential authentication is rate limited to mitigate abuse. If the rate limit is exceed a `ValidationErrorException` is thrown with the code "cf-users-auth-rate-limit-exceeded". Rate limiting can be configured via `IUserAreaDefinition.ConfigureOptions(options)`:

- `options.Authentication.IPAddressRateLimit.Quantity`: The maximum number of failed authentication attempts allowed per IP address during the rate limit time window. The default value is 50 attempts.
- `options.Authentication.IPAddressRateLimit.Window`: The time window to measure authentication attempts when rate limiting by IP address, specified as a `TimeSpan`. The default value is 60 minutes.
- `options.Authentication.UsernameRateLimit.Quantity`: The maximum number of failed authentication attempts allowed per username during the rate limiting time window. The default value is 20 attempts.
- `options.Authentication.UsernameRateLimit.Window`: The time window to measure authentication attempts when rate limiting by username, specified as a `TimeSpan`. The default value is 60 minutes. The default value is 60 minutes.

### Execution duration

Credential authentication applies a "random duration" technique to mitigate timing-based enumeration attacks to discover valid usernames. This can be configured via `IUserAreaDefinition.ConfigureOptions(options)`:

- `options.Authentication.ExecutionDuration.Enabled`: Controls whether the randomized execution duration feature is enabled. Defaults to `true`.
- `options.Authentication.ExecutionDuration.MinInMilliseconds`: The inclusive lower bound of the randomized credential authorization execution duration, measured in milliseconds (1000ms = 1s). Defaults to 1 second.
- `options.Authentication.ExecutionDuration.MaxInMilliseconds`: The inclusive upper bound of the randomized credential authorization execution duration, measured in milliseconds (2000ms = 2s). Defaults to 1.5 seconds.

Note that the minimum duration should exceed the expected duration of the authentication query, which will depend on the response times of your database and the hashing algorithm used.

## Signing Out

To sign out of the current (ambient) user area:

```csharp
await _advancedContentRepository
    .Users()
    .Authentication()
    .SignOutAsync();
```

To sign out of all user areas (including the Cofoundry admin panel user area):

```csharp
await _advancedContentRepository
    .Users()
    .Authentication()
    .SignOutAllUserAreasAsync();
```

## Forcing a password change

Users can be forced to change their password at sign in by setting the `RequirePasswordChange` flag to `true` when adding or updating a user. When this setting is `true` then any attempt to sign into the account will throw a `PasswordChangeRequiredException` validation exception. If you want to support forcing a password change in your sign in flow, the best approach is to use the [multi-step sign in approach](#multi-step) documented above and redirect these users to a password change form. Alternatively, if you are authenticating over a web API boundary you can handle the "cf-user-auth-password-change-required" error code in your front-end application.

### Updating the password

To update the password, execute `UpdateUserPasswordByCredentialsCommand` via the `IAdvancedContentRepository`:

```csharp
await _advancedContentRepository
    .Users()
    .UpdatePasswordByCredentialsAsync(new()
    {
        UserAreaCode = MemberUserArea.Code,
        Username = "Example Username",
        OldPassword = "ExampleOldPassword",
        NewPassword = "ExampleNewPassword"
    });
```

Because the user is prevented from signing in when a password change is required, they will need to provide sign in credentials again to make the change.