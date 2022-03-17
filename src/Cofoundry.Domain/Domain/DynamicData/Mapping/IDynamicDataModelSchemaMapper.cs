namespace Cofoundry.Domain.Internal;

public interface IDynamicDataModelSchemaMapper
{
    void Map(IDynamicDataModelSchema details, Type modelType);
}
