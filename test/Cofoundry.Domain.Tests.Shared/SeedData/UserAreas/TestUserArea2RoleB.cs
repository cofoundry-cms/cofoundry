namespace Cofoundry.Domain.Tests.Shared
{
    public class TestUserArea2RoleB : IRoleDefinition
    {
        public const string Code = "T2B";

        public string Title => "Test User Area 2 Role B";

        public string RoleCode => Code;

        public string UserAreaCode => TestUserArea2.Code;


        public void ConfigurePermissions(IPermissionSetBuilder builder)
        {
            builder.ApplyAnonymousRoleConfiguration();
        }
    }
}