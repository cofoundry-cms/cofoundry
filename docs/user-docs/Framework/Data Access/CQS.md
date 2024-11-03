Under the covers Cofoundry uses a lightweight framework for the domain layer based on the *Command-Query-Separation* (CSQ) principle.

In simple terms CQS is about accepting the fact that the data your application needs to display views (Queries) is different to the data required to write changes back to the data store (Commands). Sometimes you may even read from a different data store that you write to, for example you may write to a SQL database, but you may read from a lucene search index. 

More information available [here](https://lostechies.com/chrispatterson/2014/01/03/crud-is-not-a-service/) and [here](https://martinfowler.com/bliki/CommandQuerySeparation.html)

## How we do CQS

We've tried to keep the CQS library fairly simple. No event sourcing, just a simple way to write Query and Command classes, have them automatically registered for DI and have them executed.

The inspiration is taken from [this post by Steven van Deursen](https://blogs.cuttingedge.it/steven/posts/2011/meanwhile-on-the-command-side-of-my-architecture/) but has been adapted a fair bit.

## Queries

To define a query, just create a class that inherits from `IQuery<TResult>`. This class should define the parameters of your query - this might be a complex set of filters but more likely it may just contain an id field or even no parameters at all. By convention we add a 'Query' postfix so that a query might be called `GetAnimalByIdQuery` or `GetAnimalsByCountryQuery`.

To execute a query you'll need a handler associated with it that implements `IQueryHandler<TQuery, TQueryHandler`. Handlers are automatically injected with dependencies so you should use constructor injection to get hold of a DbContext or other services. 

The `ExecuteAsync` method gets an instance of `IExecutionContext` which you can use to get the UTC `DateTime` of execution and information about the user performing the query.

Example:

```csharp
using Cofoundry.Domain.CQS;

public class AnimalSummary
{
    public required int AnimalId { get; set; }

    public required string Name { get; set; }
}

public class GetAnimalSummaryByIdQuery 
    : IQuery<AnimalSummary?>
{
    public int AnimalId { get; set; }
}

public class GetAnimalSummaryByIdQueryHandler 
    : IQueryHandler<GetAnimalSummaryByIdQuery, AnimalSummary?>
{
    private readonly MyDbContext _dbContext;

    public GetAnimalSummaryByIdQueryHandler(MyDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<AnimalSummary?> ExecuteAsync(GetAnimalSummaryByIdQuery query, IExecutionContext executionContext)
    {
        var result = await _dbContext
            .Animals
            .AsNoTracking()
            .Select(a => new AnimalSummary()
            {
                AnimalId = a.AnimalId,
                Name = a.Name
            })
            .SingleOrDefaultAsync(a => a.AnimalId == query.AnimalId);

        return result;
    }
}
```

Queries should return models that are tailored to what the consumer needs, but should still be fairly generic to allow them to be reused in other areas. Typically we will have models to represent a couple of different scenarios that require varying amounts of data. E.g. there might be a model called an `UserSummary` with the bare minimum data, and also a `UserDetails` model that has more information. However if there is a particular scenario that requires very specific data, you can also create a specific query for it - that's the benefit of using CQS, e.g. a `UserSignInInfo` model might be tailored to information I need to know only when signing a user in.

You should never return models from your ORM or IQuerables attached to your ORM implementation - the data access layer should not be allowed to bleed into your GUI layer which can cause a headache with query optimization or future re-modelling or refactoring.

Queries are read only and should never cause changes to be made to the data store.

## Execution

You can execute queries using an instance of `IQueryExecutor`  using `queryExecutor.ExecuteAsync(query)`, this will automatically lookup the handler and run it, providing the contextual information needed to run the query.

It's often better to access the query executor through [`IContentRepository`](/content-management/accessing-data-programmatically) or  [`IDomainRepository`](idomainrepository) using `repository.ExecuteQueryAsync(query)`. These repositories are more fully featured and makes it simpler to coordinate query execution with other functionality.

## Commands

Commands work in a similar way to queries, first we define a class that implements `ICommand` and then a handler that implements `ICommandHandler`. Similarly we have an executor for commands called `ICommandExecutor` which can also be accessed through [`IContentRepository`](/content-management/accessing-data-programmatically) or  [`IDomainRepository`](idomainrepository) using `repository.ExecuteCommandAsync(command)`. 

Commands should never return data, which would break the CQS principle. If you need data after a command has been executed, make another query. We have one exception to the rule which is you may return an id when a new entity is created - you can then use this id to perform a query to get any additional data you might want. When returning an output value, create a property on the command named with the prefix *Output* e.g. *OutputUserId* and give it an `[OutputValue]` attribute, which will ensure the value has been set by the command handler.

Example:

```csharp
using Cofoundry.Domain.CQS;

public class AddAnimalCommand : ICommand
{
    [MaxLength(50)]
    [Required]
    public string Name { get; set; } = string.Empty;

    [OutputValue]
    public int OutputAnimalId { get; set; }
}

public class AddAnimalCommandHandler : ICommandHandler<AddAnimalCommand>
{
    private readonly MyDbContext _dbContext;

    public AddAnimalCommandHandler(MyDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task ExecuteAsync(AddAnimalCommand command, IExecutionContext executionContext)
    {
        var animal = new Animal
        {
            Name = command.Name
        };

        _dbContext.Animals.Add(animal);
        await _dbContext.SaveChangesAsync();

        // Set Ouput
        command.OutputAnimalId = animal.AnimalId;
    }
}
```

## ILoggableCommand

You can get an audit trail of commands executed by making your commands inherit from `ILoggableCommand`. This will happen automatically but do beware of logging sensitive data such as passwords - you can exclude these by adding `JsonIgnore` and `IgnoreDataMember` attributes to any properties you want excluded.

The base implementation of `ICommandLogService` simply logs the event using the built in [.NET logger](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/), but you can implement your own service or add a plugin to enhance this feature.

## Validation

Query and Command object are validated before execution using `IModelValidationService`. This uses the `System.ComponentModel.DataAnnotations` framework to run validation, so any data annotation will be validated as well as any implementations of `IValidatableObject`.

Any properties that fail validation will cause a `ValidationException` to be thrown. This might not seem optimal, but this is really a last line of defense and any validation errors at this stage are considered exceptional. This kind of validation should be enforced further up the calling chain without relying on exceptions, either in the UI layer or in the application layer; `IModelValidationService` provides mechanisms for getting validation errors that helps you do this as do the Cofoundry MVC and WebApi helpers.

For validation that requires a database check e.g. 'uniqueness', this can be done inside the handler execute method. If validation fails throw a `ValidationException`, `ValidationErrorException` or `UniqueConstraintViolationException` that can be handled appropriately by the caller.

## Permissions

Permissions are also enforced at the handler level, which ensures that we can write secure data access without having to worry about where the handler is being called from.

For more information about permission see the guidance [here](/Framework/Roles-&-Permissions)
