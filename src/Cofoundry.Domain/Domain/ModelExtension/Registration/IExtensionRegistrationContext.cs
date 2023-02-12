namespace Cofoundry.Domain;

public interface IExtensionRegistrationContext
{
    ExtensionRegistrationContext Add<TModel>(Action<ExtensionRegistrationOptions> optionsConfiguration = null) where TModel : IEntityExtensionDataModel;
}

