﻿namespace Cofoundry.Domain;

/// <summary>
/// Updates an existing block within a template region 
/// of a custom entity page.
/// </summary>
public class UpdateCustomEntityVersionPageBlockCommand : IPatchableByIdCommand, ILoggableCommand, IPageVersionBlockDataModelCommand
{
    /// <summary>
    /// Id of the block to update.
    /// </summary>
    [Required]
    [PositiveInteger]
    public int CustomEntityVersionPageBlockId { get; set; }

    /// <summary>
    /// The id of the block type to use.
    /// </summary>
    [Required]
    [PositiveInteger]
    public int PageBlockTypeId { get; set; }

    /// <summary>
    /// Optional id of the block type template to render the 
    /// block data into. Some block types have multiple rendering 
    /// templates to choose from.
    /// </summary>
    [PositiveInteger]
    public int? PageBlockTypeTemplateId { get; set; }

    /// <summary>
    /// The block type model data to save against the block.
    /// </summary>
    [Required]
    [ValidateObject]
    public IPageBlockTypeDataModel DataModel { get; set; } = null!;
}
