namespace Cofoundry.Domain.Internal;

public class GetNestedDataModelSchemaByNameQueryHandler
    : IQueryHandler<GetNestedDataModelSchemaByNameQuery, NestedDataModelSchema>
    , IIgnorePermissionCheckHandler
{
    private readonly INestedDataModelSchemaMapper _nestedDataModelSchemaMapper;
    private readonly INestedDataModelTypeRepository _nestedDataModelRepository;

    public GetNestedDataModelSchemaByNameQueryHandler(
        INestedDataModelSchemaMapper nestedDataModelSchemaMapper,
        INestedDataModelTypeRepository nestedDataModelRepository
        )
    {
        _nestedDataModelSchemaMapper = nestedDataModelSchemaMapper;
        _nestedDataModelRepository = nestedDataModelRepository;
    }

    public Task<NestedDataModelSchema> ExecuteAsync(GetNestedDataModelSchemaByNameQuery query, IExecutionContext executionContext)
    {
        NestedDataModelSchema result = null;

        if (string.IsNullOrWhiteSpace(query.Name)) return Task.FromResult(result);

        var dataModelType = _nestedDataModelRepository.GetByName(query.Name);

        if (dataModelType == null) return Task.FromResult(result);

        result = _nestedDataModelSchemaMapper.Map(dataModelType);

        return Task.FromResult(result);
    }
}