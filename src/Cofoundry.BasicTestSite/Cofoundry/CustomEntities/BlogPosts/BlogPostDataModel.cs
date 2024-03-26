using System.ComponentModel.DataAnnotations;

namespace Cofoundry.BasicTestSite;

/// <summary>
/// This defines the custom data that gets stored with each blog post. Data
/// is stored in an unstructured format (json) so simple data types are 
/// best. For associations, you just need to store the key of the relation.
/// </summary>
public class BlogPostDataModel : ICustomEntityDataModel
{
    [MaxLength(1000)]
    [Required]
    [Display(Description = "A description for display in search results and in the details page meta description.")]
    [MultiLineText(Rows = 10)]
    public string ShortDescription { get; set; } = string.Empty;

    [Image(MinWidth = 460, MinHeight = 460)]
    [Display(Name = "Thumbnail Image", Description = "Square image that displays against the blog in the listing page.")]
    public int ThumbnailImageAssetId { get; set; }

    [Required]
    [Display(Name = "Categories", Description = "Drag and drop to customize the category ordering.")]
    [CustomEntityCollection(CategoryCustomEntityDefinition.DefinitionCode, IsOrderable = true)]
    public IReadOnlyCollection<int> CategoryIds { get; set; } = Array.Empty<int>();

    [Display(Name = "Category", Description = "Test Single Category.")]
    [CustomEntity(CategoryCustomEntityDefinition.DefinitionCode)]
    public int CategoryId { get; set; }
}
