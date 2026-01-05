namespace Cofoundry.Domain;

/// <summary>
/// Content repository extensions that allow a user to mutate the results of a query
/// after it is executed.
/// </summary>
public static class IDomainRepositoryQueryMutatorExtensions
{
    extension<TQueryResult, TInput>(IDomainRepositoryQueryMutator<TQueryResult, TInput> innerMutator)
    {
        /// <summary>
        /// Maps the result of the query using the specified mapper function. The 
        /// mapping takes place after the original query has been executed. 
        /// </summary>
        /// <typeparam name="TOutput">The result type after the mutation has been applied.</typeparam>
        /// <param name="mapper">A mapper function to run on the query result.</param>
        /// <returns>A new query mutator instance that allows for method chaining.</returns>
        public IDomainRepositoryQueryMutator<TQueryResult, TOutput?> Map<TOutput>(Func<TInput, TOutput?> mapper)
        {
            return new DomainRepositoryQueryMutator<TQueryResult, TOutput?>(
                innerMutator.Query,
                async () =>
                {
                    var result = await innerMutator.ExecuteAsync();

                    return mapper(result);
                });
        }

        /// <summary>
        /// Maps the result of the query using the specified mapper function. The 
        /// mapping takes place after the original query has been executed. If the query
        /// result is <see langword="null"/> then mapping is skipped and <see langword="null"/>
        /// is returned.
        /// </summary>
        /// <typeparam name="TOutput">The result type after the mutation has been applied.</typeparam>
        /// <param name="mapper">A mapper function to run on the query result.</param>
        /// <returns>A new query mutator instance that allows for method chaining.</returns>
        public IDomainRepositoryQueryMutator<TQueryResult, TOutput?> MapWhenNotNull<TOutput>(Func<TInput, TOutput?> mapper)
        {
            return new DomainRepositoryQueryMutator<TQueryResult, TOutput?>(
                innerMutator.Query,
                async () =>
                {
                    var result = await innerMutator.ExecuteAsync();
                    if (result == null)
                    {
                        return default;
                    }

                    return mapper(result);
                });
        }

        /// <summary>
        /// Maps the result of the query using the specified mapper function. The 
        /// mapping takes place after the original query has been executed.
        /// </summary>
        /// <typeparam name="TOutput">The result type after the mutation has been applied.</typeparam>
        /// <param name="mapper">An async mapper function to run on the query result.</param>
        /// <returns>A new query mutator instance that allows for method chaining.</returns>
        public IDomainRepositoryQueryMutator<TQueryResult, TOutput?> Map<TOutput>(Func<TInput, Task<TOutput?>> mapper)
        {
            return new DomainRepositoryQueryMutator<TQueryResult, TOutput?>(
                innerMutator.Query,
                async () =>
                {
                    var result = await innerMutator.ExecuteAsync();

                    return await mapper(result);
                });
        }

        /// <summary>
        /// Maps the result of the query using the specified mapper function. The 
        /// mapping takes place after the original query has been executed. If the query
        /// result is <see langword="null"/> then mapping is skipped and the default value
        /// of <typeparamref name="TOutput"/> is returned.
        /// </summary>
        /// <typeparam name="TOutput">The result type after the mutation has been applied.</typeparam>
        /// <param name="mapper">An async mapper function to run on the query result.</param>
        /// <returns>A new query mutator instance that allows for method chaining.</returns>
        public IDomainRepositoryQueryMutator<TQueryResult, TOutput?> MapWhenNotNull<TOutput>(Func<TInput, Task<TOutput?>> mapper)
        {
            return new DomainRepositoryQueryMutator<TQueryResult, TOutput?>(
                innerMutator.Query,
                async () =>
                {
                    var result = await innerMutator.ExecuteAsync();
                    if (result == null)
                    {
                        return default;
                    }

                    return await mapper(result);
                });
        }
    }

    extension<TQueryResult, TInputValue>(IDomainRepositoryQueryMutator<TQueryResult, IReadOnlyCollection<TInputValue>> innerMutator)
    {
        /// <summary>
        /// Maps each item in the collection result of a query using the specified
        /// mapper function, returning a new collection of mapped items. The mapping 
        /// takes place after the original query has been executed.
        /// </summary>
        /// <typeparam name="TOutputValue">The result item type after the mutation has been applied.</typeparam>
        /// <param name="mapper">
        /// A mapper function to run on each item in the query result.
        /// </param>
        /// <returns>A new query mutator instance that allows for method chaining.</returns>
        public IDomainRepositoryQueryMutator<TQueryResult, IReadOnlyCollection<TOutputValue>> MapItem<TOutputValue>(Func<TInputValue, TOutputValue> mapper)
        {
            return new DomainRepositoryQueryMutator<TQueryResult, IReadOnlyCollection<TOutputValue>>(
                innerMutator.Query,
                async () =>
                {
                    var result = await innerMutator.ExecuteAsync();

                    return EnumerableHelper
                        .Enumerate(result)
                        .Select(i => mapper(i))
                        .ToArray();
                });
        }
    }

    extension<TQueryResult, TInputValue>(IDomainRepositoryQueryMutator<TQueryResult, IEnumerable<TInputValue>> innerMutator)
    {
        /// <summary>
        /// Maps each item in the collection result of a query using the specified
        /// mapper function, returning a new collection of mapped items. The mapping 
        /// takes place after the original query has been executed.
        /// </summary>
        /// <typeparam name="TOutputValue">The result item type after the mutation has been applied.</typeparam>
        /// <param name="mapper">
        /// A mapper function to run on each item in the query result.
        /// </param>
        /// <returns>A new query mutator instance that allows for method chaining.</returns>
        public IDomainRepositoryQueryMutator<TQueryResult, IReadOnlyCollection<TOutputValue>> MapItem<TOutputValue>(Func<TInputValue, TOutputValue> mapper)
        {
            return new DomainRepositoryQueryMutator<TQueryResult, IReadOnlyCollection<TOutputValue>>(
                innerMutator.Query,
                async () =>
                {
                    var result = await innerMutator.ExecuteAsync();

                    return EnumerableHelper
                        .Enumerate(result)
                        .Select(i => mapper(i))
                        .ToList();
                });
        }

        /// <summary>
        /// Maps each item in the collection result of a query using the specified
        /// mapper function, returning a new collection of mapped items. The mapping 
        /// takes place after the original query has been executed.
        /// </summary>
        /// <typeparam name="TOutputValue">The result item type after the mutation has been applied.</typeparam>
        /// <param name="mapper">
        /// An async mapper function to run on each item in the query result. If the item is <see langword="null"/>
        /// then mapping is skipped for that item and the <see langword="default"/> value of <typeparamref name="TOutputValue"/> 
        /// is returned instead.
        /// </param>
        /// <returns>A new query mutator instance that allows for method chaining.</returns>
        public IDomainRepositoryQueryMutator<TQueryResult, IReadOnlyCollection<TOutputValue>> MapItem<TOutputValue>(Func<TInputValue, Task<TOutputValue>> mapper)
        {
            return new DomainRepositoryQueryMutator<TQueryResult, IReadOnlyCollection<TOutputValue>>(
                innerMutator.Query,
                async () =>
                {
                    var innerResult = await innerMutator.ExecuteAsync();

                    var result = new List<TOutputValue>();

                    foreach (var innerItem in EnumerableHelper.Enumerate(innerResult))
                    {
                        var value = await mapper(innerItem);
                        result.Add(value);
                    }

                    return result;
                });
        }
    }

    extension<TQueryResult, TKey, TInputValue>(IDomainRepositoryQueryMutator<TQueryResult, IReadOnlyDictionary<TKey, TInputValue>> innerMutator) where TKey : notnull
    {
        /// <summary>
        /// <para>
        /// Maps each item in the dictionary result of a query using the specified
        /// mapper function, returning a new dictionary of mapped items. The mapping 
        /// takes place after the original query has been executed.
        /// </para>
        /// <para>
        /// Be careful using per-item mapping with async tasks, as running async tasks
        /// per-item can affect performance. Always prefer mapping in batch to avoid
        /// performance issues.
        /// </para>
        /// </summary>
        /// <typeparam name="TOutputValue">The type to map each value in the dictionary to.</typeparam>
        /// <param name="mapper">
        /// A mapper function to run on each value in the query result. Always prefer
        /// mapping in batch rather than per item if mapping requires data access or other
        /// time consuming tasks.
        /// </param>
        /// <returns>A new query mutator instance that allows for method chaining.</returns>
        public IDomainRepositoryQueryMutator<TQueryResult, IReadOnlyDictionary<TKey, TOutputValue>> MapItem<TOutputValue>(Func<TInputValue, TOutputValue> mapper)
        {
            return new DomainRepositoryQueryMutator<TQueryResult, IReadOnlyDictionary<TKey, TOutputValue>>(
                innerMutator.Query,
                async () =>
                {
                    var result = await innerMutator.ExecuteAsync();

                    return EnumerableHelper
                        .Enumerate(result)
                        .Select(i => new
                        {
                            i.Key,
                            Value = mapper(i.Value)
                        }).ToImmutableDictionary(i => i.Key, i => i.Value);
                });
        }

        /// <summary>
        /// Maps each item in the dictionary result of a query using the specified
        /// mapper function, returning a new dictionary of mapped items. The mapping 
        /// takes place after the original query has been executed.
        /// </summary>
        /// <typeparam name="TOutputValue">The type to map each value in the dictionary to.</typeparam>
        /// <param name="mapper">
        /// An async mapper function to run on each item in the query result.
        /// </param>
        /// <returns>A new query mutator instance that allows for method chaining.</returns>
        public IDomainRepositoryQueryMutator<TQueryResult, IReadOnlyDictionary<TKey, TOutputValue>> MapItem<TOutputValue>(Func<TInputValue, Task<TOutputValue>> mapper)
        {
            return new DomainRepositoryQueryMutator<TQueryResult, IReadOnlyDictionary<TKey, TOutputValue>>(
                innerMutator.Query,
                async () =>
                {
                    var innerResult = await innerMutator.ExecuteAsync();

                    var result = new Dictionary<TKey, TOutputValue>(innerResult?.Count ?? 0);

                    foreach (var innerItem in EnumerableHelper.Enumerate(innerResult))
                    {
                        var value = await mapper(innerItem.Value);
                        result.Add(innerItem.Key, value);
                    }

                    return result;
                });
        }
    }

    extension<TQueryResult, TKey, TValue>(IDomainRepositoryQueryMutator<TQueryResult, IReadOnlyDictionary<TKey, TValue>> innerMutator)
    {
        /// <summary>
        /// Filters items in the dictionary result to only those with keys listed in the 
        /// orderedKeys collection, in the order they appear in that collection.
        /// Duplicates may be returned if the ordered keys collections contains 
        /// them.
        /// </summary>
        /// <param name="orderedKeys">
        /// A collection of dictionary keys in the order that you would like the results
        /// return in. Duplicate items may be returned if the orderedKeys collection 
        /// contains duplicates.
        /// </param>
        /// <returns>A new query mutator instance that allows for method chaining.</returns>
        public IDomainRepositoryQueryMutator<TQueryResult, IReadOnlyCollection<TValue>> FilterAndOrderByKeys(IEnumerable<TKey> orderedKeys)
        {
            return new DomainRepositoryQueryMutator<TQueryResult, IReadOnlyCollection<TValue>>(
                innerMutator.Query,
                async () =>
                {
                    var result = await innerMutator.ExecuteAsync();
                    if (result == null)
                    {
                        return new List<TValue>();
                    }

                    return result
                        .FilterAndOrderByKeys(orderedKeys)
                        .ToArray();
                });
        }
    }

    extension<TQueryResult, TInputValue>(IDomainRepositoryQueryMutator<TQueryResult, PagedQueryResult<TInputValue>> innerMutator)
    {
        /// <summary>
        /// Maps each item in the paged result of a query using the specified
        /// mapper function, returning a new PagedQueryResult of mapped items. The mapping 
        /// takes place after the original query has been executed.
        /// </summary>
        /// <typeparam name="TOutputValue">The result item type after the mutation has been applied.</typeparam>
        /// <param name="mapper">
        /// A mapper function to run on each item in the query result.
        /// </param>
        /// <returns>A new query mutator instance that allows for method chaining.</returns>
        public IDomainRepositoryQueryMutator<TQueryResult, PagedQueryResult<TOutputValue>> MapItem<TOutputValue>(Func<TInputValue, TOutputValue> mapper)
        {
            return new DomainRepositoryQueryMutator<TQueryResult, PagedQueryResult<TOutputValue>>(
                innerMutator.Query,
                async () =>
                {
                    var result = await innerMutator.ExecuteAsync();

                    var mappedItems = EnumerableHelper.Enumerate(result.Items)
                        .Select(i => mapper(i))
                        .ToArray();

                    return result.ChangeType(mappedItems);
                });
        }

        /// <summary>
        /// Maps each item in the paged result of a query using the specified
        /// mapper function, returning a new PagedQueryResult of mapped items. The mapping 
        /// takes place after the original query has been executed.
        /// </summary>
        /// <typeparam name="TOutputValue">The result item type after the mutation has been applied.</typeparam>
        /// <param name="mapper">
        /// An async mapper function to run on each item in the query result.
        /// </param>
        /// <returns>A new query mutator instance that allows for method chaining.</returns>
        public IDomainRepositoryQueryMutator<TQueryResult, PagedQueryResult<TOutputValue>> MapItem<TOutputValue>(Func<TInputValue, Task<TOutputValue>> mapper)
        {
            return new DomainRepositoryQueryMutator<TQueryResult, PagedQueryResult<TOutputValue>>(
                innerMutator.Query,
                async () =>
                {
                    var innerResult = await innerMutator.ExecuteAsync();

                    var mappedItems = new List<TOutputValue>(innerResult.Items.Count);

                    foreach (var item in innerResult.Items)
                    {
                        var mappedItem = await mapper(item);
                        mappedItems.Add(mappedItem);
                    }

                    return innerResult.ChangeType(mappedItems);
                });
        }
    }
}
