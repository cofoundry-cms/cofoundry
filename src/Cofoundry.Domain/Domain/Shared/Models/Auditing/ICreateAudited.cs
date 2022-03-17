namespace Cofoundry.Domain;

/// <summary>
/// Marks an entity that has audit data for entity creation.
/// </summary>
public interface ICreateAudited
{
    /// <summary>
    /// Simple audit data for entity creation.
    /// </summary>
    CreateAuditData AuditData { get; set; }
}
