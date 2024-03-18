﻿namespace Cofoundry.Domain;

/// <summary>
/// Contains all of the key data for an individual page block in
/// a specific version of a custom entity, but does not include the 
/// mapped display model. Use the CustomEntityVersionPageBlockRenderDetails
/// projection to include the mapped display model.
/// </summary>
public class CustomEntityVersionPageBlockDetails
{
    /// <summary>
    /// Database id and auto-incrementing primary key of the 
    /// custom entity version block record.
    /// </summary>
    public int CustomEntityVersionPageBlockId { get; set; }

    /// <summary>
    /// A page block can optionally have display templates associated with it, 
    /// which will give the user a choice about how the data is rendered out.
    /// If no template is set then the default view is used for rendering.
    /// </summary>
    public PageBlockTypeTemplateSummary? Template { get; set; }

    /// <summary>
    /// The block type that is used to describe and render 
    /// this instance.
    /// </summary>
    public PageBlockTypeSummary BlockType { get; set; } = PageBlockTypeSummary.Uninitialized;

    /// <summary>
    /// The raw data model, deserialized from the database but
    /// not mapped to the block type display model.
    /// </summary>
    public IPageBlockTypeDataModel DataModel { get; set; } = UninitializedPageBlockTypeDataModel.Instance;
}
