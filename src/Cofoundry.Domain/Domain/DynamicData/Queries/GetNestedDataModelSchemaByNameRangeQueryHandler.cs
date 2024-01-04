namespace Cofoundry.Domain;

public class GetNestedDataModelSchemaByNameRangeQueryHandler
    : IQueryHandler<GetNestedDataModelSchemaByNameRangeQuery, IReadOnlyDictionary<string, NestedDataModelSchema>>
    , IIgnorePermissionCheckHandler
{
    private readonly INestedDataModelSchemaMapper _nestedDataModelSchemaMapper;
    private readonly INestedDataModelTypeRepository _nestedDataModelRepository;

    public GetNestedDataModelSchemaByNameRangeQueryHandler(
        INestedDataModelSchemaMapper nestedDataModelSchemaMapper,
        INestedDataModelTypeRepository nestedDataModelRepository
        )
    {
        _nestedDataModelSchemaMapper = nestedDataModelSchemaMapper;
        _nestedDataModelRepository = nestedDataModelRepository;
    }

    public Task<IReadOnlyDictionary<string, NestedDataModelSchema>> ExecuteAsync(GetNestedDataModelSchemaByNameRangeQuery query, IExecutionContext executionContext)
    {
        IReadOnlyDictionary<string, NestedDataModelSchema> typedResult = ImmutableDictionary<string, NestedDataModelSchema>.Empty;

        if (EnumerableHelper.IsNullOrEmpty(query.Names))
        {
            return Task.FromResult(typedResult);
        }

        var resultBuilder = new Dictionary<string, NestedDataModelSchema>(StringComparer.OrdinalIgnoreCase);

        foreach (var name in query.Names.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            var dataModelType = _nestedDataModelRepository.GetByName(name);
            if (dataModelType != null)
            {
                var schema = _nestedDataModelSchemaMapper.Map(dataModelType);
                resultBuilder.Add(name, schema);
            }
        }

        typedResult = resultBuilder;

        return Task.FromResult(typedResult);
    }
}
