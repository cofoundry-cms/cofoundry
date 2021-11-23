namespace Cofoundry.Domain
{
    /// <summary>
    /// Very basic information about an entity. The "root" refers to the fact that 
    /// this instance may refer to "child" or aggregate entity such as a "page version",
    /// whereby <see cref="RootEntityId"/> would then refer to the aggregate root "page". 
    /// This is used mainly in queries that work out dependency graphs for related entities.
    /// </summary>
    /// <remarks>
    /// Given that this class is used in relation to unstructure data dependencies,
    /// the term "root" is unfortunate because "root" is used differently in the
    /// <see cref="Data.UnstructuredDataDependency"/> table to refer to the root
    /// entity in the relationship.
    /// </remarks>
    public class RootEntityMicroSummary
    {
        /// <summary>
        /// Database id of the root entity. The "root" refers to the fact that 
        /// this instance may refer to "child" or aggregate entity such as a "page version",
        /// whereby <see cref="RootEntityId"/> would then refer to the aggregate root entity "page". 
        /// </summary>
        public int RootEntityId { get; set; }

        /// <summary>
        /// Descriptive title of the root entity, suitable for diaply in a GUI or
        /// to be used for formatting messages. The "root" refers to the fact that 
        /// this instance may refer to "child" or aggregate entity such as a "page version",
        /// whereby <see cref="RootEntityTitle"/> would then refer to the aggregate root entity "page". 
        /// </summary>
        public string RootEntityTitle { get; set; }

        /// <summary>
        /// 6 character identifier for the root entity type. Maps from 
        /// <see cref="IEntityDefinition.EntityDefinitionCode"/>.
        /// </summary>
        public string EntityDefinitionCode { get; set; }

        /// <summary>
        /// Descriptive name of the root entity type, suitable for diaply in a GUI or
        /// to be used for formatting messages. Maps from <see cref="IEntityDefinition.Name"/>
        /// </summary>
        public string EntityDefinitionName { get; set; }

        /// <summary>
        /// If this entity is versioned, this indicates if the entity reference relates to a 
        /// previous version e.g. for a page version, this will be <see langword="true"/> if
        /// the version record is not the latest. When managing cascade deletes, constraints
        /// are not enforced on previous versions.
        /// </summary>
        public bool IsPreviousVersion { get; set; }
    }
}
