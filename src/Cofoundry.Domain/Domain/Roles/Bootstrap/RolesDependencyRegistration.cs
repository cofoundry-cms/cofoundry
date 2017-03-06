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
            container
                .RegisterType<IPermissionValidationService, PermissionValidationService>()
                .RegisterType<IExecutePermissionValidationService, ExecutePermissionValidationService>()
                .RegisterType<IRoleCache, RoleCache>()
                .RegisterType<IRoleRepository, RoleRepository>()
                .RegisterAll<IPermission>()
                .RegisterInstance<IPermissionRepository, PermissionRepository>()
                .RegisterInstance<IInternalRoleRepository, InternalRoleRepository>()
                .RegisterInstance<RoleMappingHelper>()

                .RegisterAll<IRoleDefinition>()
                .RegisterAllGenericImplementations(typeof(IRoleInitializer<>))
                .RegisterType<IRoleInitializerFactory, RoleInitializerFactory>()
                ;
        }
    }
}
