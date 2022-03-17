namespace Cofoundry.Domain.Data;

/// <summary>
/// Contains a record of a relation between one entitiy and another
/// when it's defined in unstructured data. Also contains information on how deletions
/// should cascade for the relationship.
/// </summary>
public class UnstructuredDataDependency
{
    /// <summary>
    /// The code identifier of the root entity definition e.g. "COFIMG" for images. The root 
    /// entity is the main entity record e.g. if an image is used in a custom entity data 
    /// model, the image is the root entity and the custom entity is the relation.
    /// </summary>
    public string RootEntityDefinitionCode { get; set; }

    /// <summary>
    /// The entity definition for the root entity. The root entity is the main entity record 
    /// e.g. if an image is used in a custom entity data model, the image is the root entity 
    /// and the custom entity is the relation.
    /// </summary>
    public virtual EntityDefinition RootEntityDefinition { get; set; }

    /// <summary>
    /// The database id of the root entity. The root entity is the main entity record e.g. 
    /// if an image is used in a custom entity data model, the image is the root entity 
    /// and the custom entity is the relation.
    /// </summary>
    public int RootEntityId { get; set; }

    /// <summary>
    /// The code identifier of the entity definition the root is entity is related to e.g. 
    /// "SIMBLP" for the blog post custom entity definition in the basic sample project. The related entity
    /// is the entity that contains a property that references the root entity e.g. if an image 
    /// is used in a custom entity data model, the image is the root entity and the custom entity 
    /// is the relation.
    /// </summary>
    public string RelatedEntityDefinitionCode { get; set; }

    /// <summary>
    /// The entity definition the root is entity is related to. The related entity
    /// is the entity that contains a property that references the root entity e.g. if an image 
    /// is used in a custom entity data model, the image is the root entity and the custom entity 
    /// is the relation.
    /// </summary>
    public virtual EntityDefinition RelatedEntityDefinition { get; set; }

    /// <summary>
    /// The database id of the entity the root is entity is related. The related entity
    /// is the entity that contains a property that references the root entity e.g. if an image 
    /// is used in a custom entity data model, the image is the root entity and the custom entity 
    /// is the relation.
    /// </summary>
    public int RelatedEntityId { get; set; }

    /// <summary>
    /// The action to take on related entities when the root entity is deleted e.g. if an image
    /// is being deleted (the root), what do we do if it is referenced in a custom entity data model?
    /// This value maps to the <see cref="Domain.RelatedEntityCascadeAction"/> enum in the domain layer.
    /// </summary>
    public int RelatedEntityCascadeActionId { get; set; }
}
