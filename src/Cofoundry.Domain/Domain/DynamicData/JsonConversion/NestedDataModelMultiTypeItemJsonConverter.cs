using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Cofoundry.Domain;

/// <summary>
/// JsonConverter for converting the NestedDataModelMultiTypeItem
/// objects used in the NestedDataModelMultiTypeCollection value.
/// </summary>
public class NestedDataModelMultiTypeItemJsonConverter : JsonConverter
{
    private readonly INestedDataModelTypeRepository _nestedDataModelTypeRepository;

    public NestedDataModelMultiTypeItemJsonConverter(
        INestedDataModelTypeRepository nestedDataModelTypeRepository
        )
    {
        _nestedDataModelTypeRepository = nestedDataModelTypeRepository;
    }

    public override bool CanConvert(Type objectType)
    {
        return typeof(NestedDataModelMultiTypeItem).IsAssignableFrom(objectType);
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        var json = JObject.Load(reader);
        var result = new NestedDataModelMultiTypeItem();

        var typeNameToken = json.GetValue(nameof(result.TypeName), StringComparison.OrdinalIgnoreCase);
        var typeName = typeNameToken?.Value<string>();
        var nestedDataModelType = _nestedDataModelTypeRepository.GetByName(typeName);

        if (nestedDataModelType == null)
        {
            return result;
        }

        result.TypeName = nestedDataModelType.Name;

        var modelDataToken = json.GetValue(nameof(result.Model), StringComparison.OrdinalIgnoreCase);
        if (modelDataToken == null)
        {
            return result;
        }

        using (var dataModelReader = modelDataToken.CreateReader())
        {
            result.Model = serializer.Deserialize(dataModelReader, nestedDataModelType) as INestedDataModel;
        }

        return result;
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }
}
