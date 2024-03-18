﻿namespace Cofoundry.Domain.Internal;

/// <summary>
/// Query to get a custom entity definition by it's unique 6 character code.
/// The returned object is a lightweight projection of the data defined in a custom entity 
/// definition class. This is typically used as part of another domain model or
/// for querying lists of definitions in the admin panel.
/// </summary>
public class GetCustomEntityDefinitionMicroSummaryByCodeQueryHandler
    : IQueryHandler<GetCustomEntityDefinitionMicroSummaryByCodeQuery, CustomEntityDefinitionMicroSummary?>
    , IIgnorePermissionCheckHandler
{
    private readonly IEnumerable<ICustomEntityDefinition> _customEntityRegistrations;
    private readonly ICustomEntityDefinitionMicroSummaryMapper _customEntityDefinitionMicroSummaryMapper;

    public GetCustomEntityDefinitionMicroSummaryByCodeQueryHandler(
        IEnumerable<ICustomEntityDefinition> customEntityRegistrations,
        ICustomEntityDefinitionMicroSummaryMapper customEntityDefinitionMicroSummaryMapper
        )
    {
        _customEntityRegistrations = customEntityRegistrations;
        _customEntityDefinitionMicroSummaryMapper = customEntityDefinitionMicroSummaryMapper;
    }

    public Task<CustomEntityDefinitionMicroSummary?> ExecuteAsync(GetCustomEntityDefinitionMicroSummaryByCodeQuery query, IExecutionContext executionContext)
    {
        var definition = _customEntityRegistrations.SingleOrDefault(d => d.CustomEntityDefinitionCode == query.CustomEntityDefinitionCode);

        CustomEntityDefinitionMicroSummary? result = null;

        if (definition != null)
        {
            result = _customEntityDefinitionMicroSummaryMapper.Map(definition);
        }

        return Task.FromResult(result);
    }
}