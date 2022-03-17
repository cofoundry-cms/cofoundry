namespace Cofoundry.Domain;

/// <summary>
/// Read access to rewrite rules. Read access is required in order
/// to include any other rewrite rule permissions.
/// </summary>
public class RewriteRuleReadPermission : IEntityPermission
{
    public RewriteRuleReadPermission()
    {
        EntityDefinition = new RewriteRuleEntityDefinition();
        PermissionType = CommonPermissionTypes.Read("Rewrite Rules");
    }

    public IEntityDefinition EntityDefinition { get; private set; }
    public PermissionType PermissionType { get; private set; }
}
