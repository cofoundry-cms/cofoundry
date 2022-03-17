using Cofoundry.Core.DependencyInjection;
using Cofoundry.Domain.Internal;

namespace Cofoundry.Domain.Registration;

public class DynamicDataDependencyRegistration : IDependencyRegistration
{
    public void Register(IContainerRegister container)
    {
        container
            .RegisterSingleton<INestedDataModelTypeRepository, NestedDataModelTypeRepository>()
            .Register<IDynamicDataModelSchemaMapper, DynamicDataModelSchemaMapper>()
            .Register<INestedDataModelSchemaMapper, NestedDataModelSchemaMapper>()
            .Register<IEntityDataModelJsonConverterFactory, EntityDataModelJsonConverterFactory>()
            .RegisterAll<INestedDataModel>()
            .RegisterSingleton<DynamicDataModelJsonSerializerSettingsCache>();
        ;
    }
}
