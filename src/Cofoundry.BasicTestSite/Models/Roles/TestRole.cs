namespace Cofoundry.Domain
{
    public class TestRole : IRoleDefinition
    {
        public const string TestRoleCode = "TST";

        public string Title { get { return "Test"; } }

        public string RoleCode { get { return TestRoleCode; } }

        public string UserAreaCode { get { return CofoundryAdminUserArea.Code; } }

        public void ConfigurePermissions(IPermissionSetBuilder builder)
        {
            builder
                .ApplyRoleConfiguration<AnonymousRole>()
                .IncludePage(c => c.CRUD())
                .IncludeCurrentUser(c => c.Update().Delete())
                .ExcludeUserInCofoundryAdminUserArea()
                .IncludeAllWrite(p => p.ExceptEntityPermissions<PageEntityDefinition>())
                .IncludeAllAdminModule(p => p.ExceptEntityPermissions<PageEntityDefinition>())
                .Include<CofoundryUserCreatePermission>()
                .IncludeAllRead()
                .ExcludeUserInAllUserAreas()
                ;
        }
    }
}