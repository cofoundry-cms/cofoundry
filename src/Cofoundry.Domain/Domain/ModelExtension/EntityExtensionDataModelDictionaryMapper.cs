using Cofoundry.Domain.Data;

namespace Cofoundry.Domain;

public class EntityExtensionDataModelDictionaryMapper : IEntityExtensionDataModelDictionaryMapper
{
    private readonly IDbUnstructuredDataSerializer _dbUnstructuredDataSerializer;

    public EntityExtensionDataModelDictionaryMapper(
        IDbUnstructuredDataSerializer dbUnstructuredDataSerializer
        )
    {
        _dbUnstructuredDataSerializer = dbUnstructuredDataSerializer;
    }

    public EntityExtensionDataModelDictionary Map(IDataExtendable entity, EntityExtensionLoadProfile loadProfile, IEnumerable<ExtensionRegistrationOptions> options)
    {
        var extensionData = new EntityExtensionDataModelDictionary();
        if (string.IsNullOrWhiteSpace(entity.SerializedExtensionData))
        {
            return extensionData;
        }

        var dataDictionary = _dbUnstructuredDataSerializer.Deserialize<Dictionary<string, object>>(entity.SerializedExtensionData);
        if (dataDictionary == null) return extensionData;

        // TODO: Can this be replaced with EntityExtensionDataModelDictionaryJsonConverter?
        var optionLookup = options.ToDictionary(o => o.Name, StringComparer.OrdinalIgnoreCase);

        foreach (var data in dataDictionary)
        {
            var option = optionLookup.GetOrDefault(data.Key);
            if (option == null || option.LoadProfile < loadProfile)
            {
                continue;
            }

            var dataModel = _dbUnstructuredDataSerializer.Deserialize(data.Value.ToString(), option.Type) as IEntityExtensionDataModel;
            if (dataModel != null)
            {
                extensionData.Add(option.Name, dataModel);
            }
        }

        return extensionData;
    }
}
