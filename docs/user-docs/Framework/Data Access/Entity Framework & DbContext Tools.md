Cofoundry uses Entity Framework Core as its ORM strategy. You are free to use other tools and methodologies for your data access but using EF will allow you to re-use some of the existing Cofoundry features.

## DbContext Tools

There are a handful of ways to create a DbContext in EF, but our approach is to hand-code it using a few helpers to cut down on boilerplate. Here's an example:

```csharp
using Cofoundry.Core.EntityFramework;
using Microsoft.EntityFrameworkCore;

public class MySiteDbContext : DbContext
{
    private readonly ICofoundryDbContextInitializer _cofoundryDbContextInitializer;

    public MySiteDbContext(ICofoundryDbContextInitializer cofoundryDbContextInitializer)
    {
        _cofoundryDbContextInitializer = cofoundryDbContextInitializer;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        _cofoundryDbContextInitializer.Configure(this, optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasAppSchema()
            .ApplyConfiguration(new CatMap())
            .ApplyConfiguration(new DogMap());
    }

    public DbSet<Cat> Cats { get; set; }
    public DbSet<Dog> Dogs { get; set; }
}
```

See the [SPASite Sample](https://github.com/cofoundry-cms/Cofoundry.Samples.SPASite) for an example of this.

#### Constants

- `DbConstants.DefaultAppSchema`: The default/suggested schema for your applications tables "app", but you can alternatively use you own.
- `DbConstants.CofoundrySchema`: The schema for Cofoundry tables "Cofoundry"

#### Mapping

 - `modelBuilder.HasAppSchema()`: Shortcut for setting the default schema to 'app'.

#### CMS Data Class Mapping

You can mix classes from the Cofoundry DbContext into your own DbContext if you want to link to entities like Images, Users or Pages. To pull in the mappings you can use `modelBuilder.MapCofoundryContent()` in the `OnModelCreating` override.

#### Inheriting from CofoundryDbContext

If you're mixing your own data models with Cofoundry data models, you might find it easier to simply extend `CofoundryDbContext` with your own data models:

```csharp
using Cofoundry.Core.EntityFramework;
using Microsoft.EntityFrameworkCore;

public class MySiteDbContext : CofoundryDbContext
{
    public MySiteDbContext(ICofoundryDbContextInitializer cofoundryDbContextInitializer)
        : base(cofoundryDbContextInitializer)
    {
    }

    public DbSet<Cat> Cats { get; set; }
    public DbSet<Dog> Dogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder
            .HasAppSchema()
            .ApplyConfiguration(new CatMap())
            .ApplyConfiguration(new DogMap());
    }
}
```

## Executing Stored Procedures & Raw SQL 

`IEntityFrameworkSqlExecutor` helps you to execute raw SQL statements against an EF DbContext, including methods for executing table queries, scalar queries, commands and commands with an output parameter.

```csharp
using Cofoundry.Core.EntityFramework;
using Cofoundry.Domain.CQS;
using Microsoft.Data.SqlClient;

public class SetCatFavoriteCommandHandler 
    : ICommandHandler<SetCatFavoriteCommand>
    , ISignedInPermissionCheckHandler
{
    private readonly IEntityFrameworkSqlExecutor _entityFrameworkSqlExecutor;
    private readonly MyDbContext _dbContext;
    
    public SetCatFavoriteCommandHandler(
        IEntityFrameworkSqlExecutor entityFrameworkSqlExecutor,
        MyDbContext dbContext
        )
    {
        _entityFrameworkSqlExecutor = entityFrameworkSqlExecutor;
        _dbContext = dbContext;
    }

    public Task ExecuteAsync(SetCatFavoriteCommand command, IExecutionContext executionContext)
    {
        return _entityFrameworkSqlExecutor
            .ExecuteCommandAsync(_dbContext, 
                "app.Cat_SetFavorite",
                new SqlParameter("@CatId", command.CatId),
                new SqlParameter("@UserId", executionContext.UserContext.UserId),
                new SqlParameter("@IsLiked", command.IsFavorite),
                new SqlParameter("@CreateDate", executionContext.ExecutionDate)
                );
    }
}
```
