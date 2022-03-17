namespace Cofoundry.Domain;

/// <summary>
/// Minimal information about an entity that is part of an aggregate relationship
/// with a root entity e.g. a "page" is the root entity for a "page version" and 
/// also for a "page version block". This is used to capture information about the
/// relationship between two entities where the relation is between an entity
/// and an aggregate such as a "page version block", but we also need to know what the
/// root of the aggregate is e.g. "page".
/// </summary>
public class ChildEntityMicroSummary : RootEntityMicroSummary
{
    /// <summary>
    /// The database id of the child entity in the aggregate e.g. the id
    /// for a "page version" or "custom entity page block", where the <see cref="RootEntityId"/>
    /// refers to the root e.g. "page".
    /// </summary>
    public int ChildEntityId { get; set; }
}
