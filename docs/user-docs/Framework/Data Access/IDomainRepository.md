`IDomainRepository` is the base repository that `IContentRepository` and `IAdvancedContentRepository` inherit from and includes all the base functionality without any of the content-specific extensions, providing easy access to components such as command and query execution, permission escalation and transaction management. 

Typically you'd use `IDomainRepository` if you are only interested in using Cofoundry as a framework without any of the CMS content APIs, however all the features documented in this section are also relevant to `IContentRepository` and `IAdvancedContentRepository`.

## Executing queries and commands

Cofoundry repositories have the following methods to facilitate working with [queries and commands](CQS):

- `ExecuteQueryAsync`: Directly executes an `IQuery` instance and returns the results.
- `WithQuery`: Allows you to chain mutator functions to run after execution of a query such as `Map` or `MapItem`.
- `ExecuteCommandAsync`: Directly executes an `ICommand` instance.
- `PatchCommandAsync`: Patches an `IPatchableCommand` command to modify the current state before executing it.

Detailed information on using the CQS framework itself is [here](CQS).

**Example:**

```csharp
var command = new ExampleCommand()
{
    ExampleId = 1
};
await _domainRepository.ExecuteCommandAsync(command);
```

Executing the queries in this way allows you to benefit from many of the additional features described here such as mapping, permission elevation and transaction management.

## Mapping

Queries executed using `WithQuery` can benefit from the same mapping features found in [content repository queries](/content-management/accessing-data-programmatically):

```csharp
var results = await _domainRepository
    .WithQuery(new SearchCustomEntityRenderSummariesQuery()
    {
        CustomEntityDefinitionCode = BlogPostCustomEntityDefinition.DefinitionCode
    })
    .MapItem(b => new { b.Title })
    .ExecuteAsync();
```

## Elevating Permissions

Execution of queries and commands is restricted by [permissions](/framework/roles-and-Permissions). If you want to run a query or command that currently signed in user doesn't have permissions for, you'll need to elevate your permissions before executing it.

An example of when this might be useful would be registering a new user from a public sign-up form. The anonymous user role does not typically have permissions to create a user, so we'd need to elevate permissions:

```csharp
public async Task RegisterUser(string email, string password)
{
    await _advancedContentRepository
        .WithElevatedPermissions()
        .Users()
        .AddAsync(new()
        {
            Email = email,
            Password = password,
            UserAreaCode = MemberUserArea.Code,
            RoleCode = MemberRole.Code
        });
}
```

## Changing Context

`IDomainRepository.WithContext` can be used to change the command or query execution context. It has three different overloads:

- `WithContext(IExecutionContext)`: Executes the command or query with the specific execution context. Typically this is used to maintain context by passing the execution context down to nested queries or commands.
- `WithContext(IUserContext)`: Switches the existing execution context to use the specified user context. This is used to impersonate a user without creating a new execution context.
- `WithContext<TUserArea>()`: Execute queries or commands using the user context associated with the specified user area. This is useful when implementing multiple user areas whereby a client can be signed into multiple user accounts belonging to different user areas. Use this to force execution to use the context of a specific user area rather than relying on the "ambient" or default. The [user area docs](/user-areas/the-ambient-user-area) contains more information about the ambient user area.

This example passes down the execution context to the execution of a nested command:

```csharp
public class ExampleCommandHandler
    : ICommandHandler<ExampleCommand>
    , ISignedInPermissionCheckHandler
{
    // ...constructor omitted for brevity

    public async Task ExecuteAsync(ExampleCommand command, IExecutionContext executionContext)
    {
        await _domainRepository
            .WithContext(executionContext)
            .ExecuteCommandAsync(new NestedCommand());
    }
}
```

## Transactions

The Cofoundry transaction manager can be accessed via the `Transactions()` extension method:

```csharp
using (var scope = _domainRepository.Transactions().CreateScope())
{
    // Do stuff
    await scope.CompleteAsync();
}
```

Read more about the Cofoundry transaction manager in the [transactions docs](transactions).

## Influencing execution with ModelState

The `Cofoundry.Web` namespace includes extensions that make it easier to work with queries or command execution in ASP.NET controllers and Razor Pages by making use of `ModelState` to work with validation errors. This is covered in the [content repository docs](/content-management/accessing-data-programmatically).