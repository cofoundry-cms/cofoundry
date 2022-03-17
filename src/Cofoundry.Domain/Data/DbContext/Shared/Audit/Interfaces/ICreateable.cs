namespace Cofoundry.Domain.Data;

/// <summary>
/// An entity that stores the date it was created.
/// </summary>
public interface ICreateable
{
    /// <summary>
    /// Date and time at which the entity was created.
    /// </summary>
    DateTime CreateDate { get; set; }
}
