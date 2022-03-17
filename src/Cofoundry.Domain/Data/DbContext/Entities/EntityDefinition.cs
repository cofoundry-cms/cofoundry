namespace Cofoundry.Domain.Data;

public class EntityDefinition
{
    /// <summary>
    /// Unique 6 character code representing the entity (use uppercase).
    /// </summary>
    public string EntityDefinitionCode { get; set; }

    /// <summary>
    /// Singlar name of the entity e.g. 'Page'
    /// </summary>
    public string Name { get; set; }

    public virtual ICollection<Permission> Permissions { get; set; } = new List<Permission>();
}
