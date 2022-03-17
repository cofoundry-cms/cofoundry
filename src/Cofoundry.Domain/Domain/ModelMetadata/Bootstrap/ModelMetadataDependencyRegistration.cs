using Cofoundry.Core.DependencyInjection;
using Cofoundry.Domain.Internal;

namespace Cofoundry.Domain.Registration;

public class ModelMetadataDependencyRegistration : IDependencyRegistration
{
    public void Register(IContainerRegister container)
    {
        container
            .RegisterSingleton<CofoundryDisplayMetadataProvider>()
            .RegisterAll<IModelMetadataDecorator>(RegistrationOptions.SingletonScope())
            ;
    }
}
