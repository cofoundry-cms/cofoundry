using System.ComponentModel.DataAnnotations;

namespace Dev.Sandbox;

/// <summary>
/// This defines the custom data that gets stored with each author. Data
/// is stored in an unstructured format (json) so simple data types are 
/// best. For associations, you just need to store the key of the relation.
/// 
/// Attributes can be used to describe validations as well as hints to the 
/// content editor UI on how to render the data input controls.
/// </summary>
public class AuthorDataModel : ICustomEntityDataModel
{
    [Image(MinWidth = 300, MinHeight = 300)]
    [Display(Name = "Profile Image", Description = "Square image that displays against the author bio.")]
    public int? ProfileImageAssetId { get; set; }

    [MaxLength(500)]
    [Display(Description = "A short bio that appears alongside the author.")]
    [MultiLineText]
    public string? Biography { get; set; }
}
