namespace SitemapExample.Cofoundry.UserAreas;

public class MemberRole : IRoleDefinition
{
    public const string MemberRoleCode = "MEM";

    public string Title => "Member";

    public string RoleCode => MemberRoleCode;

    public string UserAreaCode => MemberUserArea.Code;

    public void ConfigurePermissions(IPermissionSetBuilder builder)
    {
        builder.ApplyAnonymousRoleConfiguration();
    }
}
