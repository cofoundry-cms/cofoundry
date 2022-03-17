namespace Cofoundry.Domain;

/// <summary>
/// Permission to delete a rewrite rule.
/// </summary>
public class RewriteRuleDeletePermission : IEntityPermission
{
    public RewriteRuleDeletePermission()
    {
        EntityDefinition = new RewriteRuleEntityDefinition();
        PermissionType = CommonPermissionTypes.Delete("Rewrite Rules");
    }

    public IEntityDefinition EntityDefinition { get; private set; }
    public PermissionType PermissionType { get; private set; }
}
