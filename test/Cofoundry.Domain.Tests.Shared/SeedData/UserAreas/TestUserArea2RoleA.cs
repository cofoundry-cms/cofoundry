namespace Cofoundry.Domain.Tests.Shared
{
    public class TestUserArea2RoleA : IRoleDefinition
    {
        public const string Code = "T2A";

        public string Title => "Test User Area 2 Role A";

        public string RoleCode => Code;

        public string UserAreaCode => TestUserArea2.Code;

        public void ConfigurePermissions(IPermissionSetBuilder builder)
        {
            builder.IncludeAll();
        }
    }
}