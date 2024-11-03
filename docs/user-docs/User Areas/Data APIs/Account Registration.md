New users can be added to a user area in the Cofoundry admin panel, however more often a site requires a self-service user registration form that users can use to register their own accounts. In Cofoundry a self-service account registration flow can be achieved by combining several different Cofoundry APIs.

> The **RegistrationAndVerificationSample** project in the [Cofoundry.Samples.UserAreas sample](https://github.com/cofoundry-cms/Cofoundry.Samples.UserAreas) contains an example of self-service user account registration flow.

## Programatically adding a new user

Registering a user starts with the ability to create a new user. In Cofoundry this is done with `AddUserCommand`, which is a general purpose command for creating new user accounts. It does not send any notification emails, it simply adds the user to the system.

To add a new user, execute `AddUserCommand` via the `IAdvancedContentRepository`:

```csharp
var userId = await _advancedContentRepository
    .Users()
    .AddAsync(new()
    {
        UserAreaCode = MemberUserArea.Code,
        RoleCode = MemberRole.Code,
        Username = "Example User",
        Password = "ExamplePassword",
        Email = "user@example.com"
    });
```

Adding a user to a custom user area requires the `NonCofoundryUserCreatePermission`. Typically when registering a new user the command is executed by an anonymous (not signed in) user and therefore we need to elevate permissions to the system user account by calling `WithElevatedPermissions()` on the content repository. This will be shown in the next example.

## Building a verification flow

Sometimes all that is required is to create a new user, but more often there will be other tasks that needs to be included in the registration flow, for example:

- Initiating [account verification](account-verification)
- [Signing in](authentication)
- Sending a welcome email
- Initiating a custom [authorized task flow](authorized-tasks)
- Running some other custom data access

Below is an example of an account registration and verification flow adapted from the [Cofoundry.Samples.UserAreas sample](https://github.com/cofoundry-cms/Cofoundry.Samples.UserAreas):

```csharp
public async Task Register(RegisterViewModel viewModel)
{
    // When executing multiple commands we should run them inside a transaction
    using (var scope = _advancedContentRepository.Transactions().CreateScope())
    {
        // The anonymous user does not have permissinos to add users
        // so we need to run the commands under the system account by
        // calling "WithElevatedPermissions()".
        var userId = await _advancedContentRepository
            .WithElevatedPermissions()
            .Users()
            .AddAsync(new()
            {
                UserAreaCode = MemberUserArea.Code,
                RoleCode = MemberRole.Code,
                Username = viewModel.Username,
                Password = viewModel.Password,
                Email = viewModel.Email
            });

        // In this example we require members to validate their account 
        // before we let them sign in. Initiating verification will send an
        // email notification containing a unique link to verify the account
        await _advancedContentRepository
            .Users()
            .AccountVerification()
            .EmailFlow()
            .InitiateAsync(new()
            {
                UserId = userId
            });

        // Complete the transaction if no errors have occurred
        await scope.CompleteAsync();
    }
}
```