using Cofoundry.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Samples.UserAreas
{
    public class ExclusivePartnerRoleInitializer : IRoleInitializer<ExclusivePartnerRoleDefinition>
    {
        public IEnumerable<IPermission> GetPermissions(IEnumerable<IPermission> allPermissions)
        {            
            // In this example application we don't require any additional permissions for members
            // so we can re-use the permission set on the anonymous role which include read access 
            // to most entities.
            return allPermissions
                .FilterToAnonymousRoleDefaults()
                ;
        }
    }
}
