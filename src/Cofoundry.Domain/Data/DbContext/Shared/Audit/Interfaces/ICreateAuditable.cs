namespace Cofoundry.Domain.Data;

/// <summary>
/// Marks an Entity Framework entity that has audit data 
/// for entity creation.
/// </summary>
public interface ICreateAuditable : ICreateable
{
    /// <summary>
    /// The <see cref="User"/> that created the entity.
    /// </summary>
    User Creator { get; set; }

    /// <summary>
    /// The database id of the <see cref="User"/> that created the entity.
    /// </summary>
    int CreatorId { get; set; }
}
