using Cofoundry.Domain;

namespace Cofoundry.Samples.UserAreas
{
    public class DefaultPasswordlessRole : IRoleDefinition
    {
        public const string Code = "DEF";

        public string Title { get { return "Default"; } }

        public string RoleCode { get { return Code; } }

        public string UserAreaCode { get { return PasswordlessUserArea.Code; } }


        public void ConfigurePermissions(IPermissionSetBuilder builder)
        {
            builder.ApplyAnonymousRoleConfiguration();
        }
    }
}