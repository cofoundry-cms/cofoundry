Cofoundry provides several APIs for working with the currently signed in user:

- Via [`IContentRepository`](#Via-IContentRepository) for general purpose querying
- Via [`IExecutionContext`](#via-iexecutioncontext) for easy access to the current user in a [CQS](/framework/data-access/cqs) query or command.
- Via [`ICofoundryHelper`](#via-icofoundryhelper) when accessing from ASP.NET Razor views

## Via `IContentRepository`

The current user can be accessed via `IContentRepository.Users().Current()`.

### `IsSignedIn()`

Returns `true` if there is a user signed in, based on the ambient user area context; otherwise `false`.

```csharp
var isSignedIn = await _contentRepository
    .Users()
    .Current()
    .IsSignedIn()
    .ExecuteAsync();
```

### `Get()`

Returns the current user, with options to project to a range of model types depending on how much data you need.

- `AsUserContext()`: Contains only key data about the current authenticated user such as their `UserId`, `RoleId` and which `UserArea` they belong to. The `IUserContext` is cached for the duration of the request and is the most efficient way to quickly reference the current user. If the user is not signed in then Cofoundry.Domain.UserContext.Empty is returned, which represents an anonymous user.
- `AsMicroSummary()`: A minimal projection of user data that is quick to load. If the user is not logged in then `null` is returned.
- `AsSummary()`:  Building on the `UserMicroSummary`, this projection contains additional audit and basic role data. If the user is not logged in then `null` is returned.
- `AsDetails()`: A full representation of a user, containing all properties including role and permission data. If the user is not logged in then `null` is returned.

Example:

```csharp
var userDetails = await _contentRepository
    .Users()
    .Current()
    .Get()
    .AsDetails()
    .ExecuteAsync();
```

## Via `IExecutionContext`

If you're building queries and commands using the [CQS](/framework/data-access/cqs) then the current user is included in the `IExecutionContext` as an `IUserContext` projection:

```csharp
public class ExampleCommandHandler
    : ICommandHandler<ExampleCommand>
    , IIgnorePermissionCheckHandler
{
    public Task ExecuteAsync(ExampleCommand command, IExecutionContext executionContext)
    {
        if (executionContext.UserContext.IsSignedIn())
        {
            // Signed in, UserId will not be null
            var userId = executionContext.UserContext.UserId.Value;
        }
        else
        {
            // not signed in!
        }
        
        return Task.CompletedTask;
    }
}
```

## Via `ICofoundryHelper`

When working in ASP.NET Razor view files, the [Cofoundry View Helper](/content-management/cofoundry-view-helper) can be used to access the current user via the `CurrentUser` property. The helper has two overloads:

#### `GetAsync()`

Returns information about the current user. If the user is not signed in, then a value will be returned representing an anonymous user.

```html
@inject ICofoundryHelper Cofoundry

@{
    var user = await Cofoundry.CurrentUser.GetAsync();
}

@if (user.IsSignedIn)
{
    <p>Welcome @user.Data.Username</p>
}
else
{
    <p>IsAnonymousRole will be true: @user.Role.IsAnonymousRole.</p>
    <p>Data will be null: @user.Data.</p>
}
```

#### `GetAsync(string userAreaCode)`

Returns the user signed into the specified user area. This is useful when multiple user areas are defined, because users can be signed into multiple user areas at the same time.