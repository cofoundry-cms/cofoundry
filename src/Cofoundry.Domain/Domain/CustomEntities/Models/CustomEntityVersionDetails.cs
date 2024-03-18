﻿namespace Cofoundry.Domain;

/// <summary>
/// Heavier-weight projection of a specific custom entity version that
/// includes data model and page region data.
/// </summary>
public class CustomEntityVersionDetails
{
    /// <summary>
    /// Database id of the specific custom entity version this 
    /// model represents.
    /// </summary>
    public int CustomEntityVersionId { get; set; }

    /// <summary>
    /// A display-friendly version number that indicates
    /// it's position in the hisotry of all verions of a specific
    /// custom entity. E.g. the first version for a custom entity 
    /// is version 1 and  the 2nd is version 2. The display version 
    /// is unique per custom entity.
    /// </summary>
    public int DisplayVersion { get; set; }

    /// <summary>
    /// The descriptive human-readable title of the custom entity.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// The workflow state of this version e.g. draft/published.
    /// </summary>
    public WorkFlowStatus WorkFlowStatus { get; set; }

    /// <summary>
    /// Custom entity model data deserialized from the database
    /// into the specific data model type related to this custom 
    /// entity. The interface is used as the property type to avoid 
    /// the complications of having a generic version, but you can 
    /// cast the model to the correct data model type in oder to 
    /// access the properties.
    /// </summary>
    public ICustomEntityDataModel Model { get; set; } = UninitializedCustomEntityDataModel.Instance;

    /// <summary>
    /// Page region and block data for any pages that this custom
    /// entity is associated with.
    /// </summary>
    public IReadOnlyCollection<CustomEntityPage> Pages { get; set; } = Array.Empty<CustomEntityPage>();

    /// <summary>
    /// Simple audit data for custom entity creation.
    /// </summary>
    public CreateAuditData AuditData { get; set; } = CreateAuditData.Uninitialized;

    /// <summary>
    /// A placeholder value to use for not-nullable values that you
    /// know will be initialized in later code. This value should not
    /// be used in data post-initialization.
    /// </summary>
    public static readonly CustomEntityVersionDetails Uninitialized = new()
    {
        CustomEntityVersionId = int.MinValue
    };
}
