Cofoundry includes `ITransactionScopeManager` for managing transactions across multiple statements. This is actually a light wrapper around [`System.Transaction.TransactionScope`](https://docs.microsoft.com/en-us/dotnet/api/system.transactions.transactionscope?view=net-6.0) and supports the same functionality, but includes some extra features to help with coordinating transactions when executing nested commands.

Here's an example:

```csharp
using Cofoundry.Core.Data;
using Cofoundry.Domain.CQS;

public class AddProductHandler
{
    private readonly MyDbContext _myDbContext;
    private readonly ITransactionScopeManager _transactionScopeManager;
    private readonly ICommandExecutor _commandExecutor;

    public AddProductHandler(
        MyDbContext myDbContext,
        ITransactionScopeManager transactionScopeManager,
        ICommandExecutor commandExecutor
        )
    {
        _myDbContext = myDbContext;
        _transactionScopeManager = transactionScopeManager;
        _commandExecutor = commandExecutor;
    }

    public async Task ExecuteAsync(AddProductCommand command)
    {
        // …add product to myDbContext (omitted) 

        // Create a new scope to manage the transaction using the connection associated with _myDbContext
        using (var scope = _transactionScopeManager.Create(_myDbContext))
        {
            // Save the EF context inside the transaction
            await _myDbContext.SaveChangesAsync();

            // Execute a nested command
            await _commandExecutor.ExecuteAsync(new AllocateProductOwnerCommand()
            {
                // …set some properties on the command (omitted)
            });

            // Complete the scope
            await scope.CompleteAsync();
        }
    }
}
```

In this example our command uses a transaction to ensure that local `_myDbContext.SaveChangesAsync()` operation and any data access executed inside  `AllocateProductOwnerCommand` fails or succeeds as a single operation. `AllocateProductOwnerCommand` itself may also use `ITransactionScopeManager` which will utilize the ambient transaction initiated in the outer command.

## Accessing via Repositories

Instead of using `ITransactionScopeManager` directly, it's often simpler to access this functionality from our [repository classes](/content-management/accessing-data-programmatically) such as [`IDomainRepository`](idomainrepository). In this case, the database connection associated with the ambient Cofoundry DbContext is used.

```csharp
public class TransactionExample
{
    private readonly IAdvancedContentRepository _contentRepository;

    public TransactionExample(
        IAdvancedContentRepository contentRepository
        )
    {
        _contentRepository = contentRepository;
    }

    public async Task RunExample()
    {
        using (var scope = _contentRepository.Transactions().CreateScope())
        {
            await _contentRepository
                .PageDirectories()
                .AddAsync(new()
                {
                    // …set some properties on the command
                });

            await _contentRepository
                .Pages()
                .AddAsync(new()
                {
                    // …set some properties on the command
                });

            await scope.CompleteAsync();
        }
    }
}
```

## Why not use System.Transaction.TransactionScope?

Before the re-introduction of System.Transactions in .NET Core 2.0 `ITransactionScopeManager` was a wrapper around EF transactions which added support for nested scopes. Now that System.Transactions is available the default `ITransactionScopeManager` implementation uses `System.Transaction.TransactionScope` to manage nested scopes, so it might seem pointless to use this abstraction, however there are some additional benefits of using the Cofoundry implementation:

- Deferred execution of "completion tasks" in nested transactions
- Better defaults for scope creation

## Deferred execution of "completion tasks"

`ITransactionScopeManager` adds the ability to defer execution of tasks until after the transaction is complete. Cofoundry uses this internally to ensure that message handlers and cache breaking code does not run until the top-level parent transaction is complete.

Here's an example based on a simplified [`DeletePageCommandHandler`](https://github.com/cofoundry-cms/cofoundry/blob/master/src/Cofoundry.Domain/Domain/Pages/Commands/DeletePageCommandHandler.cs). In the sample  `scope.QueueCompletionTask` is used to ensure the message publication and cache clearing code is only run when the parent transaction is complete. 

```csharp
public class DeletePageCommandHandler
{
    // …constructor removed for brevity
        
    public async Task ExecuteAsync(DeletePageCommand command, IExecutionContext executionContext)
    {
        var page = await _dbContext
            .Pages
            .FilterById(command.PageId)
            .SingleOrDefaultAsync();

        if (page == null)
        {
            return;
        }

        _dbContext.Pages.Remove(page);
            
        using (var scope = _transactionScopeManager.Create(_dbContext))
        {
            await _dbContext.SaveChangesAsync();
            await _pageStoredProcedures.UpdatePublishStatusQueryLookupAsync(command.PageId);

            scope.QueueCompletionTask(() => OnTransactionComplete(command));
            await scope.CompleteAsync();
        }
    }

    private Task OnTransactionComplete(DeletePageCommand command)
    {
        _pageCache.Clear(command.PageId);

        return _messageAggregator.PublishAsync(new PageDeletedMessage()
        {
            PageId = command.PageId
        });
    }
}
```

It's unlikely that anyone implementing a site using Cofoundry will need to use the task queuing features directly, but if you're running built-in Cofoundry commands from your code in transactions then it's best to manage the scopes using `ITransactionScopeManager` to ensure internal Cofoundry code runs as expected.

## Better defaults for scope creation

`ITransactionManager` uses the following defaults when creating a new `TransactionScope`:

- `TransactionScopeAsyncFlowOption.Enabled`: Since we should always use async for database access, we enable this by default
- `IsolationLevel.ReadCommitted`: EF uses a default isolation level of `Serializable` for backwards compatibility, but this is [rarely the best choice](https://joshthecoder.com/2020/07/27/transactionscope-considered-annoying.html) and doesn't align with the SQL Server default. Instead Cofoundry uses `ReadCommitted` by default.

### Customizing scope creation

#### Per Transaction

To change the scope creation settings there are several extensions you can use. These extensions are specific to our default Cofoundry implementation and so you will need to reference the namespace `Cofoundry.Core.Data.TransactionScopeManager.Default` to use them.

```csharp

using Cofoundry.Domain;
using Cofoundry.Domain.TransactionManager.Default;

public class TransactionExample
{
    private readonly IAdvancedContentRepository _contentRepository;

    public TransactionExample(
        IAdvancedContentRepository contentRepository
        )
    {
        _contentRepository = contentRepository;
    }

    public async Task RunExample()
    {
        using (var scope = _contentRepository
                .Transactions()
                .CreateScope(TransactionScopeOption.Required, IsolationLevel.Serializable))
        {
            // …code removed for brevity

            await scope.CompleteAsync();
        }
        
        using (var scope = _contentRepository
                .Transactions()
                .CreateScope(CreateScope))
        {
            // …code removed for brevity

            await scope.CompleteAsync();
        }
    }
    
    private TransactionScope CreateScope()
    {
        var options = new TransactionOptions()
        {
            IsolationLevel = IsolationLevel.Serializable,
            Timeout = TransactionManager.DefaultTimeout
        };

        return new TransactionScope(
            TransactionScopeOption.Required,
            options,
            TransactionScopeAsyncFlowOption.Suppress
            );
    }
}
```

#### Customizing the defaults

To change the default settings for scope creation, create your own [`ITransactionScopeFactory`](https://github.com/cofoundry-cms/cofoundry/blob/master/src/Cofoundry.Core/Data/Transactions/TransactionScopeFactory.cs) implementation and override it using the [DI System](/framework/dependency-injection).