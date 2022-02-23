namespace Cofoundry.Domain.Tests.Shared
{
    /// <summary>
    /// This test role has all permissions.
    /// </summary>
    public class TestUserArea1RoleA : IRoleDefinition
    {
        public const string Code = "T1A";

        public string Title => "Test User Area 1 Role A";

        public string RoleCode => Code;

        public string UserAreaCode => TestUserArea1.Code;

        public void ConfigurePermissions(IPermissionSetBuilder builder)
        {
            builder.IncludeAll();
        }
    }
}