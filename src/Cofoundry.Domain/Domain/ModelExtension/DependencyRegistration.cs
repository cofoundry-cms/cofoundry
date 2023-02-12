using Cofoundry.Core.DependencyInjection;
using Cofoundry.Domain.Internal;

namespace Cofoundry.Domain.ModelExtension.Registration;

public class DependencyRegistration : IDependencyRegistration
{
    public void Register(IContainerRegister container)
    {
        container.RegisterAll<IPageExtensionRegistration>();
        container.RegisterAll<IUserExtensionRegistration>();
        container.RegisterAll<IImageAssetExtensionRegistration>();
        container.RegisterAll<IDocumentAssetExtensionRegistration>();
        container.Register<IPageModelExtensionConfigurationRepository, PageModelExtensionConfigurationRepository>();
        container.Register<IEntityExtensionDataModelDictionaryMapper, EntityExtensionDataModelDictionaryMapper>();
        container.Register<IEntityExtensionDataModelSchemaMapper, EntityExtensionDataModelSchemaMapper>();
    }
}
