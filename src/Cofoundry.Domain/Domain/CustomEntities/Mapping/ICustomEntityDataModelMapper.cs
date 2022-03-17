namespace Cofoundry.Domain.Internal;

public interface ICustomEntityDataModelMapper
{
    ICustomEntityDataModel Map(string customEntityDefinitionCode, string serializedData);
}
