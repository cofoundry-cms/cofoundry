namespace Cofoundry.Domain
{
    /// <summary>
    /// The action to take on related entities when the root entity is deleted e.g. if an image
    /// is being deleted (the root), what do we do if it is referenced in a custom entity data model?
    /// </summary>
    /// <remarks>
    /// During the implementation of this feature we investigated the idea of having a "CascadeRelatedEntity" 
    /// option that would treat the relation as a direct dependency and delete it if the root entity was deleted.
    /// We also included options here for indicating that a warning should be made e.g. "WarnAndCascadeRelatedEntity"
    /// would warn the user before cascading the delete. Both these options were deemed out of scope at the time, but
    /// may be of interest at a later date.
    /// </remarks>
    public enum RelatedEntityCascadeAction
    {
        /// <summary>
        /// Take no action, the root entity will be prevented from being deleted. This
        /// is typically used when the relation is "required", and removing the relation
        /// would cause an error loading the data model on the related entity.
        /// </summary>
        None = 1,

        /// <summary>
        /// Cascades the delete, removing the relation between the root entity and the related entity.
        /// </summary>
        Cascade = 2
    }
}
