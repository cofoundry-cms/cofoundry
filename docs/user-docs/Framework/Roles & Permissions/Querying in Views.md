The [Cofoundry View Helper](/content-management/cofoundry-view-helper) has a `CurrentUser` property that can be used to retrieve information about the currently signed in user. The helper has two overloads:

- `GetAsync()`: Returns the current user. If multiple user areas are implemented then the current user will be based on the currently applied auth scheme, which by default will be the user area with `IsDefaultAuthScheme` set to `true`, but can be overridden by authorization attributes such as `[AuthorizeUserArea]`.
- `GetAsync(string userAreaCode)`: Returns the user signed into the specified user area. This is useful when multiple user areas are defined, because users can be signed into multiple user areas at the same time.

These methods return a model that provides user and role information which can be used to determine user permissions e.g. `user.Role.HasPermission<TPermission>()`.

#### Example

```html
@inject ICofoundryHelper Cofoundry

@{
    var user = await Cofoundry.CurrentUser.GetAsync();
}

<h1> Permission Example</h1>

@if (user.Role.HasPermission<MyTestPermission>())
{
    <h2>Restricted Content</h2>
    <p>You have MyTestPermission</p>
}
@if (user.Role.IsSuperAdministrator)
{
    <h2>SuperAdmin Content</h2>
    <p>You are a super administrator</p>
}
@if (user.Role.UserArea.UserAreaCode == MyCustomUserArea.Code)
{
    <h2>Custom User Area Content</h2>
    <p>Special content for MyCustomUserArea users</p>
}
```

#### Caching

Once fetched the `CurrentUserViewHelperContext` data is cached for the duration of the request, so there's no problem with calling this method multiple times in different components.
