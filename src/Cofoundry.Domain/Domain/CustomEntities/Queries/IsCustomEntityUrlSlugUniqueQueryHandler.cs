﻿using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Determines if the UrlSlug property for a custom entity is invalid because it
/// already exists. If the custom entity defition has ForceUrlSlugUniqueness 
/// set to true then duplicates are not permitted, otherwise true will always
/// be returned.
/// </summary>
public class IsCustomEntityUrlSlugUniqueQueryHandler
    : IQueryHandler<IsCustomEntityUrlSlugUniqueQuery, bool>
    , IPermissionRestrictedQueryHandler<IsCustomEntityUrlSlugUniqueQuery, bool>
{
    private readonly CofoundryDbContext _dbContext;
    private readonly IQueryExecutor _queryExecutor;
    private readonly ICustomEntityDefinitionRepository _customEntityDefinitionRepository;

    public IsCustomEntityUrlSlugUniqueQueryHandler(
        CofoundryDbContext dbContext,
        IQueryExecutor queryExecutor,
        ICustomEntityDefinitionRepository customEntityDefinitionRepository
        )
    {
        _dbContext = dbContext;
        _queryExecutor = queryExecutor;
        _customEntityDefinitionRepository = customEntityDefinitionRepository;
    }

    public async Task<bool> ExecuteAsync(IsCustomEntityUrlSlugUniqueQuery query, IExecutionContext executionContext)
    {
        if (string.IsNullOrWhiteSpace(query.UrlSlug))
        {
            return true;
        }

        var definition = await GetDefinitionAsync(query, executionContext);
        if (!definition.ForceUrlSlugUniqueness)
        {
            return true;
        }

        var dbQuery = Query(query);
        var exists = await dbQuery.AnyAsync();

        return !exists;
    }

    private IQueryable<CustomEntity> Query(IsCustomEntityUrlSlugUniqueQuery query)
    {
        var dbQuery = _dbContext
            .CustomEntities
            .AsNoTracking()
            .Where(d => d.CustomEntityId != query.CustomEntityId
                && d.CustomEntityDefinitionCode == query.CustomEntityDefinitionCode
                && d.UrlSlug == query.UrlSlug
                && d.LocaleId == query.LocaleId
                );

        return dbQuery;
    }

    private async Task<CustomEntityDefinitionSummary> GetDefinitionAsync(
        IsCustomEntityUrlSlugUniqueQuery query,
        IExecutionContext executionContext
        )
    {
        var definitionQuery = new GetCustomEntityDefinitionSummaryByCodeQuery(query.CustomEntityDefinitionCode);
        var definition = await _queryExecutor.ExecuteAsync(definitionQuery, executionContext);
        EntityNotFoundException.ThrowIfNull(definition, query.CustomEntityDefinitionCode);

        return definition;
    }

    public IEnumerable<IPermissionApplication> GetPermissions(IsCustomEntityUrlSlugUniqueQuery query)
    {
        var definition = _customEntityDefinitionRepository.GetRequiredByCode(query.CustomEntityDefinitionCode);
        yield return new CustomEntityReadPermission(definition);
    }
}

