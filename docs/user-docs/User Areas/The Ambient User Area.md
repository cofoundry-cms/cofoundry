Cofoundry supports multiple user areas, and a person could have a user account in each of them. Therefore someone could be signed into multiple user accounts at the same time, one for each user area. While it's rare that you'd need to implement more than one custom user area, it's worth understanding how this affects referencing the "current" user. To explain this let's look at a few scenarios:

#### Scenario 1: No Custom User Areas

- We have no custom user areas, only the built-in Cofoundry Admin user area. 
- The Cofoundry Admin user area will be set as the default auth scheme.
- When querying the current user, by default the "current" the user associated with the Cofoundry Admin user area will be returned.

#### Scenario 2: One Custom User Area

- We have one custom user area in addition to the built-in Cofoundry Admin user area.
- We set our custom user area to be the default auth scheme in the definition by setting `IUserAreaDefinition.IsDefaultAuthScheme` to `true`.
- Our custom user area will be set as the default auth scheme.
- When querying the current user, by default the user associated with our custom user area will be returned.
- It's rare that you'd want to query the admin user area in this scenario, but to do so you would need to query using one of the methods listed later in this section.

#### Scenario 3: Two Custom User Areas

- We have two custom user area in addition to the built-in Cofoundry Admin user area. Let's call them "Area 1" and "Area 2".
- We set up "Area 1" to be the default auth scheme in the definition by setting `IUserAreaDefinition.IsDefaultAuthScheme` to `true`. Only one user area can be set as the default auth scheme.
- "Area 1" will be set as the default auth scheme.
- When querying the current user, by default the user associated with "Area 1" will be returned.
- If you want to query the current user for "Area 2" you would need to query using one of the methods listed later in this section.

Scenario 1 and 2 are by far the most common. If this matches your scenario then you won't have to worry about managing the ambient user area, as long as your custom user area is marked as the default auth scheme.

### The "ambient" user area

When working with multiple user areas, we refer to the user area associated with the current request as the "ambient" user area. Generally the ambient user area is going to match the default auth scheme unless the context has been switched during the flow of the request or DI scope lifetime. This can happen in the following cases:

- An authorization attribute has been applied to the currently executing controller or Razor page e.g. `[AuthorizeUserArea]` or `[AuthorizeRole]`. The ambient user area will be changed to the authorized user area.
- Access to a dynamic page is granted by an [access rule](/user-areas/controlling-access). The ambient user area will be changed to match that of the user area associated with the authorized access rule.
- A manual call to `IUserSessionService.SetAmbientUserAreaAsync(userAreaCode)` is made. Changing the ambient user area during a request is not recommended and is mostly intended for internal use. If you need to query an alternative use area, use the methods listed below.
- The ASP.NET auth scheme is changed outside of the scope of Cofoundry e.g. `[Authorize(AuthenticationSchemes="MyCustomScheme")]`.

### Requesting a specific user area

Sometimes you may need to ignore the ambient user area and request the current user for a specific user area. To do this you can use one of the methods below:

#### VIA IContentRepository

When using `IContentRepository` (or similar) you can change the context of the command or query executing by calling `WithContext<TUserArea>()` in the method chain:

```csharp
var user = await _contentRepository
    .WithContext<MemberUserArea>()
    .Users()
    .Current()
    .Get()
    .AsDetails()
    .ExecuteAsync();
```

####  Via `ICofoundryHelper`

When working in ASP.NET Razor view files, the [Cofoundry View Helper](/content-management/cofoundry-view-helper) can be used to access the current user via the `CurrentUser` property. To query for a specific user area, simply use the `GetAsync(string userAreaCode)` overload:

```html
@inject ICofoundryHelper Cofoundry

@{
    var user = await Cofoundry.CurrentUser.GetAsync(MemberUserArea.Code);
}

@if (user.IsSignedIn)
{
    <p>Welcome @user.Data.Username</p>
}
```