﻿using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Returns all page directories as PageDirectoryRoute objects. The results of this 
/// query are cached.
/// </summary>
public class GetAllPageDirectoryRoutesQueryHandler
    : IQueryHandler<GetAllPageDirectoryRoutesQuery, IReadOnlyCollection<PageDirectoryRoute>>
    , IPermissionRestrictedQueryHandler<GetAllPageDirectoryRoutesQuery, IReadOnlyCollection<PageDirectoryRoute>>
{
    private readonly CofoundryDbContext _dbContext;
    private readonly IPageDirectoryRouteMapper _pageDirectoryRouteMapper;

    public GetAllPageDirectoryRoutesQueryHandler(
        CofoundryDbContext dbContext,
        IPageDirectoryRouteMapper pageDirectoryRouteMapper
        )
    {
        _dbContext = dbContext;
        _pageDirectoryRouteMapper = pageDirectoryRouteMapper;
    }

    public async Task<IReadOnlyCollection<PageDirectoryRoute>> ExecuteAsync(GetAllPageDirectoryRoutesQuery query, IExecutionContext executionContext)
    {
        var dbPageDirectories = await _dbContext
            .PageDirectories
            .AsNoTracking()
            .Include(d => d.AccessRules)
            .Include(d => d.PageDirectoryLocales)
            .ToArrayAsync();

        var activeWebRoutes = _pageDirectoryRouteMapper.Map(dbPageDirectories);

        return activeWebRoutes;
    }

    public IEnumerable<IPermissionApplication> GetPermissions(GetAllPageDirectoryRoutesQuery command)
    {
        yield return new PageDirectoryReadPermission();
    }
}
