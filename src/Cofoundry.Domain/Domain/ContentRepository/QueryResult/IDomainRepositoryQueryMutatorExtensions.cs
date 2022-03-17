namespace Cofoundry.Domain;

public static class IDomainRepositoryQueryMutatorExtensions
{
    /// <summary>
    /// Maps the result of the query using the specified mapper function. The 
    /// mapping takes place after the original query has been executed. If the query
    /// result is <see langword="null"/> then mapping is skipped and the default value
    /// of <typeparamref name="TOutput"/> is returned.
    /// </summary>
    /// <typeparam name="TQueryResult">The type of the original query result.</typeparam>
    /// <typeparam name="TInput">The input type to map from.</typeparam>
    /// <typeparam name="TOutput">The result type after the mutation has been applied.</typeparam>
    /// <param name="innerMutator">The chained query mutator to run before this instance is applied.</param>
    /// <param name="mapper">A mapper function to run on the query result.</param>
    /// <returns>A new query mutator instance that allows for method chaining.</returns>
    public static IDomainRepositoryQueryMutator<TQueryResult, TOutput> Map<TQueryResult, TInput, TOutput>(
        this IDomainRepositoryQueryMutator<TQueryResult, TInput> innerMutator,
        Func<TInput, TOutput> mapper
        )
    {
        return new DomainRepositoryQueryMutator<TQueryResult, TOutput>(
            innerMutator.Query,
            async () =>
            {
                var result = await innerMutator.ExecuteAsync();
                if (result == null) return default(TOutput);

                return mapper(result);
            });
    }

    /// <summary>
    /// Maps the result of the query using the specified mapper function. The 
    /// mapping takes place after the original query has been executed. If the query
    /// result is <see langword="null"/> then mapping is skipped and the default value
    /// of <typeparamref name="TOutput"/> is returned.
    /// </summary>
    /// <typeparam name="TQueryResult">The type of the original query result.</typeparam>
    /// <typeparam name="TInput">The input type to map from.</typeparam>
    /// <typeparam name="TOutput">The result type after the mutation has been applied.</typeparam>
    /// <param name="innerMutator">The chained query mutator to run before this instance is applied.</param>
    /// <param name="mapper">An async mapper function to run on the query result.</param>
    /// <returns>A new query mutator instance that allows for method chaining.</returns>
    public static IDomainRepositoryQueryMutator<TQueryResult, TOutput> Map<TQueryResult, TInput, TOutput>(
        this IDomainRepositoryQueryMutator<TQueryResult, TInput> innerMutator,
        Func<TInput, Task<TOutput>> mapper
        )
    {
        return new DomainRepositoryQueryMutator<TQueryResult, TOutput>(
            innerMutator.Query,
            async () =>
            {
                var result = await innerMutator.ExecuteAsync();
                if (result == null) return default(TOutput);

                return await mapper(result);
            });
    }

    /// <summary>
    /// Maps each item in the collection result of a query using the specified
    /// mapper function, returning a new collection of mapped items. The mapping 
    /// takes place after the original query has been executed. If the query
    /// result is <see langword="null"/> then mapping is skipped and <see langword="null"/>
    /// is returned.
    /// </summary>
    /// <typeparam name="TQueryResult">The type of the original query result.</typeparam>
    /// <typeparam name="TInputValue">The type of collection item to map from.</typeparam>
    /// <typeparam name="TOutputValue">The result item type after the mutation has been applied.</typeparam>
    /// <param name="innerMutator">The chained query mutator to run before this instance is applied.</param>
    /// <param name="mapper">
    /// A mapper function to run on each item in the query result. If the item is <see langword="null"/>
    /// then mapping is skipped for that item and the <see langword="default"/> value of <typeparamref name="TOutputValue"/> 
    /// is returned instead.
    /// </param>
    /// <returns>A new query mutator instance that allows for method chaining.</returns>
    public static IDomainRepositoryQueryMutator<TQueryResult, ICollection<TOutputValue>> MapItem<TQueryResult, TInputValue, TOutputValue>(
        this IDomainRepositoryQueryMutator<TQueryResult, ICollection<TInputValue>> innerMutator,
        Func<TInputValue, TOutputValue> mapper
        )
    {
        return new DomainRepositoryQueryMutator<TQueryResult, ICollection<TOutputValue>>(
            innerMutator.Query,
            async () =>
            {
                var result = await innerMutator.ExecuteAsync();
                if (result == null) return null;

                return result.Select(i => i == null ? default(TOutputValue) : mapper(i)).ToList();
            });
    }

    /// <summary>
    /// Maps each item in the collection result of a query using the specified
    /// mapper function, returning a new collection of mapped items. The mapping 
    /// takes place after the original query has been executed. If the query
    /// result is <see langword="null"/> then mapping is skipped and <see langword="null"/>
    /// is returned.
    /// </summary>
    /// <typeparam name="TQueryResult">The type of the original query result.</typeparam>
    /// <typeparam name="TInputValue">The type of collection item to map from.</typeparam>
    /// <typeparam name="TOutputValue">The result item type after the mutation has been applied.</typeparam>
    /// <param name="innerMutator">The chained query mutator to run before this instance is applied.</param>
    /// <param name="mapper">
    /// A mapper function to run on each item in the query result. If the item is <see langword="null"/>
    /// then mapping is skipped for that item and the <see langword="default"/> value of <typeparamref name="TOutputValue"/> 
    /// is returned instead.
    /// </param>
    /// <returns>A new query mutator instance that allows for method chaining.</returns>
    public static IDomainRepositoryQueryMutator<TQueryResult, ICollection<TOutputValue>> MapItem<TQueryResult, TInputValue, TOutputValue>(
        this IDomainRepositoryQueryMutator<TQueryResult, IEnumerable<TInputValue>> innerMutator,
        Func<TInputValue, TOutputValue> mapper
        )
    {
        return new DomainRepositoryQueryMutator<TQueryResult, ICollection<TOutputValue>>(
            innerMutator.Query,
            async () =>
            {
                var result = await innerMutator.ExecuteAsync();
                if (result == null) return null;

                return result.Select(i => i == null ? default(TOutputValue) : mapper(i)).ToList();
            });
    }

    /// <summary>
    /// Maps each item in the collection result of a query using the specified
    /// mapper function, returning a new collection of mapped items. The mapping 
    /// takes place after the original query has been executed. If the query
    /// result is <see langword="null"/> then mapping is skipped and <see langword="null"/>
    /// is returned.
    /// </summary>
    /// <typeparam name="TQueryResult">The type of the original query result.</typeparam>
    /// <typeparam name="TInputValue">The type of collection item to map from.</typeparam>
    /// <typeparam name="TOutputValue">The result item type after the mutation has been applied.</typeparam>
    /// <param name="innerMutator">The chained query mutator to run before this instance is applied.</param>
    /// <param name="mapper">
    /// An async mapper function to run on each item in the query result. If the item is <see langword="null"/>
    /// then mapping is skipped for that item and the <see langword="default"/> value of <typeparamref name="TOutputValue"/> 
    /// is returned instead.
    /// </param>
    /// <returns>A new query mutator instance that allows for method chaining.</returns>
    public static IDomainRepositoryQueryMutator<TQueryResult, ICollection<TOutputValue>> MapItem<TQueryResult, TInputValue, TOutputValue>(
        this IDomainRepositoryQueryMutator<TQueryResult, IEnumerable<TInputValue>> innerMutator,
        Func<TInputValue, Task<TOutputValue>> mapper
        )
    {
        return new DomainRepositoryQueryMutator<TQueryResult, ICollection<TOutputValue>>(
            innerMutator.Query,
            async () =>
            {
                var innerResult = await innerMutator.ExecuteAsync();
                if (innerResult == null) return null;

                var result = new List<TOutputValue>();

                foreach (var innerItem in innerResult)
                {
                    if (innerItem == null)
                    {
                        result.Add(default(TOutputValue));
                    }
                    else
                    {
                        var value = await mapper(innerItem);
                        result.Add(value);
                    }
                }

                return result;
            });
    }

    /// <summary>
    /// Maps each item in the dictionary result of a query using the specified
    /// mapper function, returning a new dictionary of mapped items. The mapping 
    /// takes place after the original query has been executed. If the query
    /// result is <see langword="null"/> then mapping is skipped and <see langword="null"/>
    /// is returned.
    /// 
    /// Be careful using per-item mapping with async tasks, as running async tasks
    /// per-item can affect performance. Always prefer mapping in batch to avoid
    /// performance issues.
    /// </summary>
    /// <typeparam name="TQueryResult">The type of the original query result.</typeparam>
    /// <typeparam name="TKey">The dictionary key type.</typeparam>
    /// <typeparam name="TInputValue">The type of value in the dictionary to map from.</typeparam>
    /// <typeparam name="TOutputValue">The type to map each value in the dictionary to.</typeparam>
    /// <param name="innerMutator">The chained query mutator to run before this instance is applied.</param>
    /// <param name="mapper">
    /// A mapper function to run on each value in the query result. Always prefer
    /// mapping in batch rather than per item if mapping requires data access or other
    /// time consuming tasks. If the item is <see langword="null"/>  then mapping is skipped for 
    /// that item and the <see langword="default"/> value of <typeparamref name="TOutputValue"/> 
    /// is returned instead.
    /// </param>
    /// <returns>A new query mutator instance that allows for method chaining.</returns>
    public static IDomainRepositoryQueryMutator<TQueryResult, IDictionary<TKey, TOutputValue>> MapItem<TQueryResult, TKey, TInputValue, TOutputValue>(
        this IDomainRepositoryQueryMutator<TQueryResult, IDictionary<TKey, TInputValue>> innerMutator,
        Func<TInputValue, TOutputValue> mapper
        )
    {
        return new DomainRepositoryQueryMutator<TQueryResult, IDictionary<TKey, TOutputValue>>(
            innerMutator.Query,
            async () =>
            {
                var result = await innerMutator.ExecuteAsync();
                if (result == null) return null;

                return result
                    .Select(i => new
                    {
                        Key = i.Key,
                        Value = i.Value == null ? default(TOutputValue) : mapper(i.Value)
                    }).ToDictionary(i => i.Key, i => i.Value);
            });
    }

    /// <summary>
    /// Maps each item in the dictionary result of a query using the specified
    /// mapper function, returning a new dictionary of mapped items. The mapping 
    /// takes place after the original query has been executed. If the query
    /// result is <see langword="null"/> then mapping is skipped and <see langword="null"/>
    /// is returned.
    /// </summary>
    /// <typeparam name="TQueryResult">The type of the original query result.</typeparam>
    /// <typeparam name="TKey">The dictionary key type.</typeparam>
    /// <typeparam name="TInputValue">The type of value in the dictionary to map from.</typeparam>
    /// <typeparam name="TOutputValue">The type to map each value in the dictionary to.</typeparam>
    /// <param name="innerMutator">The chained query mutator to run before this instance is applied.</param>
    /// <param name="mapper">
    /// An async mapper function to run on each item in the query result. If the item is <see langword="null"/>
    /// then mapping is skipped for that item and the <see langword="default"/> value of <typeparamref name="TOutputValue"/> 
    /// is returned instead.
    /// </param>
    /// <returns>A new query mutator instance that allows for method chaining.</returns>
    public static IDomainRepositoryQueryMutator<TQueryResult, IDictionary<TKey, TOutputValue>> MapItem<TQueryResult, TKey, TInputValue, TOutputValue>(
        this IDomainRepositoryQueryMutator<TQueryResult, IDictionary<TKey, TInputValue>> innerMutator,
        Func<TInputValue, Task<TOutputValue>> mapper
        )
    {
        return new DomainRepositoryQueryMutator<TQueryResult, IDictionary<TKey, TOutputValue>>(
            innerMutator.Query,
            async () =>
            {
                var innerResult = await innerMutator.ExecuteAsync();
                if (innerResult == null) return null;

                var result = new Dictionary<TKey, TOutputValue>(innerResult.Count);

                foreach (var innerItem in innerResult)
                {
                    if (innerItem.Value == null)
                    {
                        result.Add(innerItem.Key, default(TOutputValue));
                    }
                    else
                    {
                        var value = await mapper(innerItem.Value);
                        result.Add(innerItem.Key, value);
                    }
                }

                return result;
            });
    }

    /// <summary>
    /// Filters items in the dictionary result to only those with keys listed in the 
    /// orderedKeys collection, in the order they appear in that collection.
    /// Duplicates may be returned if the ordered keys collections contains 
    /// them.
    /// </summary>
    /// <typeparam name="TQueryResult">The type of the original query result.</typeparam>
    /// <typeparam name="TKey">The dictionary key type.</typeparam>
    /// <typeparam name="TValue">The dictionary value type.</typeparam>
    /// <param name="innerMutator">The chained query mutator to run before this instance is applied.</param>
    /// <param name="orderedKeys">
    /// A collection of dictionary keys in the order that you would like the results
    /// return in. Duplicate items may be returned if the orderedKeys collection 
    /// contains duplicates.
    /// </param>
    /// <returns>A new query mutator instance that allows for method chaining.</returns>
    public static IDomainRepositoryQueryMutator<TQueryResult, ICollection<TValue>> FilterAndOrderByKeys<TQueryResult, TKey, TValue>(
        this IDomainRepositoryQueryMutator<TQueryResult, IDictionary<TKey, TValue>> innerMutator,
        IEnumerable<TKey> orderedKeys
        )
    {
        return new DomainRepositoryQueryMutator<TQueryResult, ICollection<TValue>>(
            innerMutator.Query,
            async () =>
            {
                var result = await innerMutator.ExecuteAsync();
                if (result == null) return null;

                return result
                    .FilterAndOrderByKeys(orderedKeys)
                    .ToList();
            });
    }

    /// <summary>
    /// Maps each item in the paged result of a query using the specified
    /// mapper function, returning a new PagedQueryResult of mapped items. The mapping 
    /// takes place after the original query has been executed. If the query
    /// result is <see langword="null"/> then mapping is skipped and <see langword="null"/>
    /// is returned.
    /// </summary>
    /// <typeparam name="TQueryResult">The type of the original query result.</typeparam>
    /// <typeparam name="TInputValue">The type of item to map from.</typeparam>
    /// <typeparam name="TOutputValue">The result item type after the mutation has been applied.</typeparam>
    /// <param name="innerMutator">The chained query mutator to run before this instance is applied.</param>
    /// <param name="mapper">
    /// A mapper function to run on each item in the query result. If the item is <see langword="null"/>
    /// then mapping is skipped for that item and the <see langword="default"/> value of <typeparamref name="TOutputValue"/> 
    /// is returned instead.
    /// </param>
    /// <returns>A new query mutator instance that allows for method chaining.</returns>
    public static IDomainRepositoryQueryMutator<TQueryResult, PagedQueryResult<TOutputValue>> MapItem<TQueryResult, TInputValue, TOutputValue>(
        this IDomainRepositoryQueryMutator<TQueryResult, PagedQueryResult<TInputValue>> innerMutator,
        Func<TInputValue, TOutputValue> mapper
        )
    {
        return new DomainRepositoryQueryMutator<TQueryResult, PagedQueryResult<TOutputValue>>(
            innerMutator.Query,
            async () =>
            {
                var result = await innerMutator.ExecuteAsync();
                if (result == null) return null;

                var mappedItems = result
                    .Items
                    .Select(i => i == null ? default(TOutputValue) : mapper(i))
                    .ToList();

                return result.ChangeType(mappedItems);
            });
    }

    /// <summary>
    /// Maps each item in the paged result of a query using the specified
    /// mapper function, returning a new PagedQueryResult of mapped items. The mapping 
    /// takes place after the original query has been executed. If the query
    /// result is <see langword="null"/> then mapping is skipped and <see langword="null"/>
    /// is returned.
    /// </summary>
    /// <typeparam name="TQueryResult">The type of the original query result.</typeparam>
    /// <typeparam name="TInputValue">The type of item to map from.</typeparam>
    /// <typeparam name="TOutputValue">The result item type after the mutation has been applied.</typeparam>
    /// <param name="innerMutator">The chained query mutator to run before this instance is applied.</param>
    /// <param name="mapper">
    /// An async mapper function to run on each item in the query result. If the item is <see langword="null"/>
    /// then mapping is skipped for that item and the <see langword="default"/> value of <typeparamref name="TOutputValue"/> 
    /// is returned instead.
    /// </param>
    /// <returns>A new query mutator instance that allows for method chaining.</returns>
    public static IDomainRepositoryQueryMutator<TQueryResult, PagedQueryResult<TOutputValue>> MapItem<TQueryResult, TInputValue, TOutputValue>(
        this IDomainRepositoryQueryMutator<TQueryResult, PagedQueryResult<TInputValue>> innerMutator,
        Func<TInputValue, Task<TOutputValue>> mapper
        )
    {
        return new DomainRepositoryQueryMutator<TQueryResult, PagedQueryResult<TOutputValue>>(
            innerMutator.Query,
            async () =>
            {
                var innerResult = await innerMutator.ExecuteAsync();
                if (innerResult == null) return null;

                var mappedItems = new List<TOutputValue>(innerResult.Items.Count);

                foreach (var item in innerResult.Items)
                {
                    if (item == null)
                    {
                        mappedItems.Add(default(TOutputValue));
                    }
                    else
                    {
                        var mappedItem = await mapper(item);
                        mappedItems.Add(mappedItem);
                    }
                }

                return innerResult.ChangeType(mappedItems);
            });
    }
}
