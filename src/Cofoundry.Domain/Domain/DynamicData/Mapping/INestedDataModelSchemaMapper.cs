namespace Cofoundry.Domain;

public interface INestedDataModelSchemaMapper
{
    NestedDataModelSchema Map(Type modelType);
}
