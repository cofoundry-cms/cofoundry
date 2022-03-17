using Newtonsoft.Json;

namespace Cofoundry.Domain;

public interface IEntityDataModelJsonConverterFactory
{
    JsonConverter Create(Type dataModelType);
}
