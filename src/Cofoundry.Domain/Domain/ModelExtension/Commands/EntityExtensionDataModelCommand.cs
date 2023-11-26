namespace Cofoundry.Domain;

public class EntityExtensionDataModelCommand : Dictionary<string, IEntityExtensionDataModel>
{
    // TODO: should this be a collection / hashset? Maybe over JSON we continue
    // to use a dictionary and serialize to this, but make this a nice collection
    // to work with from a code-based API e.g. this.Set<MetaData>()
    // Maybe not - JS API shoud always look like key value, even if you don't use the jsonconverter?
    public EntityExtensionDataModelDictionary() : base(StringComparer.OrdinalIgnoreCase)
    {
        var command = new AddPageCommand();
        command.ExtensionData.Set<MetaDataDataModel>();
    }

    public TModel Get<TModel>() where TModel : IEntityExtensionDataModel
    {
        return Values
            .Where(v => v is TModel)
            .Cast<TModel>()
            .SingleOrDefault();
    }
}
