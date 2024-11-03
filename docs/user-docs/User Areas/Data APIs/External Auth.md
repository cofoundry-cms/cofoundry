> Improvements to better support external auth providers is tracked in [issue 163 ](https://github.com/cofoundry-cms/cofoundry/issues/163)

While there are currently no plugins to work with external auth providers, it is possible to use the existing APIs to provision and sign in a user that has already been authenticated by your custom code. The following example takes a pre-authenticated username and signs it in, provisioning a new user account if one does not already exist:

```csharp
public async Task SignMemberInAsync(string username)
{
    // Because the user is not signed in yet we need to elevate permissions here,
    // otherwise the anonymous user will be used and a permission exception will be thrown
    var existingUser = await _advancedContentRepository
        .WithElevatedPermissions()
        .Users()
        .GetByUsername<MemberUserArea>(username)
        .AsMicroSummary()
        .ExecuteAsync();

    int userId;

    if (existingUser == null)
    {
        // If we haven't signed in with this user before, we'll provision a new user account
        // to match the SSO user account.
        userId = await _advancedContentRepository
            .WithElevatedPermissions()
            .Users()
            .AddAsync(new()
            {
                UserAreaCode = MemberUserArea.Code,
                RoleCode = MemberRole.Code,
                Username = username
            });
    }
    else
    {
        // If the user already exists, we sign in using that UserId
        userId = existingUser.UserId;
    }

    await _advancedContentRepository
        .Users()
        .Authentication()
        .SignInAuthenticatedUserAsync(new()
        {
            UserId = userId,
            RememberUser = true
        });
}
```
