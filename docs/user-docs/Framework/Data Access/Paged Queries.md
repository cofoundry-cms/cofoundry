Cofoundry uses a standardized approach to paging queries that return large sets of data. You can take advantage of this standardized framework if you want to be consistent with Cofoundry, but it's entirely optional.

## IPageableQuery

All pageable queries in Cofoundry inherit from our standard interface `IPageableQuery` which contains properties for `PageNumber` and `PageSize`.

You can also directly use or inherit from `SimplePageableQuery` which simply implements the `IPageableQuery` interface properties.

```csharp
using Cofoundry.Domain;

public class GetProductsQuery : SimplePageableQuery
{
    // …additional members omitted
}
```

## IPagedQueryResult<TResult>

`IPagedQueryResult<TResult>` represents our standard approach to returning results from a paged query and includes a collection of result items as well as all the basic properties you'd need to create a paging UI such as the total number of items, total page count and current page number. Our default implementation of this is `PagedQueryResult<TResult>`. 

This example shows a paged result returned from a Cofoundry query:

```csharp
using Cofoundry.Domain;

public class Example
{
    private IDomainRepository _domainRepository;

    public Example(IDomainRepository domainRepository)
    {
        _domainRepository = domainRepository;
    }

    public async Task RunPagedQuery()
    {
        var query = new SearchCustomEntityRenderSummariesQuery()
        {
            CustomEntityDefinitionCode = "EXMPLE",
            PageSize = 40,
            PageNumber = 1
        };

        // Example using a Cofoundry query to get a paged result
        var pagedResult = await _domainRepository.ExecuteQueryAsync(query);

        // Examples of various properties and methods on IPagedResult<TResult>
        IReadOnlyCollection<CustomEntityRenderSummary> pagedItems = pagedResult.Items;
        int totalNumberOfItemsWithoutPaging = pagedResult.TotalItems;
        bool isFirstPage = pagedResult.IsFirstPage();
        bool isLastPage = pagedResult.IsLastPage();
    }
}
```

## Paging data with Entity Framework

To run your own paging queries on an EF DbContext you can use our query extensions:

```csharp
using Cofoundry.Domain;

public class ExampleRespository
{
    private ExampleDbContext _dbContext;

    public ExampleRespository(ExampleDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IPagedQueryResult<Product>> GetProducts(GetProductsQuery query)
    {
        // If you don't need the paging data you can use the Page(query) extention method
        List<Product> simplePagedList = await _dbContext
            .Products
            .AsNoTracking()
            .Page(query)
            .ToListAsync();

        // Call ToPagedResultAsync(query) to get the full IPagedQueryResult
        PagedQueryResult<Product> fullResult = await _dbContext
            .Products
            .AsNoTracking()
            .ToPagedResultAsync(query);

        return fullResult;
    }
}
```

Unlike the example above, you might prefer to return a domain model rather than the raw EF model. To do this first execute the EF query as normal, then map the items and use `result.ChangeType(newItems)` to created a new PagedQueryResult for the mapped items.

```csharp
public async Task<IPagedQueryResult<ProductSummary>> GetProducts(GetProductsQuery query)
{
    PagedQueryResult<Product> result = await _dbContext
        .Products
        .AsNoTracking()
        .ToPagedResultAsync(query);

    List<ProductSummary> mappedItems = result
        .Items
        .Select(Map)
        .ToList();

    PagedQueryResult<ProductSummary> mappedResult = result.ChangeType(mappedItems);

    return mappedResult;
}

public ProductSummary Map(Product product)
{
    // …mapping code omitted
}
```

## Returning an empty paged result

Similar to `Enumerable.Empty<T>()` and `Array.Empty<T>()` an empty paged result can be created using `PagedQueryResult.Empty<T>(query)`. This is sometimes useful when you want to return early from a method without running the EF query code.

## Setting query bounds

There's no page size limit or default size so it's up to you to apply those limits to your preference. By default you can also set the page size to a negative value to indicate that the query should be unbounded (return all items).

To change this behavior you can use the `SetBounds` extension method. This allows the consumer of your application or API to adjust paging settings to their liking, without abusing the API to return too much data.

```csharp
public void Example(IPageableQuery query)
{
    // Set default page size to 10, allow unbounded page size e.g. page size of -1 returns all items
    query.SetBounds(10, true);

    // Set default page size to 40, max page size to 100
    query.SetBounds(40, 100);
}
```

