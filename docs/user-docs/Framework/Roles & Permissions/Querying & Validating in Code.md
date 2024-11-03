If none of the other authorization mechanisms meet your requirements, you can manually validate permissions using `IPermissionValidationService`. This service contains many methods that allow you to query or enforce permissions against both the current user context as well as one you provide yourself.

All the "Enforce" methods will throw a `PermissionValidationFailedException` if the user does not meet the permission requirements. This exception inherits from the standard `NotPermittedException` which will get caught by the global error handler in a Cofoundry web application and show the relevant 403 (Forbidden) error page.

#### Example

```csharp
using Cofoundry.Domain;

public class PermissionChecker
{
    private readonly IPermissionValidationService _permissionValidationService;
    private readonly IUserContextService _userContextService;

    public PermissionChecker(
        IPermissionValidationService permissionValidationService,
        IUserContextService userContextService
        )
    {
        _permissionValidationService = permissionValidationService;
        _userContextService = userContextService;
    }

    public async Task Check()
    {
        // "Async" versions will check against the "current" context, loading it on-demand
        var canCurrentUserPublishPages = await _permissionValidationService.HasPermissionAsync<PagePublishPermission>();

        // Note that IUserContext instances are cached for the duration of the request, so repeat calls do have a significant performance cost
        var canCreateProducts = await _permissionValidationService.HasCustomEntityPermissionAsync<CustomEntityCreatePermission>(ProductCustomEntityDefinition.Code);

        // We can also provide an alternative context to validate
        var memberUserContext = await _userContextService.GetCurrentContextByUserAreaAsync(MemberUserArea.Code);

        // "Enforce" versions of method will throw an exception if invalid
        _permissionValidationService.EnforceIsSignedIn(memberUserContext);

        // We can also permit the current user to make an action they do not usually have permission to do e.g. if they are the owner of an entity
        var documentOwnerUserId = 3;
        _permissionValidationService.IsCurrentUserOrHasPermission<DocumentAssetUpdatePermission>(documentOwnerUserId, memberUserContext);
    }
}
```
