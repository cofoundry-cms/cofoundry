using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core.DependencyInjection;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class RolesDependencyRegistration : IDependencyRegistration
    {
        public void Register(IContainerRegister container)
        {
            var singletonOptions = RegistrationOptions.SingletonScope();

            container
                .RegisterType<IPermissionValidationService, PermissionValidationService>()
                .RegisterType<IExecutePermissionValidationService, ExecutePermissionValidationService>()
                .RegisterType<IRoleCache, RoleCache>()
                .RegisterType<IRoleRepository, RoleRepository>()
                .RegisterType<IInternalRoleRepository, InternalRoleRepository>()
                .RegisterType<RoleMappingHelper>()
                .RegisterAll<IPermission>(singletonOptions)
                .RegisterInstance<IPermissionRepository, PermissionRepository>()

                .RegisterAll<IRoleDefinition>(singletonOptions)
                .RegisterAllGenericImplementations(typeof(IRoleInitializer<>))
                .RegisterType<IRoleInitializerFactory, RoleInitializerFactory>()
                ;
        }
    }
}
