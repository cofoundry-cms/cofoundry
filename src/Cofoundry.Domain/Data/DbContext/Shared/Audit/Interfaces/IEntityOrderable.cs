namespace Cofoundry.Domain.Data;

/// <summary>
/// Indicates that an entity model has some kind of numerical ordering. 
/// Use <see cref="EntityOrderableHelper"/> to assist with ordering
/// collections of <see cref="IEntityOrderable"/> instances.
/// </summary>
public interface IEntityOrderable
{
    /// <summary>
    /// The numerical order of the entity in a collection.
    /// </summary>
    int Ordering { get; set; }
}
