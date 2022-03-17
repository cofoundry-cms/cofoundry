namespace Cofoundry.Domain;

/// <summary>
/// Permission to update a rewrite rule.
/// </summary>
public class RewriteRuleUpdatePermission : IEntityPermission
{
    public RewriteRuleUpdatePermission()
    {
        EntityDefinition = new RewriteRuleEntityDefinition();
        PermissionType = CommonPermissionTypes.Update("Rewrite Rules");
    }

    public IEntityDefinition EntityDefinition { get; private set; }
    public PermissionType PermissionType { get; private set; }
}
