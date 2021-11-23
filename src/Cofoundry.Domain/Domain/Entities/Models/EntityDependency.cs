namespace Cofoundry.Domain
{
    /// <summary>
    /// Represents an entity reference, where this instance forms part of 
    /// the aggregate of a root entity.
    /// </summary>
    public class EntityDependency
    {
        public EntityDependency(string entityDefinitionCode, int entityId, bool isRequired)
        {
            EntityDefinitionCode = entityDefinitionCode;
            EntityId = entityId;

            RelatedEntityCascadeAction = isRequired ? RelatedEntityCascadeAction.None : RelatedEntityCascadeAction.Cascade;
        }

        /// <summary>
        /// The entity definition code identifier of the entity type this instance
        /// represents e.g. "COFIMG" for the Cofoundry image entity type.
        /// </summary>
        public string EntityDefinitionCode { get; set; }

        /// <summary>
        /// The database identifier for this entity.
        /// </summary>
        public int EntityId { get; set; }

        /// <summary>
        /// The action to take on this entity if the root entity is deleted.
        /// </summary>
        public RelatedEntityCascadeAction RelatedEntityCascadeAction { get; set; }
    }
}
