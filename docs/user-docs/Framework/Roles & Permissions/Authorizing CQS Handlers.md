Whilst ASP.NET generally guides you to apply authorization at the presentation layer, applying attributes like [[Authorize] to controllers](https://docs.microsoft.com/en-us/aspnet/core/security/authorization/simple) or [policies to endpoints](https://docs.microsoft.com/en-us/aspnet/core/security/authorization/policies#apply-policies-to-endpoints), with Cofoundry we prefer to apply authorization a little lower down, in what we call the the domain layer (AKA business or data layer). By defining permissions on our domain layer commands and queries we ensure that permissions are always enforced, irrespective of where a request for data originates. e.g. from an API, a web page or a console app. It also allows us to keep our authorization logic close to our data access code, which makes it easier to manage and understand without having to worry about all the places the data is consumed.

If a query or command is executed and the current user does not have the required permissions, then a `NotPermittedException` is thrown. In a Cofoundry website this exception will automatically be caught and the site will respond with a 403 (Forbidden) error. You can refer to the [custom error pages documentation](/content-management/custom-error-pages) for more information on how to format this page.

Using our [CQS](/framework/data-access/cqs) framework for your domain layer is entirely optional, and if you prefer not to use it or have different requirements than we do have other ways of [authorizing routes](authorizing-routes) or [validating permissions](querying-and-validating-in-code).

## Restricting CQS Handler Permissions

The most common way to restrict access to a command or query handler is to implement `IPermissionRestrictedCommandHandler` or `IPermissionRestrictedQueryHandler` on the handler class. When implemented this will automatically call the `GetPermissions` method on the handler and validate that the current user is assigned all the permissions you return. 

```csharp
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;

public class AddPageCommandHandler
    : ICommandHandler<AddPageCommand>
    , IPermissionRestrictedCommandHandler<AddPageCommand>
{
    public async Task ExecuteAsync(AddPageCommand command, IExecutionContext executionContext)
    {
        // execution logic removed
    }

    public IEnumerable<IPermissionApplication> GetPermissions(AddPageCommand command)
    {
        yield return new PageCreatePermission();

        if (command.Publish)
        {
            // Create AND publish permission are required
            yield return new PagePublishPermission();
        }
    }
}
```

In rare cases you might need to permit an action if one of a combination of permissions are present. For this you can use `CompositePermissionApplication`:

```csharp
public IEnumerable<IPermissionApplication> GetPermissions(AddPageCommand command)
{
    var createPermission = new PageCreatePermission();
    var updatePermission = new PageUpdatePermission();
    
    // Either create OR update permission is required
    yield return new CompositePermissionApplication(createPermission, updatePermission);
}
```

Note that you only need to return a "READ" permission if no other (more restrictive) entity permissions are returned. This is because a user is required to have read permissions to an entity before being assigned other permissions for that entity. Typically this will mean that read permissions get returned on queries, whereas commands usually return more restrictive permissions.

#### IPermissionValidationService

Sometimes you need to do more complex permissions checking in a query or command, typically this is because you need to read from a database before you can determine which permissions are required. In this case you use the `IPermissionValidationService` directly within the `ExecuteAsync` method of your handler. An example of this is in `DeleteCustomEntityCommandHandler` where we don't know the custom entity type until we have retrieved it from the db:

```csharp
using Cofoundry.Domain.Data;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;

public class DeleteCustomEntityCommandHandler
    : ICommandHandler<DeleteCustomEntityCommand>
    , IIgnorePermissionCheckHandler
{
    private readonly CofoundryDbContext _dbContext;
    private readonly IPermissionValidationService _permissionValidationService;

    public DeleteCustomEntityCommandHandler(
        CofoundryDbContext dbContext,
        IPermissionValidationService permissionValidationService
        )
    {
        _dbContext = dbContext;
        _permissionValidationService = permissionValidationService;
    }

    public async Task ExecuteAsync(DeleteCustomEntityCommand command, IExecutionContext executionContext)
    {
        var customEntity = await _dbContext
            .CustomEntities
            .SingleOrDefaultAsync(p => p.CustomEntityId == command.CustomEntityId);

        if (customEntity != null)
        {
            _permissionValidationService.EnforceCustomEntityPermission<CustomEntityDeletePermission>(
                customEntity.CustomEntityDefinitionCode, 
                executionContext.UserContext
                );

            // logic code removed
        }
    }
}
```

## Simple Permission Handling

#### Requiring a signed in user

If your application only requires simple permission checking to make sure a user is signed in, then you can implement `ICofoundryUserPermissionCheckHandler` or `ISignedInPermissionCheckHandler` which allows you to simply check that a user is signed in or is a Cofoundry admin panel user.

This can be a lot simpler than implementing full permissions checking if you know that your application isn't going to need fine-grained permission-based access.

#### IIgnorePermissionCheckHandler

If your query/command does not need permission restrictions then it must implement `IIgnorePermissionCheckHandler`. This is done so that you don't accidentally forget to consider and apply permissions to a query/command.

This might be the case if your handler just wraps calls to other queries or commands that already handle permissions, or if permission checking is done manually during execution.