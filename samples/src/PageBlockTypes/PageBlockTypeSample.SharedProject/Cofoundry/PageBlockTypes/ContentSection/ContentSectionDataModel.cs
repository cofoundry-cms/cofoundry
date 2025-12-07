using System.ComponentModel.DataAnnotations;
using Cofoundry.Domain;

namespace PageBlockTypeSample.SharedProject;

/// <summary>
/// This is a simple example block type to demonstrate how block types can be 
/// loaded in from other assemblies.
/// </summary>
public class ContentSectionDataModel : IPageBlockTypeDataModel, IPageBlockTypeDisplayModel
{
    [Display(Description = "Optional title to display at the top of the section")]
    public string? Title { get; set; }

    [Required]
    public string Text { get; set; } = string.Empty;
}
