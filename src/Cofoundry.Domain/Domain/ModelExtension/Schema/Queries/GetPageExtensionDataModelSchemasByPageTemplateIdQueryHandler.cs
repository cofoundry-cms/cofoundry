namespace Cofoundry.Domain.Internal;

public class GetPageExtensionDataModelSchemasByPageTemplateIdQueryHandler
    : IQueryHandler<GetPageExtensionDataModelSchemasByPageTemplateIdQuery, ICollection<EntityExtensionDataModelSchema>>
    , IIgnorePermissionCheckHandler
{
    private readonly IEntityExtensionDataModelSchemaMapper _entityExtensionDataModelSchemaMapper;
    private readonly IPageModelExtensionConfigurationRepository _pageModelExtensionConfigurationRepository;

    public GetPageExtensionDataModelSchemasByPageTemplateIdQueryHandler(
        IEntityExtensionDataModelSchemaMapper entityExtensionDataModelSchemaMapper,
        IPageModelExtensionConfigurationRepository pageModelExtensionConfigurationRepository
        )
    {
        _entityExtensionDataModelSchemaMapper = entityExtensionDataModelSchemaMapper;
        _pageModelExtensionConfigurationRepository = pageModelExtensionConfigurationRepository;
    }

    public Task<ICollection<EntityExtensionDataModelSchema>> ExecuteAsync(GetPageExtensionDataModelSchemasByPageTemplateIdQuery query, IExecutionContext executionContext)
    {
        var options = _pageModelExtensionConfigurationRepository.GetByTemplateId(query.PageTemplateId);

        // TODO: maybe need an ordering?
        var result = options.Select(o => _entityExtensionDataModelSchemaMapper.Map(o)).ToArray();
        return Task.FromResult<ICollection<EntityExtensionDataModelSchema>>(result);
    }
}