using Cofoundry.Domain.Data;

namespace Cofoundry.Domain;

public interface IEntityExtensionDataModelDictionaryMapper
{
    EntityExtensionDataModelDictionary Map(IDataExtendable entity, EntityExtensionLoadProfile loadProfile, IEnumerable<ExtensionRegistrationOptions> options);
}
