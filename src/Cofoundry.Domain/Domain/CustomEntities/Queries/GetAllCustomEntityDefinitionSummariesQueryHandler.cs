namespace Cofoundry.Domain.Internal;

/// <summary>
/// Query to get a collection of all custom entity definitions registered
/// with the system. The returned projections contain much of the same data 
/// as the source defintion class, but the main difference is that instead of 
/// using generics to identify the data model type, there is instead a 
/// DataModelType property.
/// </summary>
public class GetAllCustomEntityDefinitionSummariesQueryHandler
    : IQueryHandler<GetAllCustomEntityDefinitionSummariesQuery, ICollection<CustomEntityDefinitionSummary>>
    , IIgnorePermissionCheckHandler
{
    private readonly ICustomEntityDefinitionRepository _customEntityDefinitionRepository;
    private readonly ICustomEntityDefinitionSummaryMapper _customEntityDefinitionSummaryMapper;

    public GetAllCustomEntityDefinitionSummariesQueryHandler(
        ICustomEntityDefinitionRepository customEntityDefinitionRepository,
        ICustomEntityDefinitionSummaryMapper customEntityDefinitionSummaryMapper
        )
    {
        _customEntityDefinitionRepository = customEntityDefinitionRepository;
        _customEntityDefinitionSummaryMapper = customEntityDefinitionSummaryMapper;
    }

    public Task<ICollection<CustomEntityDefinitionSummary>> ExecuteAsync(GetAllCustomEntityDefinitionSummariesQuery query, IExecutionContext executionContext)
    {
        var results = _customEntityDefinitionRepository
             .GetAll()
             .Select(_customEntityDefinitionSummaryMapper.Map)
             .ToList();

        return Task.FromResult<ICollection<CustomEntityDefinitionSummary>>(results);
    }
}