using Cofoundry.Domain;
using System.Collections.Generic;

namespace Cofoundry.Samples.UserAreas
{
    public class DefaultPasswordlessRole
        : IRoleDefinition
        , IRoleInitializer<DefaultPasswordlessRole>
    {
        public const string Code = "DEF";

        public string Title { get { return "Default"; } }

        public string RoleCode { get { return Code; } }

        public string UserAreaCode { get { return PasswordlessUserArea.Code; } }

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