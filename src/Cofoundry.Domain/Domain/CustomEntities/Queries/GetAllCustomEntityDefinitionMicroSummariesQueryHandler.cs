namespace Cofoundry.Domain.Internal;

/// <summary>
/// Query to get a collection of all custom entity definitions registered
/// with the system. The returned object is a lightweight projection of the
/// data defined in a custom entity definition class which is typically used 
/// as part of another domain model.
/// </summary>
public class GetAllCustomEntityDefinitionMicroSummariesQueryHandler
    : IQueryHandler<GetAllCustomEntityDefinitionMicroSummariesQuery, IReadOnlyCollection<CustomEntityDefinitionMicroSummary>>
    , IIgnorePermissionCheckHandler
{
    private readonly ICustomEntityDefinitionRepository _customEntityDefinitionRepository;
    private readonly ICustomEntityDefinitionMicroSummaryMapper _customEntityDefinitionMicroSummaryMapper;

    public GetAllCustomEntityDefinitionMicroSummariesQueryHandler(
        ICustomEntityDefinitionRepository customEntityDefinitionRepository,
        ICustomEntityDefinitionMicroSummaryMapper customEntityDefinitionMicroSummaryMapper
        )
    {
        _customEntityDefinitionRepository = customEntityDefinitionRepository;
        _customEntityDefinitionMicroSummaryMapper = customEntityDefinitionMicroSummaryMapper;
    }

    public Task<IReadOnlyCollection<CustomEntityDefinitionMicroSummary>> ExecuteAsync(GetAllCustomEntityDefinitionMicroSummariesQuery query, IExecutionContext executionContext)
    {
        var result = _customEntityDefinitionRepository
            .GetAll()
            .Select(_customEntityDefinitionMicroSummaryMapper.Map)
            .ToArray();

        return Task.FromResult<IReadOnlyCollection<CustomEntityDefinitionMicroSummary>>(result);
    }
}
