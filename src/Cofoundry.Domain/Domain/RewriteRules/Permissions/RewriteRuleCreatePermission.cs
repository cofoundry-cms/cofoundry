namespace Cofoundry.Domain;

/// <summary>
/// Permission to create new rewrite rules.
/// </summary>
public class RewriteRuleCreatePermission : IEntityPermission
{
    public RewriteRuleCreatePermission()
    {
        EntityDefinition = new RewriteRuleEntityDefinition();
        PermissionType = CommonPermissionTypes.Create("Rewrite Rules");
    }

    public IEntityDefinition EntityDefinition { get; private set; }
    public PermissionType PermissionType { get; private set; }
}
