namespace Cofoundry.Domain;

public class EntityExtensionDataModelDictionary : Dictionary<string, IEntityExtensionDataModel>
{
    public EntityExtensionDataModelDictionary() : base(StringComparer.OrdinalIgnoreCase)
    {
    }

    public TModel Get<TModel>() where TModel : IEntityExtensionDataModel
    {
        return Values
            .Where(v => v is TModel)
            .Cast<TModel>()
            .SingleOrDefault();
    }
}
