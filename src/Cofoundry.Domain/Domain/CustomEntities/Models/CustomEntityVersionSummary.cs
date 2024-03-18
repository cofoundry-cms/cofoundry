﻿namespace Cofoundry.Domain;

/// <summary>
/// Light-weight information about a specific custom entity version
/// that is ideal for displaying in a list. No data model or page data
/// is included in this projection.
/// </summary>
public class CustomEntityVersionSummary : ICreateAudited
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
    /// A page can have many published versions, this flag indicates if
    /// it is the latest published version which displays on the live site
    /// when the page itself is published.
    /// </summary>
    public bool IsLatestPublishedVersion { get; set; }

    /// <summary>
    /// Simple audit data for custom entity creation.
    /// </summary>
    public CreateAuditData AuditData { get; set; } = CreateAuditData.Uninitialized;
}
