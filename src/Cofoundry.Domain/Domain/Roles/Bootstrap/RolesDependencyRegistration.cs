using Cofoundry.Core.DependencyInjection;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Internal;

namespace Cofoundry.Domain.Registration
{
    public class RolesDependencyRegistration : IDependencyRegistration
    {
        public void Register(IContainerRegister container)
        {
            var singletonOptions = RegistrationOptions.SingletonScope();

            container
                .Register<IPermissionValidationService, PermissionValidationService>()
                .Register<IExecutePermissionValidationService, ExecutePermissionValidationService>()
                .Register<IRoleCache, RoleCache>()
                .Register<IInternalRoleRepository, InternalRoleRepository>()
                .Register<IRoleDetailsMapper, RoleDetailsMapper>()
                .Register<IRoleMicroSummaryMapper, RoleMicroSummaryMapper>()
                .Register<IRoleDefinitionRepository, RoleDefinitionRepository>()
                .RegisterAll<IPermission>(singletonOptions)
                .RegisterSingleton<IPermissionRepository, PermissionRepository>()

                .RegisterAll<IRoleDefinition>(singletonOptions)
                .RegisterAllGenericImplementations(typeof(IRoleInitializer<>))
                .Register<IRoleInitializerFactory, RoleInitializerFactory>()
                ;
        }
    }
}
