using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

public class CustomEntityDataModelMapper : ICustomEntityDataModelMapper
{
    private readonly IEnumerable<ICustomEntityDefinition> _customEntityDefinitions;
    private readonly IEmptyDataModelFactory _emptyDataModelFactory;
    private readonly IDbUnstructuredDataSerializer _dbUnstructuredDataSerializer;

    public CustomEntityDataModelMapper(
        IDbUnstructuredDataSerializer dbUnstructuredDataSerializer,
        IEnumerable<ICustomEntityDefinition> customEntityDefinitions,
        IEmptyDataModelFactory emptyDataModelFactory
        )
    {
        _dbUnstructuredDataSerializer = dbUnstructuredDataSerializer;
        _customEntityDefinitions = customEntityDefinitions;
        _emptyDataModelFactory = emptyDataModelFactory;
    }

    public ICustomEntityDataModel Map(string customEntityDefinitionCode, string serializedData)
    {
        var definition = _customEntityDefinitions.SingleOrDefault(d => d.CustomEntityDefinitionCode == customEntityDefinitionCode);
        EntityNotFoundException.ThrowIfNull(definition, customEntityDefinitionCode);

        var dataModelType = definition.GetDataModelType();

        var deserialized = _dbUnstructuredDataSerializer.Deserialize(serializedData, dataModelType) as ICustomEntityDataModel;

        if (deserialized != null)
        {
            return deserialized;
        }

        var emptyInstance = _emptyDataModelFactory.Create<ICustomEntityDataModel>(dataModelType);

        return emptyInstance;
    }
}
