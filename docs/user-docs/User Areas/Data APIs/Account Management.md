
Cofoundry provides several APIs for managing the current user's account:

- [Updating](#updating-the-current-user-account)
- [Changing the password](#updating-the-current-users-password)
- [Deleting](#deleting-an-account)

> The **AuthenticationSample** project in the [Cofoundry.Samples.UserAreas sample](https://github.com/cofoundry-cms/Cofoundry.Samples.UserAreas) contains an example of account management features.

## Updating the current user account

The current user's account can be updated by executing `UpdateCurrentUserCommand`. The easiest way to run this command is to use the `IAdvancedContentRepository` which provides an easy to use "patch" API. This means you don't have to worry about the existing state of the user, just update the properties you have values for:

```csharp
await _advancedContentRepository
    .Users()
    .Current()
    .UpdateAsync(c =>
    {
        c.DisplayName = "Example User";
        c.Email = "user@example.com";
    });
```

Executing this command requires the `CurrentUserUpdatePermission`.

## Updating the current users password

The password of the currently signed in user account can be updated using `UpdateCurrentUserPasswordCommand`, which requires their existing password to authenticate the request. As with other queries and commands that authenticate, rate limiting applies to mitigate abuse.

The easiest way to run this command is via `IAdvancedContentRepository`:

```csharp
await _advancedContentRepository
    .Users()
    .Current()
    .UpdatePasswordAsync(new()
    {
        OldPassword = "ExampleOldPassword",
        NewPassword = "ExampleNewPassword"
    });
```

Executing this command requires the `CurrentUserUpdatePermission`.

## Deleting an account

The current user account can be deleted using `DeleteCurrentUserCommand`. This marks the user as deleted in the database (soft delete) and signs them out. Fields containing personal data are cleared and any optional dependencies are deleted. The remaining user record and relations are left in place for auditing.

Log tables that contain IP references are not deleted, however they will be cleared out periodically by a background task if background tasks are enabled, based on the [configured retention policy](/references/common-config-settings#cleanup).

The easiest way to run this command is via `IAdvancedContentRepository`:

```csharp
await _advancedContentRepository
    .Users()
    .Current()
    .DeleteAsync();
```

Executing this command requires the `CurrentUserDeletePermission`.