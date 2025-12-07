using System.ComponentModel.DataAnnotations;

namespace Dev.Sandbox;

/// <summary>
/// This defines the custom data that gets stored with each blog post. Data
/// is stored in an unstructured format (json) so simple data types are 
/// best. For associations, you just need to store the key of the relation.
/// 
/// Attributes can be used to describe validations as well as hints to the 
/// content editor UI on how to render the data input controls.
/// </summary>
public class BlogPostDataModel : ICustomEntityDataModel
{
    [MaxLength(1000)]
    [Required]
    [Display(Description = "A description for display in search results and in the details page meta description.")]
    [MultiLineText]
    public string ShortDescription { get; set; } = string.Empty;

    [Image(MinWidth = 460, MinHeight = 460)]
    [Display(Name = "Thumbnail Image", Description = "Square image that displays against the blog in the listing page.")]
    public int ThumbnailImageAssetId { get; set; }

    [Required]
    [Display(Name = "Author", Description = "The author to attribute the blog post to.")]
    [CustomEntity(AuthorCustomEntityDefinition.DefinitionCode)]
    public int AuthorId { get; set; }

    [Display(Name = "Categories", Description = "Drag and drop to customize the category ordering.")]
    [CustomEntityCollection(CategoryCustomEntityDefinition.DefinitionCode, IsOrderable = true)]
    public int[] CategoryIds { get; set; } = [];
}
