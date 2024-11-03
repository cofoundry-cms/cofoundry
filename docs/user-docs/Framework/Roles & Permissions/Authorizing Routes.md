## Restricting Access to CMS Directories and Pages

Restricting access to directories and pages dynamically created through the CMS is explained in the [user area section](/content-management/user-areas/controlling-access) of the docs.

## Restricting Access To Controllers

To restrict access to ASP.NET MVC or API controllers, you can use one of the Cofoundry authorization attributes. These attributes inherit from the ASP.NET `[Authorize]` attribute and can be applied to a controller, action, or Razor Page in the same way. For more detailed information on applying `[Authorize]` attributes reference [the ASP.NET documentation on authorization](https://docs.microsoft.com/en-us/aspnet/core/security/authorization/simple).

In the examples below, we make use of the following user area and role definitions:

```csharp
public class MemberUserArea : IUserAreaDefinition
{
    public const string Code = "MEM";

    public string UserAreaCode { get; } = Code;

    public string Name { get; } = "Member";

    // ...other properties removed for brevity
}

public class VipMemberRole : IRoleDefinition
{
    public const string Code = "VIP";

    public string Title { get { return "VIP Member"; } }

    public string RoleCode { get { return Code; } }

    public string UserAreaCode { get { return MemberUserArea.Code; } }
    
    public void ConfigurePermissions(IPermissionSetBuilder builder)
    {
        builder
            .ApplyAnonymousRoleConfiguration()
            .Include<VipDealsPermission>();
    }
}
```

### [AuthorizeUserArea]

Use the `[AuthorizeUserArea]` attribute to restrict access to users associated with a specific user area. Using this attribute in a multi-user-area application will set the current user context to the specified user area, rather than the default.

```csharp
[Route("deals")]
public class DealsController : Controller
{
    [Route("vip-deals")]
    [AuthorizeRole(MemberUserArea.Code, VipMemberRole.Code)]
    public IActionResult VIPDeals()
    {
        return View();
    }
}
```

### [AuthorizeRole]

Use the `[AuthorizeRole]` attribute to restrict access to users associated with a specific Cofoundry role. Only roles defined in code using `IRoleDefinition` can be authorized with this attribute. Using this attribute in a multi-user-area application will set the current user context to the specified user area, rather than the default.

```csharp
[Route("deals")]
public class DealsController : Controller
{
    [Route("vip-deals")]
    [AuthorizeRole(MemberUserArea.Code, VipMemberRole.Code)]
    public IActionResult ExclusiveDeals()
    {
        return View();
    }
}
```

### [AuthorizePermission]

Use the `[AuthorizePermission]` attribute to restrict access to users with a role that
includes the specified permission. The permission needs to be passed in as a type reference:

**VIPDealsPermission.cs**

```csharp
public class VipDealsPermission : IPermission
{
    public PermissionType PermissionType => new("VIPDEA", "VIP Deals", "Access to VIP deals");
}
```

**DealsController.cs**
```csharp
[Route("deals")]
public class DealsController : Controller
{
    [Route("vip-deals")]
    [AuthorizePermission(typeof(VipDealsPermission))]
    public IActionResult VIPDeals()
    {
        return View();
    }
}
```

#### Custom Entity Permissions

Custom entity permissions are generated based on permission templates, and so when using `[AuthorizePermission]` attribute with a custom entity permission, we also need to supply the custom entity definition code:

```csharp
[Route("products")]
[AuthorizePermission(typeof(CustomEntityReadPermission), ProductCustomEntityDefintion.Code)]
public class ProductsController : Controller
{
    [Route("")]
    public IActionResult Index()
    {
        return View();
    }
}
```

## Restricting access to endpoints

The authorization attributes listed above all make use of ASP.NET policies and schemes to describe authorization requirements. Endpoint routing doesn't support authorization attributes so instead you can use our formatting helpers to reference the existing scheme and policy names:

- `AuthenticationSchemeNames`: Use to reference the authentication schemes defined by Cofoundry.
- `AuthorizationPolicyNames`: Use to reference the names of authorization policies defined by Cofoundry.

```csharp
app.UseEndpoints(endpoints =>
{
    endpoints
        .MapGet("/member", MemberAction)
        .RequireAuthorization(AuthorizationPolicyNames.UserArea(MemberUserArea.Code));
});
```