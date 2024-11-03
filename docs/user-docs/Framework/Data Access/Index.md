## CQS, IQuery & ICommand 

Under the covers Cofoundry uses a lightweight [CQS](/framework/data-access/CQS) framework to structure data access. You can structure your data access any way you want, but this framework is there for you to use if you want to benefit from:

- A best-practice and structured methodology
- Integrated validation
- Integrated permission handling
- Integrated logging

[Go to section](CQS) 

## IDomainRepository

`IDomainRepository` is the base repository that `IContentRepository` and `IAdvancedContentRpository` inherit from. It provides easy access to components such as generic command and query execution, permission escalation and transaction management. 

[Go to section](IDomainRepository)

## File Sources

When passing files to commands you'll encounter the `IFileSource` abstraction. This section covers the range of `IFileSource` implementations that you can use to upload files to Cofoundry.

[Go to section](File-Sources)

## File Storage

Cofoundry uses a storage abstraction when it needs to work with files. Most commonly this is used when saving or loading image or document assets, but the abstraction can also be used for other files. This section covers the usage and configuration of the `IFileStoreService` abstraction.

[Go to section](File-Storage)

## Paged Queries

Cofoundry uses a standardized approach to paging queries that return large sets of data. You can take advantage of this standardized framework if you want to be consistent with Cofoundry, but it's entirely optional.

[Go to section](Paged-Queries)

## Transactions

Cofoundry has a transaction manager abstraction that provides a few benefits over `System.Transactions`.

[Go to section](Transactions) 

## Entity Framework

Cofoundry uses Entity Framework Core for data access and includes a few tools you may wish to use if you are also using EF Core for your own data.

[Go to section](Entity-Framework-and-DbContext-Tools) 

## Database Migrations

Cofoundry does not use EF Database Migrations and instead we use our own framework for managing database updates via SQL scripts. This framework is also available to you as well as plugin developers.

For more information see the [Auto Update Documentation](/framework/Auto-Update)

