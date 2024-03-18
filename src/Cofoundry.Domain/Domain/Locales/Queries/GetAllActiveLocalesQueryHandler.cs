﻿using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

public class GetAllActiveLocalesQueryHandler
    : IQueryHandler<GetAllActiveLocalesQuery, IReadOnlyCollection<ActiveLocale>>
    , IIgnorePermissionCheckHandler
{
    private readonly CofoundryDbContext _dbContext;
    private readonly ILocaleCache _cache;
    private readonly IActiveLocaleMapper _activeLocaleMapper;

    public GetAllActiveLocalesQueryHandler(
        CofoundryDbContext dbContext,
        ILocaleCache cache,
        IActiveLocaleMapper activeLocaleMapper
        )
    {
        _dbContext = dbContext;
        _cache = cache;
        _activeLocaleMapper = activeLocaleMapper;
    }

    public async Task<IReadOnlyCollection<ActiveLocale>> ExecuteAsync(GetAllActiveLocalesQuery query, IExecutionContext executionContext)
    {
        var results = await _cache.GetOrAddAsync(new Func<Task<IReadOnlyCollection<ActiveLocale>>>(async () =>
        {
            var dbResults = await GetAllLocales().ToListAsync();
            var mappedResults = dbResults
                .Select(_activeLocaleMapper.Map)
                .WhereNotNull()
                .ToArray();

            return mappedResults;
        }));

        return results;
    }

    private IQueryable<Locale> GetAllLocales()
    {
        return _dbContext
            .Locales
            .AsNoTracking()
            .Where(l => l.IsActive)
            .OrderBy(l => l.LocaleName);
    }
}
