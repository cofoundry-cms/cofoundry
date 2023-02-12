using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Cofoundry.Domain;

public class EntityExtensionDataModelDictionaryJsonConverter : JsonConverter
{
    private readonly IReadOnlyCollection<ExtensionRegistrationOptions> _extensionRegistrationOptions;

    public EntityExtensionDataModelDictionaryJsonConverter(
        IReadOnlyCollection<ExtensionRegistrationOptions> extensionRegistrationOptions
        )
    {
        _extensionRegistrationOptions = extensionRegistrationOptions;
    }

    public override bool CanConvert(Type objectType)
    {
        return typeof(EntityExtensionDataModelDictionary).IsAssignableFrom(objectType);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var result = new EntityExtensionDataModelDictionary();

        var json = JObject.Load(reader);
        if (json == null) return result;

        var optionLookup = _extensionRegistrationOptions.ToDictionary(o => o.Name, StringComparer.OrdinalIgnoreCase);

        foreach (var data in json.Properties())
        {
            var option = optionLookup.GetOrDefault(data.Name);
            if (option == null)
            {
                continue;
            }

            using (var dataModelReader = data.Value.CreateReader())
            {
                var model = serializer.Deserialize(dataModelReader, option.Type) as IEntityExtensionDataModel;
                if (model != null)
                {
                    result.Add(option.Name, model);
                }
            }
        }

        return result;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        serializer.Serialize(writer, value);
    }
}
