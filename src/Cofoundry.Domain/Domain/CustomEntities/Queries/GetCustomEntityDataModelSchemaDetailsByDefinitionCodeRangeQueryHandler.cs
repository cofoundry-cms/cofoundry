﻿using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Query to extract and return meta data information about a custom 
/// entity data model for a range of custom entity definitions.
/// </summary>
public class GetCustomEntityDataModelSchemaDetailsByDefinitionCodeRangeQueryHandler
    : IQueryHandler<GetCustomEntityDataModelSchemaDetailsByDefinitionCodeRangeQuery, IReadOnlyDictionary<string, CustomEntityDataModelSchema>>
    , IPermissionRestrictedQueryHandler<GetCustomEntityDataModelSchemaDetailsByDefinitionCodeRangeQuery, IReadOnlyDictionary<string, CustomEntityDataModelSchema>>
{
    private readonly CofoundryDbContext _dbContext;
    private readonly IQueryExecutor _queryExecutor;
    private readonly IDynamicDataModelSchemaMapper _dynamicDataModelTypeMapper;
    private readonly ICustomEntityDefinitionRepository _customEntityDefinitionRepository;

    public GetCustomEntityDataModelSchemaDetailsByDefinitionCodeRangeQueryHandler(
        CofoundryDbContext dbContext,
        IQueryExecutor queryExecutor,
        IDynamicDataModelSchemaMapper dynamicDataModelTypeMapper,
        ICustomEntityDefinitionRepository customEntityDefinitionRepository
        )
    {
        _queryExecutor = queryExecutor;
        _dbContext = dbContext;
        _dynamicDataModelTypeMapper = dynamicDataModelTypeMapper;
        _customEntityDefinitionRepository = customEntityDefinitionRepository;
    }

    public async Task<IReadOnlyDictionary<string, CustomEntityDataModelSchema>> ExecuteAsync(GetCustomEntityDataModelSchemaDetailsByDefinitionCodeRangeQuery query, IExecutionContext executionContext)
    {
        var definitionQuery = new GetAllCustomEntityDefinitionSummariesQuery();
        var definitions = await _queryExecutor.ExecuteAsync(definitionQuery, executionContext);

        var results = new Dictionary<string, CustomEntityDataModelSchema>();

        foreach (var definition in definitions
            .Where(d => query.CustomEntityDefinitionCodes.Contains(d.CustomEntityDefinitionCode)))
        {
            var result = new CustomEntityDataModelSchema();
            result.CustomEntityDefinitionCode = definition.CustomEntityDefinitionCode;
            _dynamicDataModelTypeMapper.Map(result, definition.DataModelType);

            results.Add(definition.CustomEntityDefinitionCode, result);
        }

        return results;
    }

    public IEnumerable<IPermissionApplication> GetPermissions(GetCustomEntityDataModelSchemaDetailsByDefinitionCodeRangeQuery query)
    {
        foreach (var code in query.CustomEntityDefinitionCodes)
        {
            var definition = _customEntityDefinitionRepository.GetRequiredByCode(code);
            EntityNotFoundException.ThrowIfNull(definition, code);

            yield return new CustomEntityReadPermission(definition);
        }
    }
}
