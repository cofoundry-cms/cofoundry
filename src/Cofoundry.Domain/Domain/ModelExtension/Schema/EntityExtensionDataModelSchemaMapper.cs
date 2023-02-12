namespace Cofoundry.Domain.Internal;

public class EntityExtensionDataModelSchemaMapper : IEntityExtensionDataModelSchemaMapper
{
    private readonly IDynamicDataModelSchemaMapper _dynamicDataModelSchemaMapper;

    public EntityExtensionDataModelSchemaMapper(
        IDynamicDataModelSchemaMapper dynamicDataModelSchemaMapper
        )
    {
        _dynamicDataModelSchemaMapper = dynamicDataModelSchemaMapper;
    }

    public EntityExtensionDataModelSchema Map(ExtensionRegistrationOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var schema = new EntityExtensionDataModelSchema();
        schema.Name = options.Name;
        schema.GroupName = options.GroupName;

        _dynamicDataModelSchemaMapper.Map(schema, options.Type);

        return schema;
    }
}
