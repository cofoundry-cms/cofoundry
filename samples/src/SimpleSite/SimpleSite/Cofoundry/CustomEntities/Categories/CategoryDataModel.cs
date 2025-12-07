using System.ComponentModel.DataAnnotations;

namespace SimpleSite;

/// <summary>
/// This defines the custom data that gets stored with each blog post. Data
/// is stored in an unstructured format (json) so simple data types are 
/// best. For associations, you just need to store the key of the relation.
/// 
/// Attributes can be used to describe validations as well as hints to the 
/// content editor UI on how to render the data input controls.
/// </summary>
public class CategoryDataModel : ICustomEntityDataModel
{
    [MaxLength(500)]
    [Display(Description = "A short description that appears as a tooltip when hovering over the category.")]
    [MultiLineText]
    public string? ShortDescription { get; set; }
}
